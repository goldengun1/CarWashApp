using System.ComponentModel.DataAnnotations;

namespace CarWash_App.DTOs.ServiceDTOs
{
    public class ServiceCreationDTO
    {
        public int Id { get; set; }

        [Required]
        public int CarWashId { get; set; }

        [Required]
        public int ServiceTypeId { get; set; }

        [Required]
        public DateTime ScheduledTime { get; set; }
    }
}
