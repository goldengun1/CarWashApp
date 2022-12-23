using CarWash_App.DTOs.CarWashDTOs;

namespace CarWash_App.DTOs.ServiceTypeDTOs
{
    public class ServiceTypeDetailsDTO : ServiceTypeDTO
    {
        public List<CarWashMinimalDTO> CarWashes { get; set; }
    }
}
