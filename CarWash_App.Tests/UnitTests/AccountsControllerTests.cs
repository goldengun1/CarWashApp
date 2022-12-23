using CarWash_App.Controllers;
using CarWash_App.DTOs.UserDTOs;
using CarWash_App.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CarWash_App.Tests.UnitTests
{
    [TestClass]
    public class AccountsControllerTests : BaseTests
    {
        [TestMethod]
        public async Task UserIsCreated()
        {
            var databaseName = Guid.NewGuid().ToString();
            await CreateAnUser(databaseName);
            var context2 = BuildContext(databaseName);
            var count = await context2.Users.CountAsync();
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task UserCanNotLogIn()
        {
            var databaseName = Guid.NewGuid().ToString();
            await CreateAnUser(databaseName);

            var controller = BuildAccountsController(databaseName);
            controller.ControllerContext = BuildControllerContextWithDefaultCustomer();
            var userInfo = new UserSignInInfo() { UserName = "customer.customer", Password = "BadPassword" };
            var response = await controller.SignIn(userInfo);
            var result = response.Result as BadRequestObjectResult;

            Assert.IsNull(response.Value);
            Assert.AreEqual(400, result.StatusCode);
        }

        [TestMethod]
        public async Task UserLogInSuccess()
        {
            var databaseName = Guid.NewGuid().ToString();
            await CreateAnUser(databaseName);

            var controller = BuildAccountsController(databaseName);
            controller.ControllerContext = BuildControllerContextWithDefaultCustomer();
            var userInfo = new UserInfo() { UserName = "customer.customer", Email = "test@testmail.com", Password = "ABCabc123!" };
            var response = await controller.SignIn(userInfo);

            Assert.IsNotNull(response.Value);
        }

        [TestMethod]
        public async Task DeleteAccountSuccess()
        {
            var databaseName = Guid.NewGuid().ToString();
            await CreateAnUser(databaseName);

            var controller = BuildAccountsController(databaseName);
            controller.ControllerContext = BuildControllerContextWithDefaultCustomer();
            var response = await controller.Delete();
            var result = response as NoContentResult;
            Assert.AreEqual(204, result.StatusCode);

        }

        private async Task CreateAnUser(string databaseName)
        {
            var controller = BuildAccountsController(databaseName);
            controller.ControllerContext = BuildControllerContextWithDefaultCustomer();
            var userInfo = new UserInfo()
            {
                FirstName = "Name",
                LastName = "LastName",
                IsOwner = true,
                UserName = "customer.customer",
                Email = "test@testmail.com",
                Password = "ABCabc123!"
            };
            await controller.CreateUser(userInfo);
        }

        private AccountsController BuildAccountsController(string databaseName)
        {
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var myUserStore = new UserStore<ApplicationUser>(context);
            var userManager = BuildUserManager(myUserStore);

            var httpContext = new DefaultHttpContext();
            MockAuth(httpContext);

            var signInManager = SetupSignInManager(userManager, httpContext);

            var myConfiguration = new Dictionary<string, string>
            {
                { "JWT:key", "O?>}AUYEVGE(@&*#}{ASDYF(#*PIASF}AS"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            var loggerMoq = Mock.Of<ILogger<AccountsController>>();

            return new AccountsController(userManager, signInManager, configuration, context, mapper,loggerMoq);
        }

        private static SignInManager<TUser> SetupSignInManager<TUser>(UserManager<TUser> manager,
            HttpContext context, ILogger logger = null, IdentityOptions identityOptions = null,
            IAuthenticationSchemeProvider schemeProvider = null) where TUser : class
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(context);
            identityOptions = identityOptions ?? new IdentityOptions();
            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(a => a.Value).Returns(identityOptions);
            var claimsFactory = new UserClaimsPrincipalFactory<TUser>(manager, options.Object);
            schemeProvider = schemeProvider ?? new Mock<IAuthenticationSchemeProvider>().Object;
            var sm = new SignInManager<TUser>(manager, contextAccessor.Object, claimsFactory, options.Object, null, schemeProvider, new DefaultUserConfirmation<TUser>());
            sm.Logger = logger ?? (new Mock<ILogger<SignInManager<TUser>>>()).Object;
            return sm;
        }

        private UserManager<TUser> BuildUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
        {
            store = store ?? new Mock<IUserStore<TUser>>().Object;
            var options = new Mock<IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions();
            idOptions.Lockout.AllowedForNewUsers = false;

            options.Setup(o => o.Value).Returns(idOptions);

            var userValidators = new List<IUserValidator<TUser>>();

            var validator = new Mock<IUserValidator<TUser>>();
            userValidators.Add(validator.Object);
            var pwdValidators = new List<PasswordValidator<TUser>>();
            pwdValidators.Add(new PasswordValidator<TUser>());

            var userManager = new UserManager<TUser>(store, options.Object, new PasswordHasher<TUser>(),
                userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null,
                new Mock<ILogger<UserManager<TUser>>>().Object);

            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            return userManager;
        }

        private Mock<IAuthenticationService> MockAuth(HttpContext context)
        {
            var auth = new Mock<IAuthenticationService>();
            context.RequestServices = new ServiceCollection().AddSingleton(auth.Object).BuildServiceProvider();
            return auth;
        }
    }
}
