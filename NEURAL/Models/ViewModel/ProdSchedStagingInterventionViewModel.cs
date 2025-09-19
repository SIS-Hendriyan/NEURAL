namespace NEURAL.Models.ViewModel
{
    public sealed class ProdSchedStagingInterventionViewModel
    {
        public long ProdSchedHeaderId { get; set; }    
        public string MbrVersion { get; set; } = ""; 
        public long JobsiteId { get; set; }      
        public long ProcessId { get; set; }     
        public DateTime DailyDate { get; set; }      
        public string Parameter { get; set; } = ""; 
        public string? Value { get; set; }       
        public DateTime CreatedAt { get; set; }    
        public string CreatedBy { get; set; } = "";
    }
}
