namespace NEURAL.Utils
{
    public class ProdSchedHelper
    {
        public static string ComputeStatus(List<string>? progress)
        {
            if (progress == null || progress.Count == 0)
                return "PROCESS";

            var items = progress
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();

            if (items.Any(s => s.Equals("ERROR", StringComparison.OrdinalIgnoreCase)))
                return "ERROR";

            var okCount = items.Count(s => s.Equals("OK", StringComparison.OrdinalIgnoreCase));

            if (okCount >= 6) return "SUCCESS";
            return "PROCESS";
        }
    }
}
