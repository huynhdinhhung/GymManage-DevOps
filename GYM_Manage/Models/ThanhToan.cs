using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GYM_Manage.Models
{
    public class ThanhToan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaThanhToan { get; set; }

        [ForeignKey(nameof(ThanhVien))]
        public int MaThanhVien { get; set; }
        public ThanhVien ThanhVien { get; set; } = null!;

        [ForeignKey(nameof(DangKyGoiTap))]
        public int MaDangKyGoiTap { get; set; }
        public DangKyGoiTap DangKyGoiTap { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SoTien { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime NgayThanhToan { get; set; } = DateTime.Now;

        [Required, MaxLength(30)]
        public string PhuongThucThanhToan { get; set; } = "MoMo";

        [Required, MaxLength(20)]
        public string TrangThai { get; set; } = "ThanhCong";
    }
}
