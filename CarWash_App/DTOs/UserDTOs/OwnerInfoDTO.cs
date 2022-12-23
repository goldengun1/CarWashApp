using CarWash_App.DTOs.CarWashDTOs;

namespace CarWash_App.DTOs.UserDTOs
{
    public class OwnerInfoDTO : ApplicationUserDTO
    {
        public List<CarWashMinimalDTO> CarWashes { get; set; }
    }
}
