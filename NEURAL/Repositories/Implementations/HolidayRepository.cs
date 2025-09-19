using NEURAL.Models.Entities;
using NEURAL.Models.ViewModel;
using NEURAL.Repositories.Interfaces;
using NEURAL.Utils;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;

namespace NEURAL.Repositories.Implementations
{
    public class HolidayRepository : IHolidayRepository
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        public HolidayRepository(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }
        public async Task<HttpStatusCode> CallHolidayApi()
        {
            var baseUrl = _config["HolidayApi:BaseUrl"];
            var keyAuth = _config["HolidayApi:QueryParams:KeyAuth"];

            var url = $"{baseUrl}?KeyAuth={keyAuth}";

            var response = await _httpClient.GetAsync(url);
            return response.StatusCode;
        }

        public async Task<List<HOLIDAY_T>> GetDetailByFilter(int holidayYear, long jobsiteId, string mbrVersion)
        {
            var result = new List<HOLIDAY_T>();

            using (var conn = DbHelper.GetOpenConnection())
            using (var cmd = new SqlCommand("HOLIDAY_SP", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@KET", "DETAIL");
                cmd.Parameters.AddWithValue("@YEAR", holidayYear);
                cmd.Parameters.AddWithValue("@JOBSITE_ID", jobsiteId);
                cmd.Parameters.AddWithValue("@MBR_VERSION", mbrVersion);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int durasiIndex = reader.GetOrdinal("DURASI");
                        result.Add(new HOLIDAY_T
                        {
                            ID = reader.GetInt64(reader.GetOrdinal("ID")),
                            MBR_VERSION = reader["MBR_VERSION"].ToString(),
                            JOBSITE_ID = reader.GetInt64(reader.GetOrdinal("JOBSITE_ID")),
                            DATE = reader.GetDateTime(reader.GetOrdinal("DATE")),
                            KETERANGAN = reader["KETERANGAN"].ToString(),
                            DURASI = reader.IsDBNull(durasiIndex) ? 0m : reader.GetDecimal(durasiIndex),
                            CREATED_AT = reader.GetDateTime(reader.GetOrdinal("CREATED_AT")),
                            CREATED_BY = reader["CREATED_BY"].ToString(),
                            UPDATED_AT = reader.IsDBNull(reader.GetOrdinal("UPDATED_AT")) ? null : reader.GetDateTime(reader.GetOrdinal("UPDATED_AT")),
                            UPDATED_BY = reader["UPDATED_BY"]?.ToString(),
                            DELETED_AT = reader.IsDBNull(reader.GetOrdinal("DELETED_AT")) ? null : reader.GetDateTime(reader.GetOrdinal("DELETED_AT"))
                        });
                    }
                }
            }

            return result;
        }

        public async Task<List<HolidaySummaryViewModel>> GetSummary()
        {
            var result = new List<HolidaySummaryViewModel>();

            using (var conn = DbHelper.GetOpenConnection())
            using (var cmd = new SqlCommand("HOLIDAY_SP", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@KET", "GRID");

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new HolidaySummaryViewModel
                        {
                            HolidayYear = reader.GetInt32(reader.GetOrdinal("HOLIDAY_YEAR")),
                            Jobsite = reader["JOBSITE_NAME"].ToString(),
                            JobSiteId = reader.GetInt64(reader.GetOrdinal("JOBSITE_ID")),
                            MbrVersion = reader["MBR_VERSION"].ToString()
                        });
                    }
                }
            }

            return result;
        }
    }
}
