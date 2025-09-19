namespace NEURAL.Models.ViewModel
{
    public class ProdSchedInterventionDownloadViewModel
    {
        public long Id { get; set; }
        public long ProdSchedHeaderId { get; set; }
        public long JobsiteId { get; set; }
        public string Jobsite { get; set; } = string.Empty;
        public DateTime DailyDate { get; set; }
        public long ProcessId { get; set; }
        public string Process { get; set; } = string.Empty;
        public string Parameter { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}
