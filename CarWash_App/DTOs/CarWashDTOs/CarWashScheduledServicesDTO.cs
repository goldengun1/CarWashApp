using CarWash_App.DTOs.ServiceDTOs;

namespace CarWash_App.DTOs.CarWashDTOs
{
    public class CarWashScheduledServicesDTO : CarWashDTO
    {
        public List<ServicesMinimalDTO> Services { get; set; }
    }
}
