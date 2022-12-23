namespace CarWash_App.DTOs.ServiceDTOs
{
    public class ServicePatchDTO
    {
        public int CarWashId { get; set; }
        public int ServiceTypeId { get; set; }
        public DateTime ScheduledTime { get; set; }
    }
}
