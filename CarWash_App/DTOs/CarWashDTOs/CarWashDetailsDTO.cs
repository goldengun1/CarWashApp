using CarWash_App.DTOs.ServiceDTOs;
using CarWash_App.DTOs.ServiceTypeDTOs;

namespace CarWash_App.DTOs.CarWashDTOs
{
    public class CarWashDetailsDTO : CarWashDTO
    {
        public List<ServiceDTO> ScheduledServices { get; set; }
        public List<ServiceTypeDTO> OfferedServices { get; set; }
    }
}
