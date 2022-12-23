namespace CarWash_App.DTOs.CarWashDTOs
{
    public class CarWashStatsDTO
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string Profit { get; set; }
        public string ToBeCharged { get; set; }
        public int TotalScheduled { get; set; }
        public int CofirmedServices { get; set; }
        public string AverageProfitPerService { get; set; }

    }
}
