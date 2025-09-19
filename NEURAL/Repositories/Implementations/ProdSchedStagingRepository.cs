using NEURAL.Repositories.Interfaces;
using NEURAL.Utils; 
using System.Data;
using System.Data.SqlClient;
namespace NEURAL.Repositories.Implementations
{
    public sealed class ProdSchedStagingRepository : IProdSchedStagingRepository
    {
        public async Task<int> DeleteByMbrAsync(string mbrVersion, CancellationToken ct)
        {
            using var conn = DbHelper.GetOpenConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct);
            using var cmd = new SqlCommand("DELETE FROM PRODSCHED_STAGING_T WHERE MBR_VERSION=@MBR", conn);
            cmd.Parameters.AddWithValue("@MBR", mbrVersion);
            return await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task BulkWriteAsync(DataTable data, CancellationToken ct)
        {
            using var conn = DbHelper.GetOpenConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct);
            using var bulk = new SqlBulkCopy(conn) { DestinationTableName = "PRODSCHED_STAGING_T" };
            foreach (DataColumn c in data.Columns) bulk.ColumnMappings.Add(c.ColumnName, c.ColumnName);
            await bulk.WriteToServerAsync(data, ct);
        }
    }
}
