namespace NEURAL.Models.ViewModel
{
    public class ProdSchedPivotViewModel
    {
        public long ProdschedHeaderId { get; set; }
        public string Version { get; set; } = string.Empty;
        public string Jobsite { get; set; } = string.Empty;
        public string? Area { get; set; }
        public string? Pit { get; set; }
        public string PeriodKey { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public int? SettingId { get; set; }
        public string Process { get; set; } = string.Empty;
        public string? Fleet { get; set; }
        public string Parameter { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}
