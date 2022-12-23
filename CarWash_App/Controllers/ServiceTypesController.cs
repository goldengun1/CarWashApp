using AutoMapper;
using CarWash_App.DTOs.ServiceTypeDTOs;
using CarWash_App.Entities;
using CarWash_App.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace CarWash_App.Controllers
{
    [ApiController]
    [Route("api/servicetype")]
    public class ServiceTypesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<ServiceTypesController> logger;

        public ServiceTypesController(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<ServiceTypesController> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }
        [HttpGet(Name = "ListAllServiceTypes")]
        public async Task<ActionResult<List<ServiceTypeDTO>>> Get([FromQuery] ServiceTypeFilterDTO filter)
        {
            var serviceTypesQueryable = context.ServiceTypes
                .Include(st => st.CarWashesServiceTypes)
                .AsQueryable();

            if (filter.MaxDuration != 0)
                serviceTypesQueryable = serviceTypesQueryable.Where(st => st.Duration.Hours <= filter.MaxDuration);

            if (filter.MaxCost != 0)
                serviceTypesQueryable = serviceTypesQueryable.Where(st => st.Cost <= filter.MaxCost);

            if (filter.AvailableAnywhere)
                serviceTypesQueryable = serviceTypesQueryable
                    .Where(st => st.CarWashesServiceTypes.Any());

            if (!string.IsNullOrWhiteSpace(filter.OrderingField))
            {
                try
                {
                    serviceTypesQueryable = serviceTypesQueryable
                        .OrderBy($"{filter.OrderingField} {(filter.AscendingOrder ? "ascending" : "descending")}");
                }
                catch
                {
                    logger.LogInformation($"INFO: Order by non existing field: {filter.OrderingField}");
                    return BadRequest($"Could not order buy field: {filter.OrderingField}{Environment.NewLine}");
                }
            }

            await HttpContext.InsertPaginationParametersInResponse(serviceTypesQueryable, filter.RecordsPerPage);
            var serviceTypes = await serviceTypesQueryable.Paginate(filter.Pagination).ToListAsync();

            //var serviceTypes = await context.ServiceTypes.ToListAsync();
            return mapper.Map<List<ServiceTypeDTO>>(serviceTypes);
        }


        [HttpGet("{id:int}", Name = "GetServiceType")]
        public async Task<ActionResult<ServiceTypeDetailsDTO>> GetById(int id)
        {
            var serviceType = await context.ServiceTypes
                .Include(x => x.CarWashesServiceTypes)
                .ThenInclude(x => x.CarWash)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (serviceType == null)
                return NotFound();
            return mapper.Map<ServiceTypeDetailsDTO>(serviceType);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerAccess")]
        public async Task<ActionResult> Post([FromBody] ServiceTypeCreationDTO serviceTypeCreationDTO)
        {
            var serviceType = mapper.Map<ServiceType>(serviceTypeCreationDTO);

            context.Add(serviceType);
            await context.SaveChangesAsync();

            var returnViewDTO = mapper.Map<ServiceTypeDTO>(serviceType);
            return new CreatedAtRouteResult("GetCarWash", new { serviceType.Id }, returnViewDTO);
        }

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await context.ServiceTypes.AnyAsync(x => x.Id == id);

            if (!exists)
                return NotFound();

            context.ServiceTypes.Remove(new ServiceType() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Put(int id, [FromForm] ServiceTypeCreationDTO serviceTypeCreationDTO)
        {
            var serviceType = await context.ServiceTypes.FirstOrDefaultAsync(x => x.Id == id);

            if (serviceType == null)
                return NotFound();

            serviceType = mapper.Map(serviceTypeCreationDTO, serviceType);


            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
