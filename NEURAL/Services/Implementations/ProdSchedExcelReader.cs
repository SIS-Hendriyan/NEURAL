using ClosedXML.Excel;
using NEURAL.Services.Interfaces;
using System.Data;

namespace NEURAL.Services.Implementations
{

    public sealed class ProdSchedExcelReader : IProdSchedExcelReader
    {
        private static readonly Dictionary<int, string> Map = new()
        {
            { 0,"SOURCE"},{1,"MBR_VERSION"},{2,"JOBSITE"},{3,"AREA"},{4,"PIT"},
            { 5,"PERIODE"},{6,"SETTING_ID"},{7,"PROCESS"},{8,"MATERIAL"},
            { 9,"GEOMETRY"},{10,"CONDITION"},{11,"FLEET_MODEL"},{12,"PARAMETER"},{13,"VALUE"}
        };

        public ProdSchedExcelParseResult Read(Stream excelStream)
        {
            using var wb = new XLWorkbook(excelStream);
            var ws = wb.Worksheet(1);

            var dt = new DataTable();
            foreach (var c in Map.Values) dt.Columns.Add(c);

            string? year = null, mbr = null;
            bool skipHeader = true;

            foreach (var row in ws.RowsUsed())
            {
                if (skipHeader) { skipHeader = false; continue; }
                var nr = dt.NewRow();
                foreach (var kv in Map)
                {
                    var cell = row.Cell(kv.Key + 1);
                    if (cell == null || cell.IsEmpty()) { nr[kv.Value] = DBNull.Value; continue; }

                    if (kv.Value == "VALUE" && double.TryParse(cell.Value.ToString(), out var d))
                        nr[kv.Value] = d;
                    else
                        nr[kv.Value] = cell.Value;

                    if (kv.Value == "PERIODE" && year == null &&
                        DateTime.TryParse(cell.Value.ToString(), out var dtp)) year = dtp.Year.ToString();

                    if (kv.Value == "MBR_VERSION" && mbr == null) mbr = cell.Value.ToString();
                }
                dt.Rows.Add(nr);
            }

            return new ProdSchedExcelParseResult { Data = dt, ExtractedYear = year, ExtractedMbrVersion = mbr };
        }
    }
}
