using CarWash_App.DTOs.ServiceDTOs;

namespace CarWash_App.DTOs.UserDTOs
{
    public class CustomerInfoDTO : ApplicationUserDTO
    {
        public List<ServiceDTO> Services { get; set; }
    }
}
