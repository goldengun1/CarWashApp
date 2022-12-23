using AutoMapper;
using CarWash_App.DTOs;
using CarWash_App.DTOs.UserDTOs;
using CarWash_App.Entities;
using CarWash_App.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarWash_App.Controllers
{
    [ApiController]
    [Route("api/users")]

    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<AccountsController> logger;

        public AccountsController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<AccountsController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]   // api/users/
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<List<ApplicationUserDTO>>> Get([FromQuery] int page = 1)
        {
            var usersQueryable = context.Users.AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(usersQueryable, 10);
            var users = await usersQueryable.Paginate(new PaginationDTO() { Page = page, RecordsPerPage = 10 }).ToListAsync();

            var result = mapper.Map<List<ApplicationUserDTO>>(users);
            return result;
        }

        [HttpGet("personal", Name = "GetPersonal")] // api/users/personal
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ApplicationUserDTO>> Get()
        {

            var curruser = await context.applicationUsers.
                Include(usr => usr.Services)
                .Include(usr => usr.CarWashes)
                .FirstOrDefaultAsync(usr => usr.UserName.Equals(HttpContext.User.Identity.Name));

            if (curruser.IsAnOwner)
                return mapper.Map<OwnerInfoDTO>(curruser);
            else
                return mapper.Map<CustomerInfoDTO>(curruser);

        }

        [HttpPost("signup", Name = "SignUp")] // api/users/signup
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserInfo userCreationInfo)
        {

            var user = new ApplicationUser 
            { 
                UserName = userCreationInfo.UserName,
                Email = userCreationInfo.Email,
                FirstName = userCreationInfo.FirstName,
                LastName = userCreationInfo.LastName,
                IsAnOwner = userCreationInfo.IsOwner
            };
            var result = await userManager.CreateAsync(user, userCreationInfo.Password);

            if (result.Succeeded)
            {
                if (userCreationInfo.IsOwner)
                {
                    await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Owner"));
                    logger.LogInformation($"ACCOUNT CREATED(Owner) : {Request.HttpContext.Connection.RemoteIpAddress}");
                }
                else
                {
                    await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Customer"));
                    logger.LogInformation($"ACCOUNT CREATED(Customer) : {Request.HttpContext.Connection.RemoteIpAddress}");
                }

                await context.SaveChangesAsync();

                return await BuildToken(new UserSignInInfo()
                {
                    UserName = userCreationInfo.UserName,
                    Password = userCreationInfo.Password
                }
                ,userCreationInfo.Email);
            }
            else
                return BadRequest(result.Errors);
        }

        [HttpPost("signin", Name = "SignIn")] // api/users/signin
        public async Task<ActionResult<ExtendedUserToken>> SignIn([FromBody] UserSignInInfo userInfo)
        {
            var result = await signInManager.PasswordSignInAsync(userInfo.UserName, userInfo.Password, isPersistent: false, lockoutOnFailure: false);
            var user = await userManager.FindByNameAsync(userInfo.UserName);

            if (result.Succeeded)
            {
                logger.LogInformation($"USER LOGIN SUCCESS : {userInfo.UserName} From IP:{Request.HttpContext.Connection.RemoteIpAddress}");
                var token =  await BuildToken(userInfo,user.Email);
                return new ExtendedUserToken()
                { 
                    Token = token.Token,
                    Expiration = token.Expiration,
                    isOwner = user.IsAnOwner
                };
            }
            else
            {
                logger.LogInformation($"INVALID LOGIN: Remote IP Address: {Request.HttpContext.Connection.RemoteIpAddress}");
                return BadRequest(new JsonResult("Invalid Sign In attempt!"));
            }
        }

        [HttpPost("signout", Name = "SignOut")] // api/users/signout
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task LogOut()
        {
            //if (signInManager.IsSignedIn(HttpContext.User))
            //{
            //    logger.LogInformation($"LOGGED OUT SUCCES: User: {Request.HttpContext.User.Identity.Name}");
            //    await signInManager.SignOutAsync();
            //}
            //else
            //    return BadRequest("Not logged in!");

            //return Ok("Logged out!");
            await signInManager.SignOutAsync();
            logger.LogInformation($"LOGGED OUT SUCCES: User: {Request.HttpContext.User.Identity.Name}");


        }

        [HttpDelete("deleteaccount", Name = "DeleteAccount")] // api/users/deleteaccount
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserAccess")]
        public async Task<ActionResult> Delete()
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (user == null)
                return NotFound();

            if (user.IsAnOwner)
            {
                var carWashes = await context.carWashes
                    .Include(x => x.Services)
                    .Where(x => x.OwnerId.Equals(user.Id))
                    .ToListAsync();
                if (carWashes != null && carWashes.Count != 0)
                {
                    foreach (var cw in carWashes)
                    {
                        context.Services.RemoveRange(cw.Services);
                    }
                    await context.SaveChangesAsync();
                }
            }

            await userManager.DeleteAsync(user);
            logger.LogInformation($"Account Deleted: {user.Email}");

            return NoContent();
        }

        [HttpGet("renewtoken")] //api/users/renewtoken
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserToken>> RenewToken()
        {
            var email = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            var userName = HttpContext.User.Identity.Name;
            var userInfo = new UserInfo()
            {
                UserName = userName,
                Email = email
            };
            return await BuildToken(userInfo,email);
        }

        [HttpPut("edit")] //api/users/edit
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> EditProfile([FromBody] UserEditDTO editDTO) 
        {
            var currentUser = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (currentUser == null)
                return NotFound();

            currentUser.Email = editDTO.Email;
            currentUser.FirstName = editDTO.FirstName;
            currentUser.LastName = editDTO.LastName;
            currentUser.UserName = editDTO.UserName;

            await context.SaveChangesAsync();
            return Ok("Profile Update Successfuly");
        }

        private async Task<UserToken> BuildToken(UserSignInInfo userInfo, string email)
        {
            var user = await userManager.FindByNameAsync(userInfo.UserName);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userInfo.UserName),
                new Claim(ClaimTypes.Email, email)
            };

            var identityUser = await userManager.FindByNameAsync(userInfo.UserName);
            var claimsFromDB = await userManager.GetClaimsAsync(identityUser);

            claims.AddRange(claimsFromDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.Now.AddMinutes(30);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                signingCredentials: creds,
                claims: claims,
                expires: expiration
            );

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }


    }
}
