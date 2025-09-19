using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEURAL.Models.Entities
{
    [Table("JOBSITE_T")]
    public sealed class JOBSITE_T
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }

        [Required, MaxLength(10)]
        [Column("CODE", TypeName = "varchar(10)")]
        public string Code { get; set; } = null!;

        [Required, MaxLength(25)]
        [Column("NAME", TypeName = "varchar(25)")]
        public string Name { get; set; } = null!;

        [Column("CREATED_AT")] public DateTime? CreatedAt { get; set; }
        [Column("CREATED_BY"), MaxLength(25)] public string? CreatedBy { get; set; }
        [Column("UPDATED_AT")] public DateTime? UpdatedAt { get; set; }
        [Column("UPDATED_BY"), MaxLength(25)] public string? UpdatedBy { get; set; }
        [Column("DELETED_AT")] public DateTime? DeletedAt { get; set; }
        [Column("DELETED_BY"), MaxLength(25)] public string? DeletedBy { get; set; }

        [Column("NAME_SICOPP_PLUS"), MaxLength(255)]
        public string? NameSicoppPlus { get; set; }
    }
}
