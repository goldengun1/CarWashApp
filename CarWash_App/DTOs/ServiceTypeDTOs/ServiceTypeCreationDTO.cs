using CarWash_App.Helpers;
using CarWash_App.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CarWash_App.DTOs.ServiceTypeDTOs
{
    public class ServiceTypeCreationDTO
    {
        [Required]
        [FirstLetterUppercase]
        public string ServiceName { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public float Cost { get; set; }


        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int>? CarWashIds { get; set; }
    }
}
