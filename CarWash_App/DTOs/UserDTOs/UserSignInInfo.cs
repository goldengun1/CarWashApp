using System.ComponentModel.DataAnnotations;

namespace CarWash_App.DTOs.UserDTOs
{
    public class UserSignInInfo
    {
        [Required]
        [MinLength(6)]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
