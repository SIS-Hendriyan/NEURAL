
using NEURAL.Models.Request;
using NEURAL.Utils;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Channels;

namespace NEURAL.Services
{
    public class ProdSchedUploadExcelService : BackgroundService
    {
        private readonly ChannelReader<ProdSchedUploadRequest> _reader;

        public ProdSchedUploadExcelService(Channel<ProdSchedUploadRequest> channel)
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

                        using var cmd = new SqlCommand("PRODSCHED_UPLOAD_SP", conn)
                        { CommandType = CommandType.StoredProcedure };
                        cmd.Parameters.AddWithValue("@MBR_VERSION", job.MbrVersion);

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
