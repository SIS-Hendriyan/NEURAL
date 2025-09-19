using NEURAL.Repositories.Interfaces;
using NEURAL.Utils;
using System.Data;
using System.Data.SqlClient;

namespace NEURAL.Repositories.Implementations
{
    public sealed class ProdSchedStagingInterventionRepository : IProdSchedStagingInterventionRepository
    {
        public async Task BulkWriteAsync(DataTable data, string newMbrVersion, CancellationToken ct)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Rows.Count == 0) return;

            // isi kolom MBR_VERSION_NEW
            if (!data.Columns.Contains("MBR_VERSION_NEW"))
                data.Columns.Add("MBR_VERSION_NEW", typeof(string));

            foreach (DataRow row in data.Rows)
            {
                row["MBR_VERSION_NEW"] = string.IsNullOrWhiteSpace(newMbrVersion)
                    ? DBNull.Value
                    : newMbrVersion;

                if (data.Columns.Contains("VALUE"))
                {
                    var v = row["VALUE"];
                    if (v == null || v == DBNull.Value || (v is string s && string.IsNullOrWhiteSpace(s)))
                        row["VALUE"] = DBNull.Value;
                }
            }

            await using var conn = DbHelper.GetOpenConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(ct);

            using var bulk = new SqlBulkCopy(conn)
            {
                DestinationTableName = "dbo.PRODSCHED_STAGING_INTERVENTION_T",
                BatchSize = 10_000,
                BulkCopyTimeout = 0
            };

            string[] expected =
            {
                "PRODSCHED_HEADER_ID",
                "MBR_VERSION",
                "MBR_VERSION_NEW",
                "JOBSITE_ID",
                "PROCESS_ID",
                "DAILY_DATE",
                "PARAMETER",
                "VALUE",
                "CREATED_AT",
                "CREATED_BY"
            };

            foreach (var col in expected)
            {
                if (data.Columns.Contains(col))
                    bulk.ColumnMappings.Add(col, col);
            }

            await bulk.WriteToServerAsync(data, ct);
        }

        public async Task<int> DeleteByHeaderId(long headerId, CancellationToken ct)
        {
            using var conn = DbHelper.GetOpenConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct);
            using var cmd = new SqlCommand("DELETE FROM PRODSCHED_STAGING_INTERVENTION_T WHERE PRODSCHED_HEADER_ID=@HeaderId", conn);
            cmd.Parameters.AddWithValue("@HeaderId", headerId);
            return await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}
