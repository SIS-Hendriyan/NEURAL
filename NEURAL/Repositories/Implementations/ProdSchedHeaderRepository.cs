
using NEURAL.Models.Entities;
using NEURAL.Models.ViewModel;
using NEURAL.Repositories.Interfaces;
using NEURAL.Utils;
using System.Data;
using System.Data.SqlClient;

namespace NEURAL.Repositories.Implementations
{
    public class ProdSchedHeaderRepository : IProdSchedHeaderRepository
    {
        public async Task DeleteProdschedHeader(long id)
        {
            await using var conn = DbHelper.GetOpenConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            await using var cmd = new SqlCommand("PRODSCHED_HEADER_SP", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@type", SqlDbType.VarChar, 20).Value = "DELETE";
            cmd.Parameters.Add("@ID", SqlDbType.BigInt).Value = id;

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<string> DownloadError(long id, CancellationToken ct)
        {
            await using var conn = DbHelper.GetOpenConnection();
            await using var cmd = new SqlCommand("PRODSCHED_HEADER_SP", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@type", SqlDbType.VarChar, 50).Value = "DOWNLOAD_ERROR";
            cmd.Parameters.Add("@ID", SqlDbType.BigInt).Value = id;

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            if (await reader.ReadAsync(ct))
            {
                return reader["ERROR"] as string ?? string.Empty;
            }

            return string.Empty;
        }

        public async Task<(bool Found, string FileName, string ContentType, byte[] Data)> DownloadFile(long id, CancellationToken ct)
        {
            await using var conn = DbHelper.GetOpenConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct);

            await using var cmd = new SqlCommand("PRODSCHED_HEADER_SP", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@type", SqlDbType.VarChar, 20).Value = "DOWNLOAD";
            cmd.Parameters.Add("@ID", SqlDbType.BigInt).Value = id;

            await using var reader = await cmd.ExecuteReaderAsync(
                CommandBehavior.SequentialAccess | CommandBehavior.SingleRow, ct);

            if (!await reader.ReadAsync(ct))
                return (false, null, "application/octet-stream", Array.Empty<byte>());

            int ordData = reader.GetOrdinal("FILE_DATA");
            int ordName = reader.GetOrdinal("DOCUMENT_NAME");

            string? fileName = null;
            using var ms = new MemoryStream();

            if (ordData < ordName)
            {
                if (!reader.IsDBNull(ordData))
                {
                    await using var s = reader.GetStream(ordData);
                    await s.CopyToAsync(ms, 81920, ct);
                }
                fileName = reader.IsDBNull(ordName) ? null : reader.GetString(ordName);
            }
            else
            {
                fileName = reader.IsDBNull(ordName) ? null : reader.GetString(ordName);
                if (!reader.IsDBNull(ordData))
                {
                    await using var s = reader.GetStream(ordData);
                    await s.CopyToAsync(ms, 81920, ct);
                }
            }

            var contentType = FileHelper.GetContentType(fileName);
            return (true, fileName, contentType, ms.ToArray());
        }

        public async Task<List<ProdSchedHeaderViewModel>> GetAllHeaders()
        {
            using var conn = DbHelper.GetOpenConnection();
            var result = new List<ProdSchedHeaderViewModel>();
            using (var cmd = new SqlCommand("PRODSCHED_HEADER_SP", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", "HEADER");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string progress = reader["PROGRESS"] as string ?? string.Empty;
                        var progressList = progress
                                            .Split(',')
                                            .Select(p => p.Trim())
                                            .ToList();
                        if (progressList.Count(p => p.Equals("OK", StringComparison.OrdinalIgnoreCase)) == 6)
                        {
                            progressList.Add("OK");
                        }
                        else
                        {
                            progressList.Add("-");
                        }

                        result.Add(new ProdSchedHeaderViewModel
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("ID")),
                            MbrVersion = reader["MBR_VERSION"] as string ?? string.Empty,
                            Year = reader["YEAR"] as string ?? string.Empty,
                            DocumentName = reader["DOCUMENT_NAME"] as string ?? string.Empty,
                            VersionName = reader["VERSION_NAME"] as string ?? string.Empty,
                            Progress = progressList,
                            Status = ProdSchedHelper.ComputeStatus(progressList)
                        });
                    }
                }
            }
            
            return result;
        }

        public async Task<List<string>> GetMBRVersion(int year)
        {
            using var conn = DbHelper.GetOpenConnection();
            var result = new List<string>();
            using (var cmd = new SqlCommand("PRODSCHED_HEADER_SP", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", "MBR");
                cmd.Parameters.AddWithValue("@year", year);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(reader["MBR_VERSION"] as string ?? string.Empty);
                    }
                }
            }

            return result;
        }

        public async Task<string?> GetNextMbr(long headerId)
        {
            await using var conn = DbHelper.GetOpenConnection();
            await using var cmd = new SqlCommand("SELECT dbo.PRODSCHED_INTERVENTION_NEXT_MBR_FN(@HeaderId)", conn);

            cmd.Parameters.Add("@HeaderId", SqlDbType.BigInt).Value = headerId;

            var result = await cmd.ExecuteScalarAsync();

            return result == DBNull.Value ? null : result?.ToString();
        }

        public async Task<List<int>> GetYearList()
        {
            using var conn = DbHelper.GetOpenConnection();
            var result = new List<int>();
            using (var cmd = new SqlCommand("PRODSCHED_HEADER_SP", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", "YEAR");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(int.Parse(reader.GetString(reader.GetOrdinal("YEAR"))));
                    }
                }
            }
            return result;
        }

        public async Task InsertProdSchedHeader(PRODSCHED_HEADER_T header)
        {
            if (string.IsNullOrWhiteSpace(header.MBR_VERSION))
                throw new ArgumentException("MBR_VERSION is required.");

            header.VERSION_NAME = $"{DateTime.Now:dd/MM/yy HH:mm}-{header.MBR_VERSION}";

            using var conn = DbHelper.GetOpenConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            using var cmd = new SqlCommand("PRODSCHED_HEADER_INSERT_SP", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@MBR_VERSION", header.MBR_VERSION);
            cmd.Parameters.AddWithValue("@YEAR", (object?)header.YEAR ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DOCUMENT_NAME", header.DOCUMENT_NAME ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VERSION_NAME", header.VERSION_NAME);
            cmd.Parameters.AddWithValue("@FILE_DATA", (object?)header.FILE_DATA ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CREATED_BY", header.CREATED_BY ?? (object)DBNull.Value);

            var pOut = new SqlParameter("@NEW_ID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pOut);

            await cmd.ExecuteNonQueryAsync();

            header.ID = Convert.ToInt64(pOut.Value);
        }
    }
}
