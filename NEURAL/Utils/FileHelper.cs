namespace NEURAL.Utils
{
    public class FileHelper
    {
        public static string GetContentType(string? fileName)
        {
            var ext = Path.GetExtension(fileName ?? string.Empty).ToLowerInvariant();
            return ext switch
            {
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" => "application/vnd.ms-excel",
                ".pdf" => "application/pdf",
                ".csv" => "text/csv",
                _ => "application/octet-stream"
            };
        }
    }
}
