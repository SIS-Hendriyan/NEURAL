using NEURAL.Models.ViewModel;
using NEURAL.Repositories.Interfaces;
using NEURAL.Utils;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace NEURAL.Repositories.Implementations
{
    public class ProdSchedPivotRepository : IProdSchedPivotRepository
    {
        public async Task<List<ProdSchedPivotViewModel>> GetPivot(string periode, string tipeData, string mbrVersion, int year)
        {
            using var conn = DbHelper.GetOpenConnection();
            var result = new List<ProdSchedPivotViewModel>();
            using (var cmd = new SqlCommand("PRODSCHED_PIVOT_SP", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@periode", periode);
                cmd.Parameters.AddWithValue("@tipeData", tipeData);
                cmd.Parameters.AddWithValue("@mbrVersion", mbrVersion);
                cmd.Parameters.AddWithValue("@year", year);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new ProdSchedPivotViewModel
                        {
                            ProdschedHeaderId = reader.GetInt64(reader.GetOrdinal("PRODSCHED_HEADER_ID")),
                            Version = reader["MBR_VERSION"] as string ?? string.Empty,
                            Jobsite = reader["JOBSITE"] as string ?? string.Empty,
                            Area = reader["AREA"] as string ?? string.Empty,
                            Pit = reader["PIT"] as string ?? string.Empty,
                            PeriodKey = reader["PERIOD_KEY"] as string ?? string.Empty,
                          //  Period = (periode != "daily") ? (reader["PERIOD_LABEL"] as string ?? string.Empty) : (reader["PERIOD_KEY"] as string ?? string.Empty),
                            Period = reader["PERIOD_LABEL"] as string ?? string.Empty,
                            SettingId = reader.GetInt32(reader.GetOrdinal("SETTING_ID")),
                            Process = reader["PROCESS"] as string ?? string.Empty,
                            Fleet = reader["FLEET_MODEL"] as string ?? string.Empty,
                            Parameter = reader["PARAMETER"] as string ?? string.Empty,
                            Value = decimal.Parse(reader["VALUE"] as string ?? "0", CultureInfo.InvariantCulture)
                        });
                    }
                }
            }

            return result;
        }

       
    }
}
