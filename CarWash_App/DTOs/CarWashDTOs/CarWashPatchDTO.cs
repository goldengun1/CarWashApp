using System.ComponentModel.DataAnnotations;

namespace CarWash_App.DTOs.CarWashDTOs
{
    public class CarWashPatchDTO
    {
        [Required]
        [Range(0, 24)]
        public int OpeningTime { get; set; }

        [Required]
        [Range(0, 24)]
        public int ClosingTime { get; set; }
    }
}
