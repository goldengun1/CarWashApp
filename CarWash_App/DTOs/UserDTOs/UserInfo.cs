using CarWash_App.Validations;
using System.ComponentModel.DataAnnotations;

namespace CarWash_App.DTOs.UserDTOs
{
    public class UserInfo : UserSignInInfo
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [FirstLetterUppercase]
        public string FirstName { get; set; }

        [Required]
        [FirstLetterUppercase]
        public string LastName { get; set; }

        [Required]
        public bool IsOwner { get; set; }

    }
}
