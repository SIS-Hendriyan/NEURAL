using System.ComponentModel.DataAnnotations;

namespace NEURAL.Models.DPR
{
    public class DPRData
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Jobsite { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty; // "Quick" or "Fix"

        public Progress Progress { get; set; } = new Progress();
    }

    public class Progress
    {
        public string GenerateDataDP { get; set; } = "pending"; // pending, running, completed, error
        public string GenerateDataDF { get; set; } = "pending";
        public string GenerateReport { get; set; } = "pending";
        public string Success { get; set; } = "pending";
        public string SendEmail { get; set; } = "pending";
        public string CurrentStage { get; set; } = "generate_data";
        public ErrorLogs? ErrorLogs { get; set; }
    }

    public class ErrorLogs
    {
        public string? GenerateDataDP { get; set; }
        public string? GenerateDataDF { get; set; }
        public string? GenerateReport { get; set; }
    }

    public class FilterOptions
    {
        public string Month { get; set; } = "all";
        public string Jobsite { get; set; } = "all";
    }

    public class EmailSetting
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Jobsite { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string EmailTo { get; set; } = string.Empty;

        public string EmailCC { get; set; } = string.Empty;
    }

    // DPR Form Data Types
    public class ProductionData
    {
        public string Id { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pit { get; set; } = string.Empty;
        public string Process { get; set; } = string.Empty;
        public double Production { get; set; }
        public double WorkingHours { get; set; }
    }

    public class MiningData
    {
        public string Id { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pit { get; set; } = string.Empty;
        public string Process { get; set; } = string.Empty;
        public double S1 { get; set; }
        public double S2 { get; set; }
        public double S3 { get; set; }
        public double S4 { get; set; }
        public double S5 { get; set; }
        public double S6 { get; set; }
        public double S7 { get; set; }
        public double S8 { get; set; }
        public double S9 { get; set; }
        public double S10 { get; set; }
        public double S11 { get; set; }
        public double S12 { get; set; }
        public double S13 { get; set; }
        public double S14 { get; set; }
        public double S15 { get; set; }
        public double S16 { get; set; }
        public double S17 { get; set; }
        public double S18 { get; set; }
        public double S19 { get; set; }
        public double S20 { get; set; }
        public double RC { get; set; }
        public double BD { get; set; }
        public double Distance { get; set; }
        public double TravelSpeed { get; set; }
        public double Payload { get; set; }
        public double TripPerDay { get; set; }
        public double CycleTime { get; set; }
    }

    public class TransportData
    {
        public string Id { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pit { get; set; } = string.Empty;
        public string Process { get; set; } = string.Empty;
        public double S1 { get; set; }
        public double S2 { get; set; }
        public double S3 { get; set; }
        public double S4 { get; set; }
        public double S5 { get; set; }
        public double S6 { get; set; }
        public double S7 { get; set; }
        public double S8 { get; set; }
        public double S9 { get; set; }
        public double S10 { get; set; }
        public double S11 { get; set; }
        public double S12 { get; set; }
        public double S13 { get; set; }
        public double S14 { get; set; }
        public double S15 { get; set; }
        public double S16 { get; set; }
        public double S17 { get; set; }
        public double S18 { get; set; }
        public double S19 { get; set; }
        public double S20 { get; set; }
        public double RC { get; set; }
        public double BD { get; set; }
        public double Distance { get; set; }
        public double TravelSpeed { get; set; }
        public double Payload { get; set; }
        public double TripPerDay { get; set; }
        public double CycleTime { get; set; }
        public double LoadingTime { get; set; }
        public double TravelTime { get; set; }
        public double WeightBridge { get; set; }
        public double ManuverTime { get; set; }
    }

    public class StockpileData
    {
        public string Id { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pit { get; set; } = string.Empty;
        public string Process { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string StockpileName { get; set; } = string.Empty;
        public double Stock { get; set; }
        public double Capacity { get; set; }
    }

    public class GlobalWeatherData
    {
        public string Id { get; set; } = string.Empty;
        public double RainHours { get; set; }
        public double RainfallPlan { get; set; }
        public double RainfallActual { get; set; }
        public double Frequency { get; set; }
        public double SlipperyTime { get; set; }
    }

    public class DPRFormData
    {
        [Required]
        public string Jobsite { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public List<ProductionData> Production { get; set; } = new List<ProductionData>();
        public List<MiningData> Mining { get; set; } = new List<MiningData>();
        public List<TransportData> Transport { get; set; } = new List<TransportData>();
        public List<StockpileData> Stockpile { get; set; } = new List<StockpileData>();
        public List<GlobalWeatherData> GlobalWeather { get; set; } = new List<GlobalWeatherData>();
    }

    // Master DPR Types
    public class MasterProductionData
    {
        public string Id { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pit { get; set; } = string.Empty;
        public string Process { get; set; } = string.Empty;
    }

    public class MasterMiningData
    {
        public string Id { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pit { get; set; } = string.Empty;
        public string Process { get; set; } = string.Empty;
    }

    public class MasterTransportData
    {
        public string Id { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pit { get; set; } = string.Empty;
        public string Process { get; set; } = string.Empty;
    }

    public class MasterStockpileData
    {
        public string Id { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pit { get; set; } = string.Empty;
        public string Process { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string StockpileName { get; set; } = string.Empty;
    }

    public class MasterGlobalWeatherData
    {
        public string Id { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string WeatherStation { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    // Report Types
    public class ReportDataPoint
    {
        public double Actual { get; set; }
        public double Plan { get; set; }
        public double Achievement { get; set; }
        public string Status { get; set; } = string.Empty; // good, warning, critical
    }

    public class ReportDataPointWithDeviation : ReportDataPoint
    {
        public double Deviation { get; set; }
    }

    public class ReportActivity
    {
        public string Id { get; set; } = string.Empty;
        public string Activity { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public ReportDataPoint Daily { get; set; } = new ReportDataPoint();
        public ReportDataPoint MTD { get; set; } = new ReportDataPoint();
        public ReportDataPointWithDeviation YTD { get; set; } = new ReportDataPointWithDeviation();
        public string Category { get; set; } = string.Empty; // mining, distance, weather, other
    }

    public class ReportData
    {
        public string Id { get; set; } = string.Empty;
        public string Jobsite { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string MBRVersion { get; set; } = string.Empty;
        public string EstimatedTruckCount { get; set; } = string.Empty;
        public int ElapsedDays { get; set; }
        public int RemainingDays { get; set; }
        public List<ReportActivity> Activities { get; set; } = new List<ReportActivity>();
    }
}
