using System.ComponentModel.DataAnnotations;

namespace NEURAL.Models.Outlook
{
    public class OutlookModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Outlook Version")]
        public string? OutlookVersion { get; set; }

        [Required]
        [Display(Name = "Prodsched Version")]
        public string ProdschedVersion { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Actual Version")]
        public string ActualVersion { get; set; } = string.Empty;

        [Required]
        public OutlookStatus Status { get; set; }

        [Required]
        public string Author { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Reviewer")]
        public string? Reviewer { get; set; }

        [Display(Name = "Approval Due Date")]
        public DateTime? ApprovalDueDate { get; set; }

        [Display(Name = "Sent Date")]
        public DateTime? SentDate { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime? ApprovedDate { get; set; }

        [Display(Name = "Jobsite")]
        public string? Jobsite { get; set; }
    }

    public enum OutlookStatus
    {
        Draft = 1,
        Review = 2,
        Approved = 3
    }

    public class OutlookFilterModel
    {
        public string? ProdschedVersion { get; set; }
        public string? ActualVersion { get; set; }
        public string? Jobsite { get; set; }
    }

    public class KendoDataSourceRequestModel
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public string? SortField { get; set; }
        public string? SortDir { get; set; }
        public OutlookFilterModel? Filter { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class KendoDataSourceResultModel
    {
        public object Data { get; set; } = new object();
        public int Total { get; set; }
    }
}
