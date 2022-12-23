namespace CarWash_App.Entities
{
    public class ServiceType
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public TimeSpan Duration { get; set; }
        public float Cost { get; set; }

        public List<Service> Services { get; set; }
        public List<CarWashesServiceTypes> CarWashesServiceTypes { get; set; }
    }
}
