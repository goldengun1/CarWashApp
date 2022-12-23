namespace CarWash_App.Entities
{
    public class CarWashesServiceTypes
    {
        public int CarWashId { get; set; }
        public int ServiceTypeId { get; set; }

        public CarWash CarWash { get; set; }
        public ServiceType ServiceType { get; set; }
    }
}
