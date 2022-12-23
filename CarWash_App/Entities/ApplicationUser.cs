using Microsoft.AspNetCore.Identity;

namespace CarWash_App.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public override string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsAnOwner { get; set; }
        //TODO: add signup date
        public List<CarWash>? CarWashes { get; set; }
        public List<Service>? Services { get; set; }
    }
}
