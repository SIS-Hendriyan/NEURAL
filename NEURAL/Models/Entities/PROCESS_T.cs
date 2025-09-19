using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEURAL.Models.Entities
{
    [Table("PROCESS_T")]
    public sealed class PROCESS_T
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }

        [Required, MaxLength(20)]
        [Column("NAME", TypeName = "varchar(20)")]
        public string Name { get; set; } = null!;

        [MaxLength(255)]
        [Column("DESCRIPTION", TypeName = "varchar(255)")]
        public string? Description { get; set; }

        [Column("PROCESS_GROUP_ID")]
        public long? ProcessGroupId { get; set; } 

        [Column("CREATED_AT")]
        public DateTime? CreatedAt { get; set; }

        [MaxLength(25)]
        [Column("CREATED_BY", TypeName = "varchar(25)")]
        public string? CreatedBy { get; set; }

        [Column("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        [MaxLength(25)]
        [Column("UPDATED_BY", TypeName = "varchar(25)")]
        public string? UpdatedBy { get; set; }

        [Column("DELETED_AT")]
        public DateTime? DeletedAt { get; set; }

        [MaxLength(25)]
        [Column("DELETED_BY", TypeName = "varchar(25)")]
        public string? DeletedBy { get; set; }

        [MaxLength(4)]
        [Column("PROCESS_SAP", TypeName = "varchar(4)")]
        public string? ProcessSap { get; set; }

        [MaxLength(255)]
        [Column("NAME_SICOPP_PLUS", TypeName = "varchar(255)")]
        public string? NameSicoppPlus { get; set; }

    }
}
