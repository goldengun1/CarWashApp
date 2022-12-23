using AutoMapper;
using CarWash_App.DTOs.ServiceDTOs;
using CarWash_App.Entities;
using CarWash_App.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarWash_App.Controllers
{
    [ApiController]
    [Route("api/service")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ServicesController : ControllerBase
    {
        private ApplicationDbContext context;
        private IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<ServicesController> logger;

        public ServicesController(
            ApplicationDbContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ServicesController> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<List<ServiceDTO>>> Get()
        {
            var services = await context.Services.ToListAsync();
            return mapper.Map<List<ServiceDTO>>(services);
        }

        [HttpGet("getmyservices")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
        public async Task<ActionResult<List<ServiceDetailsDTO>>> GetMyServices()
        {
            var currUser = await context.applicationUsers.FirstOrDefaultAsync(usr => usr.UserName.Equals(httpContextAccessor.HttpContext.User.Identity.Name));

            var services = await context.Services
                .Include(svc => svc.Customer)
                .Include(svc => svc.ServiceType)
                .Include(svc => svc.CarWash)
                .ThenInclude(cw => cw.Owner)
                .Where(svc => svc.CustomerId == currUser.Id)
                .ToListAsync();
            return mapper.Map<List<ServiceDetailsDTO>>(services);
        }

        [HttpGet("{id:int}", Name = "GetService")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserAccess")]
        public async Task<ActionResult<ServiceDetailsDTO>> Get(int id)
        {
            var service = await context.Services
                .Include(svc => svc.Customer)
                .Include(svc => svc.ServiceType)
                .Include(svc => svc.CarWash)
                .ThenInclude(cw => cw.Owner)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (service == null)
                return NotFound();

            //creds check
            if (!HelperUtils.CredsValidated(httpContextAccessor.HttpContext.User, service.Customer.Email)
                && !HelperUtils.CredsValidated(httpContextAccessor.HttpContext.User, service.CarWash.Owner.Email))
            {
                logger.LogWarning($"PERMISSION DENIED:{Environment.NewLine}\tUnauthorized access on {HttpContext.Request.Path}{Environment.NewLine}"
                                     + $"\tUsername: {httpContextAccessor.HttpContext.User.Identity.Name}{Environment.NewLine}"
                                     + $"\tTried action: GET");
                return StatusCode(403);

            }

            return mapper.Map<ServiceDetailsDTO>(service);
        }

        [HttpPost(Name = "ScheduleService")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
        public async Task<ActionResult> Post([FromBody] ServiceCreationDTO serviceCreationDTO)
        {
            var currUser = await context.applicationUsers.FirstOrDefaultAsync(usr => usr.UserName.Equals(httpContextAccessor.HttpContext.User.Identity.Name));
            if (currUser.IsAnOwner)
                return BadRequest($"Can't schedule service as an owner");

            var carWash = await context.carWashes
                .Include(cw => cw.Services.Where(x =>
                    x.ScheduledTime.Year == serviceCreationDTO.ScheduledTime.Year
                    && x.ScheduledTime.Month == serviceCreationDTO.ScheduledTime.Month
                    && x.ScheduledTime.Day == serviceCreationDTO.ScheduledTime.Day)
                )
                .ThenInclude(svc => svc.ServiceType)
                .FirstOrDefaultAsync(cw => cw.Id == serviceCreationDTO.CarWashId);
            if(carWash == null)
                return BadRequest(new JsonResult("Car Wash not found"));

            //availability check
            var serviceType = await context.ServiceTypes
                .FirstOrDefaultAsync(x => x.Id == serviceCreationDTO.ServiceTypeId);
            if (!HelperUtils.CheckAvailability(carWash, serviceCreationDTO, serviceType))
                return BadRequest(new JsonResult("Unable to schedule service at desired date/time"));


            var service = mapper.Map<Service>(serviceCreationDTO);
            service.CustomerId = currUser.Id;

            context.Services.Add(service);
            await context.SaveChangesAsync();

            var returnViewDTO = mapper.Map<ServiceDTO>(service);
            return new CreatedAtRouteResult("GetCarWash", new { service.Id }, returnViewDTO);
        }

        [HttpDelete("cancelService/{id:int}", Name = "CancelService")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerAccess")]
        public async Task<ActionResult> CancelService(int id)
        {

            var service = await context.Services
                .Include(svc => svc.Customer)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (service == null)
                return NotFound();

            //credentials check
            if (!HelperUtils.CredsValidated(httpContextAccessor.HttpContext.User, service.Customer.Email))
            {
                logger.LogWarning($"PERMISSION DENIED:{Environment.NewLine}\tUnauthorized service deleting on {HttpContext.Request.Path}{Environment.NewLine}"
                                     + $"\tUsername: {httpContextAccessor.HttpContext.User.Identity.Name}{Environment.NewLine}"
                                     + $"\tTried action: CANCELING");
                return StatusCode(403);

            }

            if (!service.EligibleForCancelation)
                return BadRequest("Service not eligable for cancelation at this time.");
            
            context.Services.Remove(service);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerAccess")]
        public async Task<ActionResult> Put([FromForm] ServiceCreationDTO serviceCreationDTO)
        {
            var service = await context.Services
                .Include(svc => svc.Customer)
                .FirstOrDefaultAsync(x => x.Id == serviceCreationDTO.Id);

            if (service == null)
                return NotFound();

            //credentials check
            if (!HelperUtils.CredsValidated(httpContextAccessor.HttpContext.User, service.Customer.Email))
                return StatusCode(403);

            service = mapper.Map(serviceCreationDTO, service);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerAccess")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ServicePatchDTO> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var service = await context.Services.FirstOrDefaultAsync(x => x.Id == id);

            if (service == null)
                return NotFound();

            //credentials check
            if (!HelperUtils.CredsValidated(httpContextAccessor.HttpContext.User, service.Customer.Email))
                return StatusCode(403);


            var patchDTO = mapper.Map<ServicePatchDTO>(service);
            patchDoc.ApplyTo(patchDTO, ModelState);

            var isValid = TryValidateModel(patchDTO);
            if (!isValid)
                return BadRequest(ModelState);

            mapper.Map(patchDTO, service);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
