namespace NEURAL.Models.ViewModel
{
    public class ProdSchedHeaderViewModel
    {
        public long Id { get; set; }
        public string MbrVersion { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public string VersionName { get; set; } = string.Empty;
        public List<string> Progress { get; set; } = new List<string>();
        public string Status { get; set; } = string.Empty;
    }
}
