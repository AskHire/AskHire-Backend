namespace AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs
{
    public class PaginationQuery
    {
        public int Page { get; set; } = 1;               // default to page 1
        public int PageSize { get; set; } = 10;          // default page size
        public string? SortBy { get; set; }              // e.g. "JobTitle"
        public bool IsDescending { get; set; } = false;  // true = DESC, false = ASC
        public string? SearchTerm { get; set; }          // optional keyword

        public string? StatusFilter { get; set; } // e.g., "Open", "Expired"
    }
}