using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using NEURAL.Models.ViewModel;
using NEURAL.Repositories.Interfaces;
using NEURAL.Utils;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace NEURAL.Repositories.Implementations
{
    public class ProdSchedInterventionRepository : IProdSchedInterventionRepository
    {
        public async Task<List<ProdSchedInterventionViewModel>> GetInterventions(long headerId, long jobsiteId, long processId)
        {
            using var conn = DbHelper.GetOpenConnection();
            var result = new List<ProdSchedInterventionViewModel>();
            using (var cmd = new SqlCommand("PRODSCHED_INTERVENTION_SP", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", "HEADER");
                cmd.Parameters.AddWithValue("@headerId", headerId);
                cmd.Parameters.AddWithValue("@jobsiteId", jobsiteId);
                cmd.Parameters.AddWithValue("@processId", processId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new ProdSchedInterventionViewModel
                        {
                            Parameter = reader.IsDBNull(reader.GetOrdinal("Parameter")) ? "" : reader.GetString(reader.GetOrdinal("Parameter")),
                            January = ReadDecNull(reader, "January"),
                            February = ReadDecNull(reader, "February"),
                            March = ReadDecNull(reader, "March"),
                            April = ReadDecNull(reader, "April"),
                            May = ReadDecNull(reader, "May"),
                            June = ReadDecNull(reader, "June"),
                            July = ReadDecNull(reader, "July"),
                            August = ReadDecNull(reader, "August"),
                            September = ReadDecNull(reader, "September"),
                            October = ReadDecNull(reader, "October"),
                            November = ReadDecNull(reader, "November"),
                            December = ReadDecNull(reader, "December"),
                            FY = ReadDecNull(reader, "FY")
                        });

                        static decimal? ReadDecNull(SqlDataReader rd, string name)
                        {
                            var i = rd.GetOrdinal(name);
                            return rd.IsDBNull(i) ? (decimal?)null : rd.GetDecimal(i);
                        }
                    }
                }
            }

            return result;
        }

        public async Task<List<ProdSchedInterventionDownloadViewModel>> GetInterventionsForDownload(long headerId, int month, CancellationToken ct)
        {
            using var conn = DbHelper.GetOpenConnection();
            using var cmd = new SqlCommand("PRODSCHED_INTERVENTION_SP", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@type", "DOWNLOAD");
            cmd.Parameters.AddWithValue("@headerId", headerId);
            cmd.Parameters.AddWithValue("@month", month);

            var result = new List<ProdSchedInterventionDownloadViewModel>();

            using var reader = await cmd.ExecuteReaderAsync(ct);

            int oId = TryGetOrdinal(reader, "ID");
            int oHeaderId = TryGetOrdinal(reader, "PRODSCHED_HEADER_ID");
            int oJobsiteId = TryGetOrdinal(reader, "JOBSITE_ID");
            int oJobsite = TryGetOrdinal(reader, "JOBSITE");
            int oDate = TryGetOrdinal(reader, "DATE");
            int oProcessId = TryGetOrdinal(reader, "PROCESS_ID");
            int oProcess = TryGetOrdinal(reader, "PROCESS");
            int oParameter = TryGetOrdinal(reader, "PARAMETER");
            int oValue = TryGetOrdinal(reader, "VALUE"); 

            while (await reader.ReadAsync(ct))
            {
                decimal valueDec = 0m;
                if (oValue >= 0 && !reader.IsDBNull(oValue))
                {
                    var raw = reader.GetValue(oValue)?.ToString();
                    if (!string.IsNullOrWhiteSpace(raw))
                    {
                  
                        decimal.TryParse(raw, System.Globalization.NumberStyles.Any,
                                         System.Globalization.CultureInfo.InvariantCulture,
                                         out valueDec);
                    }
                }

                result.Add(new ProdSchedInterventionDownloadViewModel
                {
                    Id = GetInt64OrDefault(reader, oId, 0),
                    ProdSchedHeaderId = GetInt64OrDefault(reader, oHeaderId, headerId),
                    JobsiteId = GetInt64OrDefault(reader, oJobsiteId, 0),
                    Jobsite = GetStringOrEmpty(reader, oJobsite),
                    DailyDate = GetDateTimeOrDefault(reader, oDate, DateTime.MinValue),
                    ProcessId = GetInt64OrDefault(reader, oProcessId, 0),
                    Process = GetStringOrEmpty(reader, oProcess),
                    Parameter = GetStringOrEmpty(reader, oParameter),
                    Value = valueDec
                });
            }

            return result;

         
            static int TryGetOrdinal(IDataRecord r, string name)
            {
                for (int i = 0; i < r.FieldCount; i++)
                    if (string.Equals(r.GetName(i), name, StringComparison.OrdinalIgnoreCase))
                        return i;
                return -1; 
            }

            static long GetInt64OrDefault(IDataRecord r, int ord, long def = 0) =>
                ord >= 0 && !r.IsDBNull(ord) ? r.GetInt64(ord) : def;

            static DateTime GetDateTimeOrDefault(IDataRecord r, int ord, DateTime def) =>
                ord >= 0 && !r.IsDBNull(ord) ? r.GetDateTime(ord) : def;

            static string GetStringOrEmpty(IDataRecord r, int ord) =>
                ord >= 0 && !r.IsDBNull(ord) ? r.GetValue(ord)?.ToString() ?? string.Empty : string.Empty;
        }

       

        public async Task<List<ProdSchedInterventionMonthStatViewModel>> GetMonthStats(long headerId, int month)
        {
            using var conn = DbHelper.GetOpenConnection();
            var result = new List<ProdSchedInterventionMonthStatViewModel>();
            using (var cmd = new SqlCommand("PRODSCHED_INTERVENTION_SP", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", "MONTH_STAT");
                cmd.Parameters.AddWithValue("@headerId", headerId);
                cmd.Parameters.AddWithValue("@month", month);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var processes = new HashSet<long>();
                    while (await reader.ReadAsync())
                    {
                        result.Add(new ProdSchedInterventionMonthStatViewModel
                        {
                            JobsiteId = reader.GetInt64(reader.GetOrdinal("JobsiteId")),
                            Jobsite = reader["Jobsite"] as string ?? string.Empty,
                            TotalDays = reader.GetInt32(reader.GetOrdinal("TotalDays")),
                            TotalFridays = reader.GetInt32(reader.GetOrdinal("TotalFridays")),
                            TotalAvailableDays = reader.GetInt32(reader.GetOrdinal("TotalAvailableDays")),
                            TotalHolidays = reader.GetDecimal(reader.GetOrdinal("TotalHolidays")),
                            DayInHoliday = reader["DayInHoliday"] as string ?? string.Empty
                        });

                    }
                }
            }
            return result;
        }

    }
}
