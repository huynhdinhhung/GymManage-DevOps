using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GYM_Manage.Models
{
    [Table("CaLamViec")]   
    public class CaLamViec
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaCa { get; set; }

        [Required, MaxLength(50)]
        public string TenCa { get; set; } = string.Empty;

        [Required]
        public TimeSpan GioBatDau { get; set; }

        [Required]
        public TimeSpan GioKetThuc { get; set; }

        [MaxLength(100)]
        public string? GhiChu { get; set; }

        public ICollection<LichLamViecHLV>? LichLamViecs { get; set; }
    }
}
