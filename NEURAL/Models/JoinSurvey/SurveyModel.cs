using System.ComponentModel.DataAnnotations;

namespace NEURAL.Models
{
    public class SurveyModel
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Month")]
        public string Month { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Area")]
        public string Area { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "PIT")]
        public string Pit { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Process")]
        public string Process { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Survey")]
        public string Survey { get; set; } = string.Empty;
        
        [Display(Name = "Volume")]
        public decimal Volume { get; set; }
        
        [Display(Name = "Horizontal")]
        public decimal Horizontal { get; set; }
        
        [Display(Name = "Vertical")]
        public decimal Vertical { get; set; }
        
        [Display(Name = "Weighted")]
        public decimal Weighted { get; set; }
        
        [Display(Name = "Status Email")]
        public bool StatusEmail { get; set; }
        
        [Required]
        [Display(Name = "Jobsite")]
        public string Jobsite { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
    }

    public class SurveyDataRequest
    {
        public int Take { get; set; } = 10;
        public int Skip { get; set; } = 0;
        public string? SortField { get; set; }
        public string? SortDir { get; set; }
        public SurveyFilterModel Filter { get; set; } = new();
    }

    public class SurveyDataResponse
    {
        public List<SurveyModel> Data { get; set; } = new();
        public int Total { get; set; }
    }

    public class SurveyFilterModel
    {
        public string Year { get; set; } = "all";
        public string Jobsite { get; set; } = "all";
    }
}