namespace CarWash_App.DTOs.CarWashDTOs
{
    public class CarWashesFilter
    {
        public int Page { get; set; } = 1;
        public int RecordsPerPage { get; set; } = 10;
        public PaginationDTO Pagination
        {
            get { return new PaginationDTO() { Page = Page, RecordsPerPage = RecordsPerPage }; }
        }
        public bool CurrentlyOpen { get; set; }
        public string? ServiceTypeName { get; set; }
        public string? OrderingField { get; set; }
        public bool AscendingOrder { get; set; } = true;

    }
}
