namespace CarWash_App.Entities  
{
    public class CarWash
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string CarWashName { get; set; }
        public int OpeningTime { get; set; }
        public int ClosingTime { get; set; }
        public float Profit { get; set; } = 0f;

        public ApplicationUser Owner { get; set; }
        public List<Service> Services { get; set; }
        public List<CarWashesServiceTypes> CarWashesServiceTypes { get; set; }
    }
}
