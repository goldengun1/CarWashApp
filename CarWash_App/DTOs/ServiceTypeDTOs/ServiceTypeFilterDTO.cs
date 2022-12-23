namespace CarWash_App.DTOs.ServiceTypeDTOs
{
    public class ServiceTypeFilterDTO
    {
        public int Page { get; set; } = 1;
        public int RecordsPerPage { get; set; } = 10;
        public PaginationDTO Pagination
        {
            get { return new PaginationDTO() { Page = Page, RecordsPerPage = RecordsPerPage }; }
        }
        public int MaxDuration { get; set; } = 0;
        public float MaxCost { get; set; } = 0;
        public bool AvailableAnywhere { get; set; } = true;
        public string? OrderingField { get; set; }
        public bool AscendingOrder { get; set; } = true;
    }

}
