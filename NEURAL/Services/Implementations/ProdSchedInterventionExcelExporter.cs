using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using NEURAL.Models.ViewModel;
using NEURAL.Services.Interfaces;
using System.Globalization;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace NEURAL.Services.Implementations
{
    public sealed class ProdSchedInterventionExcelExporter : IProdSchedInterventionExcelExporter
    {

        // ===== layout constants =====
        int dataImportCol = 0; // initiate later
        const int headerIdRow = 1;
        const int jobsiteIdRow = 2;
        const int jobsiteNameRow = 3;
        const int processIdRow = 4;
        const int processNameRow = 5;
        const int mbrBasisRow = 6; // row for MBR basis
        private const int DateRow = 2;            // row for date headers
        private const int DateColStart = 5;       // E
        int DateColEnd = 0;  // initiate later
        private const int ParamHeaderRow = 8;     // "Parameter"
        private const int ParamCol = 2;           // B
        private const int ParamCol3 = 2; //C
        private const int FirstDataRow = 13;
        private const int MStatRow = 3;
        private const int MStatColStart = 2;      // B
        private const int MStatColValueStart = 3; // C
        private const int HolidayRow = MStatRow + 3;      // 6
        private const int ShiftRow = MStatRow + 4;        // 7
        private const int AvailableDayRow = MStatRow + 1; // 4
        private const int EWHRow = 11;
        private const int MOHHRow = 10;
        private const int PDTYRow = 36;
        private const int ProdRow = 37; // Production row
        private const int NByModelRow = 40;
        

        // ==== formula
        const string ShiftFormula = "IF(R[-1]C<0.5,2,IF(ABS(R[-1]C-1)<1E-9,0,1))";   // If (Holiday < 0.5) {2 } else {If (Holiday = 1) {0} else {1}}
        const string AvailableDayFormula = "IF(R[2]C=0,1,1-R[2]C)";                          //If (Holiday = 0) {1} else {1 - Holiday}
        const string EwhFormula = "24*N(R[-7]C) - SUM(R[2]C:R[21]C, R[22]C, R[23]C)"; // 24*AvailDay - (S1..S20 + BD + Reloc)
        const string MohhFormula = "SUM(R[1]C, R[3]C:R[22]C, R[23]C, R[24]C)";       // EWH + (S1..S20 + BD + Reloc)
        const string PdtyFormula = "R[1]C/R[-25]C"; // PROD / EWH

        private static readonly HashSet<string> AllowParams = new(StringComparer.OrdinalIgnoreCase)
        {
        "S1","S2","S3","S4","S5","S6","S7","S8","S9","S10","S11","S12","S13","S14","S15","S16","S17","S18","S19","S20",
        "Relocation","BREAKDOWN","PDTY","PRODUCTION"
        };

        public record MonthlySumFormula(string Parameter, string Formula, int Row, int Col);

        public async Task<byte[]> BuildFromData(List<ProdSchedInterventionDownloadViewModel> rows, List<ProdSchedInterventionMonthStatViewModel> monthStats, int month, string mbrVersion, CancellationToken ct)
        {
            if (rows.Count == 0) return Array.Empty<byte>();

            var year = rows.Select(r => r.DailyDate.Year).Distinct().DefaultIfEmpty(DateTime.UtcNow.Year).First();
            var monthStr = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            var start = new DateTime(year, month, 1);
            var end = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            var dates = Enumerable.Range(0, (end - start).Days + 1).Select(i => start.AddDays(i).Date).ToList();
            var monthlySumFormulas = new List<MonthlySumFormula>();

            using var wb = new XLWorkbook();
            var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var sheets = rows.GroupBy(r => new { r.JobsiteId, r.Jobsite, r.ProcessId, r.Process })
                             .OrderBy(g => g.Key.Jobsite)
                             .ThenBy(g => g.Key.Process);

            foreach (var g in sheets)
            {
                var ws = wb.Worksheets.Add(UniqueName(MakeSheetName($"{g.Key.Jobsite}-{g.Key.Process}"), usedNames));

                // meta
                ws.Cell(1, 1).Value = g.Key.Jobsite;
                ws.Cell(1, 2).Value = mbrVersion;

                // month stat block (labels)
                ws.Cell(MStatRow, MStatColStart).Value = "Jumlah Hari";
                ws.Cell(AvailableDayRow, MStatColStart).Value = "Available Days";
                ws.Cell(MStatRow + 2, MStatColStart).Value = "Jumlah Hari Jumat";
                ws.Cell(HolidayRow, MStatColStart).Value = "Holiday";
                ws.Cell(ShiftRow, MStatColStart).Value = "Jumlah Shift";

                // month stat values
                var mStat = monthStats.FirstOrDefault(x => x.JobsiteId == g.Key.JobsiteId);
                ws.Cell(MStatRow, MStatColValueStart).Value = mStat?.TotalDays ?? 0;
                ws.Cell(AvailableDayRow, MStatColValueStart).Value = mStat?.TotalAvailableDays ?? 0;
                ws.Cell(MStatRow + 2, MStatColValueStart).Value = mStat?.TotalFridays ?? 0;
                ws.Cell(HolidayRow, MStatColValueStart).Value = mStat?.TotalHolidays ?? 0;

                // holiday csv -> set of day numbers
                var holidayDays = ParseDaysCsv(mStat?.DayInHoliday, dates[^1].Day);

                // write dates & formula rows
                int c = DateColStart;
                DateColEnd = DateColStart + dates.Count - 1;
                int MonthlyCol = DateColEnd + 2;
                int offToFirst = MonthlyCol - DateColStart;
                int offToLast = MonthlyCol - DateColEnd;
                string sumR1C1 = $"=SUM(RC[-{offToFirst}]:RC[-{offToLast}])";
                foreach (var d in dates)
                {
                    int day = d.Day;
                    ws.Cell(DateRow, c).Value = d.ToString("yyyy-MM-dd");
                    ws.Cell(HolidayRow, c).Value = holidayDays.Contains(day) ? 1 : 0;
                    ws.Cell(ShiftRow, c).FormulaR1C1 = ShiftFormula;
                    ws.Cell(AvailableDayRow, c).FormulaR1C1 = AvailableDayFormula; 
                    ws.Cell(EWHRow, c).FormulaR1C1 = EwhFormula;
                    ws.Cell(MOHHRow, c).FormulaR1C1 = MohhFormula;
                    ws.Cell(PDTYRow, c).FormulaR1C1 = PdtyFormula;

                    // monthly sum formulas

                    monthlySumFormulas.Add(new MonthlySumFormula("EWH", sumR1C1, EWHRow, MonthlyCol));
                    monthlySumFormulas.Add(new MonthlySumFormula("MOHH", sumR1C1, MOHHRow, MonthlyCol));
                    monthlySumFormulas.Add(new MonthlySumFormula("PDTY", sumR1C1, PDTYRow, MonthlyCol));
                    monthlySumFormulas.Add(new MonthlySumFormula("Shift", sumR1C1, ShiftRow, MonthlyCol));
                    c++;
                }
                ws.Row(DateRow).Style.Font.Bold = true;


                // headers / captions
                ws.Cell(ParamHeaderRow, ParamCol).Value = "Parameter";
                ws.Row(ParamHeaderRow).Style.Font.Bold = true;
                ws.Range(ShiftRow, DateColStart, ShiftRow, DateColEnd).Style.NumberFormat.Format = "0";
                ws.Cell(MOHHRow, ParamCol).Value = "MOHH"; ws.Row(MOHHRow).Style.Font.Bold = true;
                ws.Cell(EWHRow, ParamCol).Value = "WORKHOURS"; ws.Row(EWHRow).Style.Font.Bold = true;
                ws.Cell(PDTYRow, ParamCol).Value = "Pdty";

                // parameters table from daily process
                int r = FirstDataRow;
                var paramGroupsDP = g.Where(s => AllowParams.Contains(s.Parameter))
                                   .GroupBy(x => x.Parameter, StringComparer.OrdinalIgnoreCase)
                                   .OrderBy(x => GetParameterOrder(x.Key));

                foreach (var pg in paramGroupsDP)
                {
                    var isProduction = pg.Key.Equals("Production", StringComparison.OrdinalIgnoreCase);
                    var label = pg.Key.Equals("PDTY", StringComparison.OrdinalIgnoreCase) ? "PDTY Awal" : pg.Key;

                    //special case for production, adding space for PDTY
                    if (isProduction) r++;

                    ws.Cell(r, ParamCol).Value = label;

                    // day -> sum
                    var map = new Dictionary<DateTime, decimal>();
                    foreach (var item in pg)
                    {
                        var day = item.DailyDate.Date;
                        map[day] = map.TryGetValue(day, out var v) ? v + item.Value : item.Value;
                    }

                    c = DateColStart;
                    foreach (var d in dates)
                        ws.Cell(r, c++).Value = map.TryGetValue(d, out var v) ? v : 0m;

                    monthlySumFormulas.Add(new MonthlySumFormula(label, sumR1C1, r, MonthlyCol));

                    r++;
                }


                // parameter only for N by model
                var nByRowByModel = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                var pdtyNbyAwalRowByModel = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                var paramNbyModelDP = g.Where(s => !string.IsNullOrWhiteSpace(s.Parameter) && s.Parameter.Trim().StartsWith("N by", StringComparison.OrdinalIgnoreCase))
                                        .GroupBy(x => x.Parameter.Trim(), StringComparer.OrdinalIgnoreCase)
                                        .OrderBy(x => x.Key);

                var rNby = NByModelRow;
                foreach (var pg in paramNbyModelDP)
                {
                    var model = ModelFromNBy(pg.Key);
                    nByRowByModel[model] = rNby;

                    ws.Cell(rNby, ParamCol3).Value = pg.Key;

                    // day -> sum
                    var map = new Dictionary<DateTime, decimal>();
                    foreach (var item in pg)
                    {
                        var day = item.DailyDate.Date;
                        map[day] = map.TryGetValue(day, out var v) ? v + item.Value : item.Value;
                    }

                    c = DateColStart;
                    foreach (var d in dates)
                        ws.Cell(rNby, c++).Value = map.TryGetValue(d, out var v) ? v : 0m;

                    // monthly sum formulas

                    monthlySumFormulas.Add(new MonthlySumFormula("NByModel", sumR1C1, rNby, MonthlyCol));

                    rNby++;
                }

                // parameter N Pdty, Production (Check Point)
                int nByStartRow = NByModelRow;      // 40
                int nByLastRow = rNby - 1;         // last row "N by"
                int nPdtyRow = Math.Max(nByStartRow, nByLastRow + 1);
                int prodCPRow = nPdtyRow + 1;

                ws.Cell(nPdtyRow, ParamCol3).Value = "N Pdty";
                ws.Cell(prodCPRow, ParamCol3).Value = "Production (Check Point)";
              
                int firstColNpdty = DateColStart;
                int lastColNpdty = DateColEnd;

                for (int col = firstColNpdty; col <= lastColNpdty; col++)
                {
                    if (nByLastRow < nByStartRow) // no "N by" rows
                    {
                        ws.Cell(nPdtyRow, col).FormulaR1C1 = "0";
                        ws.Cell(prodCPRow, col).FormulaR1C1 = "0";
                    }
                    else
                    {
                        ws.Cell(nPdtyRow, col).FormulaR1C1 = $"IF(SUM(R{nByStartRow}C:R{nByLastRow}C)=0,0,R36C/SUM(R{nByStartRow}C:R{nByLastRow}C))";
                        ws.Cell(prodCPRow, col).FormulaR1C1 = $"SUM(R{nByStartRow}C:R{nByLastRow}C)*R{EWHRow}C*R[-1]C";
                    }

                    // monthly sum formulas

                    monthlySumFormulas.Add(new MonthlySumFormula("NPdty", sumR1C1, nPdtyRow, MonthlyCol));
                    monthlySumFormulas.Add(new MonthlySumFormula("ProdCP", sumR1C1, prodCPRow, MonthlyCol));
                }

                // parameter only for PDTY by model awal
                var paramPdtyByModelDF = g.Where(s => !string.IsNullOrWhiteSpace(s.Parameter) &&
                                         s.Parameter.IndexOf("Pdty by", StringComparison.OrdinalIgnoreCase) >= 0 &&
                                         s.Parameter.EndsWith("Awal", StringComparison.OrdinalIgnoreCase))
                                        .GroupBy(x => x.Parameter.Trim(), StringComparer.OrdinalIgnoreCase)
                                        .OrderBy(x => x.Key).ToList();

                var rPdtyByModelAwal = prodCPRow+1;
                foreach (var pg in paramPdtyByModelDF)
                {
                    var model = ModelFromPdtyAwal(pg.Key);
                    pdtyNbyAwalRowByModel[model] = rPdtyByModelAwal;

                    ws.Cell(rPdtyByModelAwal, ParamCol3).Value = pg.Key;

                    // day -> sum
                    var map = new Dictionary<DateTime, decimal>();
                    foreach (var item in pg)
                    {
                        var day = item.DailyDate.Date;
                        map[day] = map.TryGetValue(day, out var v) ? v + item.Value : item.Value;
                    }

                    c = DateColStart;
                    foreach (var d in dates)
                        ws.Cell(rPdtyByModelAwal, c++).Value = map.TryGetValue(d, out var v) ? v : 0m;

                    // monthly sum formulas

                    monthlySumFormulas.Add(new MonthlySumFormula("NPdtyByModelAwal", sumR1C1, rPdtyByModelAwal, MonthlyCol));

                    rPdtyByModelAwal++;
                }


                // parameter PDTY by model
                int pdtyByModelStartRow = rPdtyByModelAwal;
                int pdtyByModelLastRow = 0;
                ws.Cell(pdtyByModelStartRow, ParamCol).Value = ""; 

                for (int idx = 0; idx < paramPdtyByModelDF.Count; idx++)
                {
                    var awalKey = paramPdtyByModelDF[idx].Key;
                    var model = ModelFromPdtyAwal(awalKey);
                    var hasNBy = nByRowByModel.TryGetValue(model, out int nByRow);
                    var hasAwal = pdtyNbyAwalRowByModel.TryGetValue(model, out int awalRow);

                    pdtyByModelLastRow = pdtyByModelStartRow + idx;

                    // label
                    ws.Cell(pdtyByModelLastRow, ParamCol3).Value = $"Pdty by {model}";

                    // Pdty / PdtyAwal * (PdtyByModelAwal / NByModel)
                    string f;
                    if (hasNBy && hasAwal)
                    {
                        // if PdtyAwal or NByModel is 0 -> 0
                        f = $"IF(OR(R{PDTYRow + 1}C=0,R{nByRow}C=0),0, R{PDTYRow}C/R{PDTYRow+1}C * R{awalRow}C/R{nByRow}C)";
                    }
                    else
                    {
                        f = "0";
                    }

              
                    int firstpdtyByModelCol = DateColStart;
                    int lastpdtyByModelCol = DateColEnd;
                    for (int col = firstpdtyByModelCol; col <= lastpdtyByModelCol; col++)
                        ws.Cell(pdtyByModelLastRow, col).FormulaR1C1 = f;

                    ws.Range(pdtyByModelLastRow, firstpdtyByModelCol, pdtyByModelLastRow, lastpdtyByModelCol).Style.NumberFormat.Format = "0.00";

                    // monthly sum formulas

                    monthlySumFormulas.Add(new MonthlySumFormula("PdtyByModel", sumR1C1, pdtyByModelLastRow, MonthlyCol));
                }

                // parameter total PDTY all model
                int pdtyAllModelRow = pdtyByModelLastRow + 1;
                ws.Cell(pdtyAllModelRow, ParamCol3).Value = "Total Pdty All Model";

                int nbyStartPdtyAllModel = NByModelRow;
                int nbyLastPdtyAllModel = rNby - 1;

                int pdtyStartPdtyAllModel = pdtyByModelStartRow;
                int pdtyLastPdtyAllModel = pdtyByModelLastRow;

                int nbyCountPdtyAllModel = Math.Max(0, nbyLastPdtyAllModel - nbyStartPdtyAllModel + 1);
                int pdtyCountPdtyAllModel = Math.Max(0, pdtyLastPdtyAllModel - pdtyStartPdtyAllModel + 1);
                int pairCountPdtyAllModel = Math.Min(nbyCountPdtyAllModel, pdtyCountPdtyAllModel);

                int effPdtyLastPdtyAllModel = pdtyStartPdtyAllModel + pairCountPdtyAllModel - 1;
                int effNbyLastPdtyAllModel = nbyStartPdtyAllModel + pairCountPdtyAllModel - 1;

                int totalPdtyAllRowPdtyAllModel = pdtyLastPdtyAllModel + 1; 
                int paramColPdtyAllModel = ParamCol3;                

                int firstColPdtyAllModel = DateColStart;
                int lastColPdtyAllModel = DateColEnd;

                if (pairCountPdtyAllModel <= 0)
                {
                    for (int colPdtyAllModel = firstColPdtyAllModel; colPdtyAllModel <= lastColPdtyAllModel; colPdtyAllModel++)
                        ws.Cell(totalPdtyAllRowPdtyAllModel, colPdtyAllModel).Value = 0;
                }
                else
                {
                    // SUMPRODUCT(Pdty by {Model}, N by {Model}) for the current column
                    string fPdtyAllModel =
                        $"SUMPRODUCT(R{pdtyStartPdtyAllModel}C:R{effPdtyLastPdtyAllModel}C, R{nbyStartPdtyAllModel}C:R{effNbyLastPdtyAllModel}C)";

                    for (int colPdtyAllModel = firstColPdtyAllModel; colPdtyAllModel <= lastColPdtyAllModel; colPdtyAllModel++)
                        ws.Cell(totalPdtyAllRowPdtyAllModel, colPdtyAllModel).FormulaR1C1 = fPdtyAllModel;

                    ws.Range(totalPdtyAllRowPdtyAllModel, firstColPdtyAllModel, totalPdtyAllRowPdtyAllModel, lastColPdtyAllModel)
                      .Style.NumberFormat.Format = "#,##0.00";
                }

                // monthly sum formulas
                monthlySumFormulas.Add(new MonthlySumFormula("TotalPdtyAllModel", sumR1C1, totalPdtyAllRowPdtyAllModel, MonthlyCol));


                // parameter Total PDTY by DP
                int totalPdtyByDPRow = pdtyAllModelRow + 1;
                int firstColtotalPdtyByDP = DateColStart;
                int lastColtotalPdtyByDP = DateColEnd;
                string totalPdtyByDPFormula = $"R{ProdRow}C/R{EWHRow}C";
                ws.Cell(totalPdtyByDPRow, ParamCol3).Value = "Total Pdty By DP";
                for (int colPdtyByDP = firstColtotalPdtyByDP; colPdtyByDP <= lastColtotalPdtyByDP; colPdtyByDP++)
                    ws.Cell(totalPdtyByDPRow, colPdtyByDP).FormulaR1C1 = totalPdtyByDPFormula;

                monthlySumFormulas.Add(new MonthlySumFormula("TotalPdtyByDP", sumR1C1, totalPdtyByDPRow, MonthlyCol));

                //===================== MONTHLY ========================================

                //month stat


                ws.Cell(DateRow, MonthlyCol).Value = $"Monthly {monthStr} {year}";

                ws.Cell(MStatRow, MonthlyCol).Value = mStat?.TotalDays ?? 0;
                ws.Cell(AvailableDayRow, MonthlyCol).Value = mStat?.TotalAvailableDays ?? 0;
                ws.Cell(MStatRow + 2, MonthlyCol).Value = mStat?.TotalFridays ?? 0;
                ws.Cell(HolidayRow, MonthlyCol).Value = mStat?.TotalHolidays ?? 0;

                foreach (var entry in monthlySumFormulas)
                {
                    ws.Cell(entry.Row, entry.Col).FormulaR1C1 = entry.Formula;
                }

                dataImportCol = MonthlyCol + 2; // next column for data import

                //===================== END MONTHLY ======================================


                // ================ DATA IMPORT =========================================
                ws.Cell(headerIdRow, dataImportCol).Value = rows.Where(w => w.JobsiteId == g.Key.JobsiteId && w.ProcessId == g.Key.ProcessId).Select(r => r.ProdSchedHeaderId).FirstOrDefault();
                ws.Cell(jobsiteIdRow, dataImportCol).Value = g.Key.JobsiteId;
                ws.Cell(jobsiteNameRow, dataImportCol).Value = rows.Where(w => w.JobsiteId == g.Key.JobsiteId && w.ProcessId == g.Key.ProcessId).Select(r => r.Jobsite).FirstOrDefault(); 
                ws.Cell(processIdRow, dataImportCol).Value = g.Key.ProcessId;
                ws.Cell(processNameRow, dataImportCol).Value = rows.Where(w => w.JobsiteId == g.Key.JobsiteId && w.ProcessId == g.Key.ProcessId).Select(r => r.Process).FirstOrDefault();
                ws.Cell(mbrBasisRow, dataImportCol).Value = mbrVersion;

                const string MetaMarker = "__META_COL__";
                ws.Cell(headerIdRow, dataImportCol).AddToNamed("HeaderId", XLScope.Worksheet);
                ws.Cell(jobsiteIdRow, dataImportCol).AddToNamed("JobsiteId", XLScope.Worksheet);
                ws.Cell(jobsiteNameRow, dataImportCol).AddToNamed("Jobsite", XLScope.Worksheet);
                ws.Cell(processIdRow, dataImportCol).AddToNamed("ProcessId", XLScope.Worksheet);
                ws.Cell(processNameRow, dataImportCol).AddToNamed("Process", XLScope.Worksheet);
                ws.Cell(mbrBasisRow, dataImportCol).AddToNamed("MbrBasis", XLScope.Worksheet);

                ws.Column(dataImportCol).Hide();
                var metaRows = new[] { headerIdRow, jobsiteIdRow, jobsiteNameRow, processIdRow, processNameRow, mbrBasisRow };
                int topMetaRow = metaRows.Min();
                int bottomMetaRow = metaRows.Max();


                var metaRange = ws.Range(topMetaRow, dataImportCol, bottomMetaRow, dataImportCol);

                // “White out” the column
                metaRange.Style.Fill.BackgroundColor = XLColor.White;   // white background
                metaRange.Style.Font.FontColor = XLColor.White;   // white text


                metaRange.Style.Border.OutsideBorder = XLBorderStyleValues.None;
                metaRange.Style.Border.InsideBorder = XLBorderStyleValues.None;
                // ================ END DATA IMPORT =====================================

                // styling
                var used = ws.RangeUsed();
                if (used != null)
                {
                    used.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    used.Style.Border.InsideBorder = XLBorderStyleValues.Hair;
                    ws.SheetView.FreezeRows(ParamHeaderRow);
                    ws.SheetView.FreezeColumns(ParamCol);
                    ws.Column(ParamCol).AdjustToContents();
                    ws.Columns(DateColStart, DateColEnd)
                      .Style.NumberFormat.Format = "#,##0.00";
                    ws.Columns(DateColStart, DateColEnd).Width = 12.0;
                }
            }

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

   
        private static string MakeSheetName(string s)
        {
            foreach (var ch in new[] { ':', '\\', '/', '?', '*', '[', ']' }) s = s.Replace(ch.ToString(), "");
            s = s.Replace('\n', ' ').Replace('\r', ' ');
            if (s.Length > 31) s = s[..31];
            return string.IsNullOrWhiteSpace(s) ? "Sheet" : s;
        }

        private static string UniqueName(string baseName, HashSet<string> used)
        {
            if (!used.Contains(baseName)) return baseName;
            for (int i = 2; i < 1000; i++)
            {
                var candidate = MakeSheetName($"{baseName}-{i}");
                if (!used.Contains(candidate)) return candidate;
            }
            return Guid.NewGuid().ToString("N")[..31];
        }

        private static int GetParameterOrder(string? parameter)
        {
            parameter = (parameter ?? string.Empty).Trim();
            if (parameter.Length > 1 && (parameter[0] == 'S' || parameter[0] == 's') &&
                int.TryParse(parameter[1..], out var n)) return n;

            return parameter.ToUpperInvariant() switch
            {
                "BREAKDOWN" => 1001,
                "RELOCATION" => 1002,
                "PDTY" => 1003,
                "PRODUCTION" => 1004,
                _ => 2000
            };
        }

        private static HashSet<int> ParseDaysCsv(string? csv, int maxDay)
        {
            var set = new HashSet<int>();
            if (string.IsNullOrWhiteSpace(csv)) return set;
            foreach (var tok in csv.Split(',', StringSplitOptions.RemoveEmptyEntries))
                if (int.TryParse(tok.Trim(), out var d) && d >= 1 && d <= maxDay) set.Add(d);
            return set;
        }

        static string ModelFromNBy(string s)
        {
            // "N by PCxxx" -> "PCxxx"
            if (string.IsNullOrWhiteSpace(s)) return "";
            s = s.Trim();
            const string prefix = "N by";
            if (s.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return s[prefix.Length..].Trim();
            return s; // fallback if already just the model
        }
        static string ModelFromPdtyAwal(string s)
        {
            // "Pdty by PCxxx Awal" -> "PCxxx"
            if (string.IsNullOrWhiteSpace(s)) return "";
            s = s.Trim();
            // remove prefix
            const string prefix = "Pdty by";
            if (s.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                s = s[prefix.Length..].Trim();
            // remove trailing "Awal"
            if (s.EndsWith("Awal", StringComparison.OrdinalIgnoreCase))
                s = s[..^4].Trim();
            return s;
        }
    }
}
