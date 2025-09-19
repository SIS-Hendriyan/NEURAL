namespace NEURAL.Models.ViewModel
{
    public class ProdSchedInterventionMonthStatViewModel
    {
        public long JobsiteId { get; set; }
        public string Jobsite { get; set; } = string.Empty;
        public int TotalDays { get; set; }
        public int TotalAvailableDays { get; set; }
        public int TotalFridays { get; set; }
        public decimal TotalHolidays { get; set; }
        public string DayInHoliday { get; set; } = string.Empty;

    }
}
