namespace Web.ViewModels;

public class PaginationInfoViewModel
{
    public int ActualPage { get; set; }
    public int ItemsPerPage { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
