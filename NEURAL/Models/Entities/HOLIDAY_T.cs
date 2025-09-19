namespace NEURAL.Models.Entities
{
    public class HOLIDAY_T
    {
        public long ID { get; set; }
        public string MBR_VERSION { get; set; }
        public long JOBSITE_ID { get; set; }
        public DateTime DATE { get; set; }
        public string KETERANGAN { get; set; }
        public decimal DURASI { get; set; }
        public DateTime CREATED_AT { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? UPDATED_AT { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? DELETED_AT { get; set; }
    }
}
