namespace CarWash_App.DTOs.ServiceDTOs
{
    public class ServicesMinimalDTO
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public int CarWashId { get; set; }
        public int ServiceTypeId { get; set; }
        public string ServiceTypeName { get; set; }
        public DateTime ScheduledTime { get; set; }
    }
}
