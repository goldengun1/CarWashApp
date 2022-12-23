using AutoMapper;
using CarWash_App.DTOs;
using CarWash_App.DTOs.CarWashDTOs;
using CarWash_App.DTOs.ServiceDTOs;
using CarWash_App.Entities;
using CarWash_App.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace CarWash_App.Controllers
{
    [ApiController]
    [Route("api/carwash")]
    public class CarWashesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<CarWashesController> logger;

        public CarWashesController(
            ApplicationDbContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CarWashesController> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<CarWashDetailsDTO>>> Get([FromQuery] PaginationDTO pagination)
        {
            var carWashesQueryable = context.carWashes
                .Include(x => x.Services)
                .Include(x => x.CarWashesServiceTypes)
                .ThenInclude(x => x.ServiceType)
                .AsQueryable();

            await HttpContext.InsertPaginationParametersInResponse(carWashesQueryable,pagination.RecordsPerPage);
            var carWashes = await carWashesQueryable.Paginate(pagination).ToListAsync();
            return mapper.Map<List<CarWashDetailsDTO>>(carWashes);
        }

        [HttpGet("{id:int}", Name = "GetCarWash")]
        public async Task<ActionResult<CarWashDetailsDTO>> Get(int id)
        {
            var carWash = await context.carWashes
                .Include(x => x.Services)
                .Include(x => x.CarWashesServiceTypes)
                .ThenInclude(x => x.ServiceType)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (carWash == null)
                return NotFound();

            return mapper.Map<CarWashDetailsDTO>(carWash);
        }

        [HttpGet("filter", Name = "GetFiltered")]
        public async Task<ActionResult<List<CarWashDTO>>> GetByServiceName([FromQuery] CarWashesFilter filter)
        {
            var carWashesQueryable = context.carWashes
               .Include(cw => cw.CarWashesServiceTypes)
               .ThenInclude(cw => cw.ServiceType)
               .AsQueryable();

            //filtering logic
            if (filter.CurrentlyOpen)
                carWashesQueryable = carWashesQueryable.Where(cw => DateTime.Now.Hour < cw.ClosingTime);

            if (!string.IsNullOrWhiteSpace(filter.ServiceTypeName))
                carWashesQueryable = carWashesQueryable
                    .Where(cw => cw.CarWashesServiceTypes
                        .Any(cwst => cwst.ServiceType.ServiceName.Equals(filter.ServiceTypeName)));

            if (!string.IsNullOrWhiteSpace(filter.OrderingField))
            {
                try
                {
                    carWashesQueryable = carWashesQueryable
                        .OrderBy($"{filter.OrderingField} {(filter.AscendingOrder ? "ascending" : "descending")}");
                }
                catch
                {
                    logger.LogInformation($"INFO: Order by non existing field");
                    return BadRequest($"Could not order by field: {filter.OrderingField}");
                }

            }

            await HttpContext.InsertPaginationParametersInResponse(carWashesQueryable, filter.RecordsPerPage);
            var carWashes = await carWashesQueryable.Paginate(filter.Pagination).ToListAsync();

            return mapper.Map<List<CarWashDTO>>(carWashes);

        }

        [HttpGet("revenueoverview/{id:int}", Name = "RevenueOverview")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Owner")]
        public async Task<ActionResult<string>> GetRevenue(int id, [FromQuery] CarWashRevenueFilter filter)
        {
            var carWash = await context.carWashes
                .Include(cw => cw.Owner)
                .FirstOrDefaultAsync(cw => cw.Id == id);

            //credentials check
            if (!HelperUtils.CredsValidated(httpContextAccessor.HttpContext.User, carWash.Owner.Email))
                return StatusCode(403);

            if (!(filter.Weekly || filter.Daily || filter.Monthly))
                filter.Monthly = true;

            var services = context.Services
                   .Include(sc => sc.ServiceType)
                   .Where(sc => sc.CarWashId == id && sc.ScheduledTime < DateTime.Now);
            if (services == null)
                return NotFound();
            else if (services.Count() == 0)
                return Ok(new JsonResult("No payments collected in the last month."));

            if (filter.Monthly)
            {
                var revenue = services
                    .GroupBy(sc => sc.CarWashId)
                    .Select(scGroup => new
                    {
                        carWashId = scGroup.Key,
                        SumOfRevenue = scGroup.Sum(x => x.ServiceType.Cost)
                    });
                return Ok(new JsonResult($"Monthly revenue for carwash: {revenue.First().SumOfRevenue}$"));
            }
            else if (filter.Weekly)
            {
                var lastWeek = DateTime.Now.Subtract(TimeSpan.FromDays(7));
                var revenue = services
                    .Where(sc => sc.ScheduledTime > lastWeek)
                    .GroupBy(sc => sc.CarWashId)
                    .Select(scGroup => new
                    {
                        carWashId = scGroup.Key,
                        SumOfRevenue = scGroup.Sum(x => x.ServiceType.Cost)
                    });
                if (!await revenue.AnyAsync())
                    return Ok(new JsonResult("No payments collected in the last week."));

                return Ok(new JsonResult($"Weekly revenue for carwash: {revenue.First().SumOfRevenue}$"));
            }
            else
            {

                //var today = new DateTime(DateTime.Now.Year, DateTime.Now.Year, DateTime.Now.Day);
                var today = DateTime.Now.Subtract(TimeSpan.FromHours(DateTime.Now.Hour));

                var revenue = services
                    .Where(sc => sc.ScheduledTime > today)
                    .GroupBy(sc => sc.CarWashId)
                    .Select(scGroup => new
                    {
                        carWashId = scGroup.Key,
                        SumOfRevenue = scGroup.Sum(x => x.ServiceType.Cost)
                    });
                if (!await revenue.AnyAsync())
                    return Ok(new JsonResult("No payments collected today."));

                return Ok(new JsonResult($"Todays revenue for carwash: {revenue.First().SumOfRevenue}$"));
            }
        }

        [HttpGet("getmyshops", Name = "GetPersonalShops")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Owner")]
        public async Task<ActionResult<List<CarWashDetailsDTO>>> GetMyShops()
        {
            var owner = await context.applicationUsers
                .FirstOrDefaultAsync(usr => usr.UserName.Equals(httpContextAccessor.HttpContext.User.Identity.Name));

            var carWash = await context.carWashes
                .Where(x => x.OwnerId == owner.Id)
                .Include(x => x.Services)
                .Include(x => x.CarWashesServiceTypes)
                .ThenInclude(x => x.ServiceType)
                .ToListAsync();

            if (carWash == null)
                return NotFound();

            return mapper.Map<List<CarWashDetailsDTO>>(carWash);
        }

        [HttpGet("{id:int}/services", Name = "GetCarWashServices")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = "Owner")]
        public async Task<ActionResult<CarWashScheduledServicesDTO>> GetServices(int id)
        {
            var carWash = await context.carWashes
                .Include(cw => cw.Services)
                .ThenInclude(svc => svc.ServiceType)
                .Include(cw => cw.Owner)
               .FirstOrDefaultAsync(x => x.Id == id);
            if (carWash == null)
                return NotFound();

            //credentials check
            if (!HelperUtils.CredsValidated(httpContextAccessor.HttpContext.User, carWash.Owner.Email))
                return StatusCode(403);

            return mapper.Map<CarWashScheduledServicesDTO>(carWash);
        }

        [HttpGet("{id:int}/stats")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Owner")]
        public async Task<ActionResult<CarWashStatsDTO>> GetCarWashStats(int id)
        {
            var carWash = await context.carWashes
                .Include(cw => cw.Owner)
                .Include(cw => cw.Services)
                .ThenInclude(svc => svc.ServiceType)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (carWash == null)
                return NotFound();

            //credentials check
            if (!HelperUtils.CredsValidated(httpContextAccessor.HttpContext.User, carWash.Owner.Email))
                return StatusCode(403);

            return HelperUtils.CalculateCarWashStatistics(carWash);

        }

        [HttpPost(Name = "CreateCarWash")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Owner")]
        public async Task<ActionResult> Post([FromBody] CarWashCreationDTO carWashCreationDTO)
        {
            var currUser = await context.applicationUsers.FirstOrDefaultAsync(usr => usr.UserName.Equals(httpContextAccessor.HttpContext.User.Identity.Name));
            
            var carWash = mapper.Map<CarWash>(carWashCreationDTO);
            carWash.OwnerId = currUser.Id;

            context.carWashes.Add(carWash);
            await context.SaveChangesAsync();

            var returnViewDTO = mapper.Map<CarWashDTO>(carWash);
            return new CreatedAtRouteResult("GetCarWash", new { carWash.Id }, returnViewDTO);
        }

        [HttpPost("confirmservice")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Owner")]
        public async Task<ActionResult> ConfirmService([FromBody] ServiceConfirmationDTO confirmationDTO)
        {
            var service = await context.Services
                .Include(svc => svc.CarWash)
                .ThenInclude(cw => cw.Owner)
                .FirstOrDefaultAsync(x => x.Id == confirmationDTO.ServiceId && x.CarWashId == confirmationDTO.CarWashId);

            if (service == null)
                return NotFound();

            //credentials check
            if (!HelperUtils.CredsValidated(httpContextAccessor.HttpContext.User, service.CarWash.Owner.Email))
            {
                logger.LogWarning($"PERMISSION DENIED:{Environment.NewLine}\tUnauthorized access on {HttpContext.Request.Path}{Environment.NewLine}"
                                    + $"\tUsername: {httpContextAccessor.HttpContext.User.Identity.Name}{Environment.NewLine}"
                                    + $"\tTried action: CONFIRM");

                return StatusCode(403);
            }

            if (service.Confirmed)
                return NoContent();

            service.Confirmed = true;
            context.Services.Update(service);
            await context.SaveChangesAsync();
            return Ok("Service confirmed");
        }

        [HttpPost("{id:int}/confirmall")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Owner")]
        public async Task<ActionResult> ConfirmAllService(int id)
        {
            var services = await context.Services
                .Include(svc => svc.CarWash)
                .ThenInclude(cw => cw.Owner)
                .Where(x => x.CarWashId == id && x.Confirmed==false)
                .ToListAsync();

            if(services == null)
                return NotFound();

            if (services.Count == 0)
                return NoContent();

            //credentials check
            if (!HelperUtils.CredsValidated(httpContextAccessor.HttpContext.User, services[0].CarWash.Owner.Email))
            {
                logger.LogWarning($"PERMISSION DENIED:{Environment.NewLine}\tUnauthorized access on {HttpContext.Request.Path}{Environment.NewLine}"
                                     + $"\tUsername: {httpContextAccessor.HttpContext.User.Identity.Name}{Environment.NewLine}"
                                     + $"\tTried action: CONFIRM");

                return StatusCode(403);
            }

            foreach (var service in services)
            { 
                service.Confirmed = true;
                context.Services.Update(service);
            }
            await context.SaveChangesAsync();
            return Ok("Accepted all unconfirmed sheduled services");
        }

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Owner")]
        public async Task<ActionResult> Delete(int id)
        {

            var cw = await context.carWashes
                .Include(cw => cw.Services)
                .Include(cw => cw.Owner)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (cw == null)
                return NotFound();

            //credentials check
            if (!HelperUtils.CredsValidated(httpContextAccessor.HttpContext.User, cw.Owner.Email))
            {
                logger.LogWarning($"PERMISSION DENIED:{Environment.NewLine}\tUnauthorized access on {HttpContext.Request.Path}{Environment.NewLine}"
                                     + $"\tUsername: {httpContextAccessor.HttpContext.User.Identity.Name}{Environment.NewLine}"
                                     + $"\tTried action: DELETE");
                return StatusCode(403);

            }

            var services = cw.Services;
            context.Services.RemoveRange(services);
            context.carWashes.Remove(cw);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("rejectservice")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Owner")]
        public async Task<ActionResult> RejectBookedService([FromBody] ServiceConfirmationDTO rejectionDTO)
        {
            var service = await context.Services
                .Include(sc => sc.CarWash)
                .ThenInclude(cw => cw.Owner)
                .FirstOrDefaultAsync(x => x.Id == rejectionDTO.ServiceId && x.CarWashId == rejectionDTO.CarWashId);

            if (service == null)
                return NotFound();

            //credentials check
            if (!HelperUtils.CredsValidated(httpContextAccessor.HttpContext.User, service.CarWash.Owner.Email))
            {
                logger.LogWarning($"PERMISSION DENIED:{Environment.NewLine}\tUnauthorized access on {HttpContext.Request.Path}{Environment.NewLine}"
                                     + $"\tUsername: {httpContextAccessor.HttpContext.User.Identity.Name}{Environment.NewLine}"
                                     + $"\tTried action: REJECT_SERVICE");
                return StatusCode(403);
            }

            var timeLeft = service.ScheduledTime.Subtract(DateTime.Now);
            if (timeLeft < TimeSpan.FromHours(1))
                return BadRequest(new JsonResult("Unable to cancel service that is less than 1h from now."));


            context.Services.Remove(service);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut] //api/carwash
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Owner")]
        public async Task<ActionResult> Put([FromBody] CarWashCreationDTO carWashCreationDTO)
        {
            var carWash = await context.carWashes
                .Include(cw => cw.Owner)
                .FirstOrDefaultAsync(x => x.Id == carWashCreationDTO.Id);

            if (carWash == null)
                return NotFound();

            //credentials check
            if (!HelperUtils.CredsValidated(httpContextAccessor.HttpContext.User, carWash.Owner.Email))
                return StatusCode(403);

            carWash = mapper.Map(carWashCreationDTO, carWash);

            //ImportantNote: This must be commented for unit testing
            await context.Database.ExecuteSqlInterpolatedAsync($"delete from CarWashesServiceTypes where CarWashId = {carWash.Id}");

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Owner")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<CarWashPatchDTO> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var carWash = await context.carWashes
                .Include(cw => cw.Owner)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (carWash == null)
                return NotFound();

            //credentials check
            if (!HelperUtils.CredsValidated(httpContextAccessor.HttpContext.User, carWash.Owner.Email))
                return StatusCode(403);

            var patchDTO = mapper.Map<CarWashPatchDTO>(carWash);
            patchDoc.ApplyTo(patchDTO, ModelState);

            var isValid = TryValidateModel(patchDTO);
            if (!isValid)
                return BadRequest(ModelState);

            mapper.Map(patchDTO, carWash);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
