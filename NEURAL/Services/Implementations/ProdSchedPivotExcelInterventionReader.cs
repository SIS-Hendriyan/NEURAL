using ClosedXML.Excel;
using NEURAL.Models.ViewModel;
using NEURAL.Utils;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using System.Globalization;

namespace NEURAL.Services.Implementations
{
    public class ProdSchedPivotExcelInterventionReader
    {

        private const int ParamCol = 2; // column B
        private const int DateRow = 2; // row with date headers
        private const int DateColStart = 5; // column E
        private const string ParamHeaderText = "Parameter";

        public static List<ProdSchedStagingInterventionViewModel> Read(
            Stream excel,
            string createdBy,
            long? defaultHeaderId = null,
            long? defaultJobsiteId = null,
            long? defaultProcessId = null,
            string? defaultMbrVersion = null)
        {
            using var recalced = RecalculateWorkbook(excel);
            using var wb = new XLWorkbook(recalced);
            var result = new List<ProdSchedStagingInterventionViewModel>();
            var nowUtc = DateTime.UtcNow;

            foreach (var ws in wb.Worksheets)
            {
                // ---- metadata 
                long headerId = TryGetNamedLong(ws, "HeaderId") ?? defaultHeaderId ?? 0;
                long jobsiteId = TryGetNamedLong(ws, "JobsiteId") ?? defaultJobsiteId ?? 0;
                long processId = TryGetNamedLong(ws, "ProcessId") ?? defaultProcessId ?? 0;
                string mbr = TryGetNamedString(ws, "MbrBasis") ?? defaultMbrVersion ?? "";

                //if (headerId == 0 || jobsiteId == 0 || processId == 0 || string.IsNullOrWhiteSpace(mbr))
                //{
                   
                //}

                // ---- read date header row 
                var dateMap = new List<(int Col, DateTime Day)>();
                for (int c = DateColStart; ; c++)
                {
                    var cell = ws.Cell(DateRow, c);
                    if (cell.IsEmpty()) break;

                    if (cell.TryGetValue<DateTime>(out var dt))
                    {
                        dateMap.Add((c, dt.Date));
                    }
                    else
                    {
                        var s = cell.GetString().Trim();
                        if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                                   DateTimeStyles.None, out dt))
                            dateMap.Add((c, dt.Date));
                        else
                            break; 
                    }
                }
                if (dateMap.Count == 0) continue;

             

                int paramHeaderRow = FindParameterHeaderRow(ws);
                if (paramHeaderRow < 0) continue;
                int startRow = paramHeaderRow + 1;
                int lastRow = ws.LastRowUsed()?.RowNumber() ?? startRow;


                for (int r = startRow; r <= lastRow; r++)
                {
                    string parameter = ws.Cell(r, ParamCol).GetString().Trim();
                    if (string.IsNullOrEmpty(parameter)) continue;            // skip blank label rows
                    if (parameter.Equals(ParamHeaderText, StringComparison.OrdinalIgnoreCase)) continue;

                    foreach (var (col, day) in dateMap)
                    {
                        var cell = ws.Cell(r, col);

                        string? value = GetCellAsString(cell);

                        result.Add(new ProdSchedStagingInterventionViewModel
                        {
                            ProdSchedHeaderId = headerId,
                            MbrVersion = mbr,
                            JobsiteId = jobsiteId,
                            ProcessId = processId,
                            DailyDate = day,
                            Parameter = parameter,
                            Value = value,     
                            CreatedAt = nowUtc,
                            CreatedBy = createdBy
                        });
                    }
                }
            }

            return result;
        }
        private static int FindParameterHeaderRow(IXLWorksheet ws)
        {
            var used = ws.RangeUsed();
            if (used == null) return -1;

            foreach (var cell in used.Cells())
            {
                if (cell.GetString().Trim().Equals(ParamHeaderText, StringComparison.OrdinalIgnoreCase))
                    return cell.Address.RowNumber;
            }
            return -1;
        }

        private static string? GetCellAsString(IXLCell cell)
        {
            if (cell.IsEmpty()) return null;

            
            if (cell.TryGetValue<decimal>(out var dec))
                return dec.ToString(CultureInfo.InvariantCulture);

            if (cell.TryGetValue<double>(out var dbl))
                return dbl.ToString(CultureInfo.InvariantCulture);

        
            if (cell.TryGetValue<DateTime>(out var dt))
                return dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            var s = cell.GetString();
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        private static long? TryGetNamedLong(IXLWorksheet ws, string name)
        {
            var nr = ws.Workbook.NamedRange(name) ?? ws.NamedRange(name);
            if (nr == null || nr.Ranges.Count == 0) return null;
            var cell = nr.Ranges.First().FirstCell();
            return cell.TryGetValue<long>(out var v) ? v : (long?)null;
        }

        private static string? TryGetNamedString(IXLWorksheet ws, string name)
        {
            var nr = ws.Workbook.NamedRange(name) ?? ws.NamedRange(name);
            if (nr == null || nr.Ranges.Count == 0) return null;
            var s = nr.Ranges.First().FirstCell().GetString();
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }

        private static Stream RecalculateWorkbook(Stream excelInput)
        {
           // NpoiFormulaBootstrap.EnsureRegistered();
            excelInput.Position = 0;

            FunctionEval.RegisterFunction("N", new NFunctionImpl());

            IWorkbook wb = WorkbookFactory.Create(excelInput);
            var evaluator = wb.GetCreationHelper().CreateFormulaEvaluator();
            evaluator.EvaluateAll();

            var ms = new MemoryStream();
            wb.Write(ms, leaveOpen: true);
            ms.Position = 0;
            return ms;
        }
    }
}
