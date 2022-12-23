using AutoMapper;
using CarWash_App.DTOs.CarWashDTOs;
using CarWash_App.DTOs.ServiceDTOs;
using CarWash_App.DTOs.ServiceTypeDTOs;
using CarWash_App.DTOs.UserDTOs;
using CarWash_App.Entities;

namespace CarWash_App.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CarWash, CarWashDTO>();
            CreateMap<CarWashCreationDTO, CarWash>()
               .ForMember(x => x.CarWashesServiceTypes, options => options.MapFrom(MapServiceTypes))
               .ForMember(x => x.Id,options => options.Ignore())
               .ForMember(x => x.OwnerId,options => options.Ignore())
               .ForMember(x => x.Profit, options => options.Ignore())
               .ForMember(x => x.OwnerId, options => options.Ignore())
               .ForMember(x => x.Owner, options => options.Ignore())
               .ForMember(x => x.Services, options => options.Ignore());
            CreateMap<CarWash, CarWashDetailsDTO>()
                .ForMember(x => x.ScheduledServices, options => options.MapFrom(MapScheduledServices))
                .ForMember(x => x.OfferedServices, options => options.MapFrom(MapOfferedServices));
            CreateMap<CarWash, CarWashScheduledServicesDTO>()
                .ForMember(x => x.Services, options => options.MapFrom(MapServices));


            CreateMap<ServiceType, ServiceTypeDTO>()
               .ForMember(x => x.Cost, options => options.MapFrom(x => $"{x.Cost} $"));
            CreateMap<ServiceTypeCreationDTO, ServiceType>()
               .ForMember(x => x.Duration, options => options.MapFrom(x => new TimeSpan(x.Duration, 0, 0)))
               .ForMember(x => x.CarWashesServiceTypes, options => options.MapFrom(MapCarWashes))
               .ForMember(x => x.Id, options => options.Ignore())
               .ForMember(x => x.Services, options => options.Ignore());
            CreateMap<ServiceType, ServiceTypeDetailsDTO>()
               .ForMember(x => x.CarWashes, options => options.MapFrom(MapCarWashesMinimal));


            CreateMap<Service, ServiceDTO>();
            CreateMap<Service, ServiceDetailsDTO>()
                .ForMember(x => x.Customer, options => options.MapFrom(x => $"{x.Customer.FirstName} {x.Customer.LastName}"))
                .ForMember(x => x.CarWashName, options => options.MapFrom(x => $"{x.CarWash.CarWashName}"))
                .ForMember(x => x.ServiceTypeInfo, options => options.MapFrom(x => $"{x.ServiceType.ServiceName} {x.ServiceType.Cost}$"));
            CreateMap<ServiceCreationDTO, Service>()
                .ForMember(x => x.Id, options => options.Ignore())
                .ForMember(x => x.CustomerId, options => options.Ignore())
                .ForMember(x => x.EligibleForCancelation, options => options.Ignore())
                .ForMember(x => x.Confirmed, options => options.Ignore())
                .ForMember(x => x.PaymentCollected, options => options.Ignore())
                .ForMember(x => x.Customer, options => options.Ignore())
                .ForMember(x => x.CarWash, options => options.Ignore())
                .ForMember(x => x.ServiceType, options => options.Ignore());
            CreateMap<ServicePatchDTO, Service>()
                .ForMember(x => x.Id, options => options.Ignore())
                .ForMember(x => x.CustomerId, options => options.Ignore())
                .ForMember(x => x.EligibleForCancelation, options => options.Ignore())
                .ForMember(x => x.Confirmed, options => options.Ignore())
                .ForMember(x => x.PaymentCollected, options => options.Ignore())
                .ForMember(x => x.Customer, options => options.Ignore())
                .ForMember(x => x.CarWash, options => options.Ignore())
                .ForMember(x => x.ServiceType, options => options.Ignore())
                .ReverseMap();


            CreateMap<ApplicationUser,ApplicationUserDTO>()
                .ForMember(usr => usr.EmailConfirmed, options => options.MapFrom(x => x.EmailConfirmed ? "Yes":"No"))
                .ForMember(usr => usr.AccountType, options => options.MapFrom(x => x.IsAnOwner ? "Owner" : "Customer"));
            CreateMap<ApplicationUser, OwnerInfoDTO>()
                .ForMember(usr => usr.CarWashes, options => options.MapFrom(MapOwner))
                .ForMember(usr => usr.EmailConfirmed, options => options.MapFrom(x => x.EmailConfirmed ? "Yes" : "No"))
                .ForMember(usr => usr.AccountType, options => options.MapFrom(x => x.IsAnOwner ? "Owner" : "Customer"));
            CreateMap<ApplicationUser, CustomerInfoDTO>()
               .ForMember(usr => usr.Services, options => options.MapFrom(MapCustomer))
               .ForMember(usr => usr.EmailConfirmed, options => options.MapFrom(x => x.EmailConfirmed ? "Yes" : "No"))
               .ForMember(usr => usr.AccountType, options => options.MapFrom(x => x.IsAnOwner ? "Owner" : "Customer"));

        }

        private List<CarWashesServiceTypes> MapServiceTypes(CarWashCreationDTO carWashCreationDTO, CarWash carWash)
        {
            var result = new List<CarWashesServiceTypes>();
            if (carWashCreationDTO.ServiceTypeIds == null)
                return result;

            foreach (var serviceTypeId in carWashCreationDTO.ServiceTypeIds)
            {
                result.Add(new CarWashesServiceTypes() { ServiceTypeId = serviceTypeId });
            }
            return result;
        }

        private List<ServiceTypeDTO> MapOfferedServices(CarWash carWash, CarWashDetailsDTO carWashDetailsDTO)
        {
            var result = new List<ServiceTypeDTO>();

            foreach (var serviceType in carWash.CarWashesServiceTypes)
            {
                result.Add(new ServiceTypeDTO()
                {
                    Id = serviceType.ServiceType.Id,
                    ServiceName = serviceType.ServiceType.ServiceName,
                    Duration = serviceType.ServiceType.Duration,
                    Cost = $"{serviceType.ServiceType.Cost} $"
                });
            }
            return result;
        }

        private List<ServiceDTO> MapScheduledServices(CarWash carWash, CarWashDetailsDTO carWashDetailsDTO)
        {
            var result = new List<ServiceDTO>();

            foreach (var service in carWash.Services)
            {
                result.Add(new ServiceDTO()
                {
                   Id = service.Id,
                   CarWashId = (int)service.CarWashId,
                   CustomerId = service.CustomerId,
                   ServiceTypeId = service.ServiceTypeId,
                   ScheduledTime = service.ScheduledTime,
                   EligibleForCancelation = service.EligibleForCancelation,
                   Confirmed = service.Confirmed
                });
            }
            return result;
        }

        private List<CarWashesServiceTypes> MapCarWashes(ServiceTypeCreationDTO serviceTypeCreationDTO, ServiceType serviceType)
        {
            var result = new List<CarWashesServiceTypes>();
            if (serviceTypeCreationDTO.CarWashIds == null)
                return result;

            foreach (var carWashId in serviceTypeCreationDTO.CarWashIds)
            {
                result.Add(new CarWashesServiceTypes() { CarWashId = carWashId });
            }
            return result;
        }

        private List<ServicesMinimalDTO> MapServices(CarWash carWash, CarWashScheduledServicesDTO carWashScheduledServicesDTO)
        {
            var result = new List<ServicesMinimalDTO>();

            foreach (var service in carWash.Services)
            {
                result.Add(new ServicesMinimalDTO()
                {
                    Id = service.Id,
                    CustomerId = service.CustomerId,
                    CarWashId = (int)service.CarWashId,
                    ScheduledTime = service.ScheduledTime,
                    ServiceTypeId = service.ServiceTypeId,
                    ServiceTypeName = service.ServiceType.ServiceName
                });
            }
            return result;
        }

        private List<CarWashMinimalDTO> MapCarWashesMinimal(ServiceType serviceType, ServiceTypeDetailsDTO serviceTypeDetailsDTO)
        {
            var result = new List<CarWashMinimalDTO>();

            foreach (var carWash in serviceType.CarWashesServiceTypes)
            {
                result.Add(new CarWashMinimalDTO()
                {
                    Id = carWash.CarWash.Id,
                    OpeningTime = carWash.CarWash.OpeningTime,
                    ClosingTime = carWash.CarWash.ClosingTime
                });
            }
            return result;
        }

        private List<CarWashMinimalDTO> MapOwner(ApplicationUser applicationUser, OwnerInfoDTO ownerInfoDTO)
        {
            var result = new List<CarWashMinimalDTO>();

            foreach (var carWash in applicationUser.CarWashes)
            {
                result.Add(new CarWashMinimalDTO()
                {
                    Id = carWash.Id,
                    OpeningTime = carWash.OpeningTime,
                    ClosingTime = carWash.ClosingTime
                });
            }
            return result;
        }

        private List<ServiceDTO> MapCustomer(ApplicationUser applicationUser, CustomerInfoDTO customerInfoDTO)
        {
            var result = new List<ServiceDTO>();

            foreach (var service in applicationUser.Services)
            {
                result.Add(new ServiceDTO()
                {
                    Id = service.Id,
                    CarWashId = (int)service.CarWashId,
                    CustomerId = service.CustomerId,
                    ServiceTypeId = service.ServiceTypeId,
                    ScheduledTime = service.ScheduledTime,
                    EligibleForCancelation = service.EligibleForCancelation,
                    Confirmed = service.Confirmed
                });
            }
            return result;

        }

    }
}
