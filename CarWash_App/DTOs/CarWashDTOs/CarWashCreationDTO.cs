using CarWash_App.Helpers;
using CarWash_App.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CarWash_App.DTOs.CarWashDTOs
{
    public class CarWashCreationDTO : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        [FirstLetterUppercase]
        public string CarWashName { get; set; }

        [Required]
        [Range(0, 24)]
        public int OpeningTime { get; set; }

        [Required]
        [Range(0, 24)]
        public int ClosingTime { get; set; }


        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int>? ServiceTypeIds { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ClosingTime < OpeningTime || ClosingTime - OpeningTime < 5)
                yield return new ValidationResult("Closing time must be after opening time at least 5 hours.");
        }
    }
}
