using System.ComponentModel.DataAnnotations;

namespace NEURAL.Models.ProductionActual
{
    public class ProductionActualModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Jobsite { get; set; } = string.Empty;

        [Required]
        public string Year { get; set; } = string.Empty;

        [Required]
        public string VersionName { get; set; } = string.Empty;

        public Progress Progress { get; set; } = new Progress();
    }

    public class Progress
    {
        public ProgressStatus DailyFleet { get; set; } = ProgressStatus.pending;
        public ProgressStatus MonthlyFleet { get; set; } = ProgressStatus.pending;
        public ProgressStatus DailyProcess { get; set; } = ProgressStatus.pending;
        public ProgressStatus MonthlyProcess { get; set; } = ProgressStatus.pending;
        public ProgressStatus YearlyProcess { get; set; } = ProgressStatus.pending;
        public ProgressStatus YearlyFleet { get; set; } = ProgressStatus.pending;
        public ProgressStatus Success { get; set; } = ProgressStatus.pending;
        public ProgressStage CurrentStage { get; set; } = ProgressStage.DailyFleet;
        public Dictionary<string, string>? ErrorLogs { get; set; }
    }

    public enum ProgressStatus
    {
        pending,
        running,
        completed,
        error
    }

    public enum ProgressStage
    {
        DailyFleet,
        MonthlyFleet,
        DailyProcess,
        MonthlyProcess,
        YearlyProcess,
        YearlyFleet,
        Success
    }

    public class FilterOptions
    {
        public string Year { get; set; } = "all";
        public string Jobsite { get; set; } = "all";
    }
}
