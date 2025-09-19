namespace NEURAL.Models.Entities
{
    public class PRODSCHED_HEADER_T
    {
        public long ID { get; set; }
        public string MBR_VERSION { get; set; } = string.Empty;
        public string YEAR { get; set; } = string.Empty;
        public string DOCUMENT_NAME { get; set; } = string.Empty;
        public string VERSION_NAME { get; set; } = string.Empty;
        public byte[] FILE_DATA { get; set; } = Array.Empty<byte>();
        public string? STATUS_DAILY_FLEET { get; set; }
        public string? STATUS_DAILY_PROCESS { get; set; }
        public string? STATUS_MONTHLY_FLEET { get; set; }
        public string? STATUS_MONTHLY_PROCESS { get; set; }
        public string? STATUS_YEARLY_FLEET { get; set; }
        public string? STATUS_YEARLY_PROCESS { get; set; }
        public DateTime CREATED_AT { get; set; }
        public string CREATED_BY { get; set; } = string.Empty;
        public DateTime? UPDATED_AT { get; set; }
        public string? UPDATED_BY { get; set; }
        public DateTime? DELETED_AT { get; set; }
        public string? DELETED_BY { get; set; }
    }
}
