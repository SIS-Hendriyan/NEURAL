using NEURAL.Models.Request;
using NEURAL.Utils;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Channels;

namespace NEURAL.Services.Implementations
{
    public class ProdSchedUploadInterventionService : BackgroundService
    {
        private readonly ChannelReader<ProdSchedInterventionUploadRequest> _reader;

        public ProdSchedUploadInterventionService(Channel<ProdSchedInterventionUploadRequest> channel)
           => _reader = channel.Reader;
        
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (await _reader.WaitToReadAsync(ct))
            {
                while (_reader.TryRead(out var job))
                {
                    try
                    {
                        using var conn = DbHelper.GetOpenConnection();
                        if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct);

                        using var cmd = new SqlCommand("PRODSCHED_INTERVENTION_UPLOAD_SP", conn)
                        { CommandType = CommandType.StoredProcedure };
                        cmd.Parameters.Add("@PRODSCHED_HEADER_ID", SqlDbType.BigInt).Value = job.ProdSchedHeaderId;
                        cmd.Parameters.Add("@MBR_VERSION", SqlDbType.NVarChar, 100).Value = job.MbrVersion;
                        cmd.Parameters.Add("@MBR_VERSION_NEW", SqlDbType.NVarChar, 100).Value = job.MbrVersionNew;
                        cmd.Parameters.Add("@YEAR", SqlDbType.Int).Value = job.Year;
                        cmd.Parameters.Add("@MONTH", SqlDbType.Int).Value = job.Month;

                        await cmd.ExecuteNonQueryAsync(ct);
                        Console.WriteLine($"[BG] PRODSCHED_UPLOAD_SP OK: {job.MbrVersion}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[BG] PRODSCHED_UPLOAD_SP FAIL ({job.MbrVersion}): {ex.Message}");
                    }
                }
            }
        }
      
    }
    
}
