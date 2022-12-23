using System.ComponentModel.DataAnnotations;

namespace CarWash_App.Entities
{
    public class Service
    {
        public int Id { get; set; }
        public int? CarWashId { get; set; }

        [StringLength(450)]
        public string CustomerId { get; set; }
        public int ServiceTypeId { get; set; }
        public DateTime ScheduledTime { get; set; }
        public bool EligibleForCancelation { get; set; } = true;
        public bool Confirmed { get; set; } = false;
        public bool PaymentCollected { get; set; } = false;

        public ApplicationUser Customer { get; set; }
        public CarWash? CarWash { get; set; }
        public ServiceType ServiceType { get; set; }
       

    }
}
