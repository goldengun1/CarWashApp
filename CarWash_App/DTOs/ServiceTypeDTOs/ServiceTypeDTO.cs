namespace CarWash_App.DTOs.ServiceTypeDTOs
{
    public class ServiceTypeDTO
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public TimeSpan Duration { get; set; }
        public string Cost { get; set; }
    }
}
