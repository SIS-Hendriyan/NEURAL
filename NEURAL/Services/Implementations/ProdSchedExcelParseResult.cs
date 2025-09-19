using System.Data;

namespace NEURAL.Services.Implementations
{
    public sealed class ProdSchedExcelParseResult
    {
        public required DataTable Data { get; init; }
        public string? ExtractedYear { get; init; }
        public string? ExtractedMbrVersion { get; init; }

    }
}
