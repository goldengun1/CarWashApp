namespace CarWash_App.DTOs.ServiceDTOs
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        public int CarWashId { get; set; }
        public string CustomerId { get; set; }
        public int ServiceTypeId { get; set; }
        public DateTime ScheduledTime { get; set; }
        public bool EligibleForCancelation { get; set; }
        public bool Confirmed { get; set; }
    }
}
