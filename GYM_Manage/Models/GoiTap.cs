using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GYM_Manage.Models
{
    public class GoiTap
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaGoiTap { get; set; }

        [Required, MaxLength(150)]
        public string TenGoiTap { get; set; } = null!;

        [MaxLength(500)]
        public string? MoTa { get; set; }

        [Required] // số ngày
        public int ThoiHan { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaTien { get; set; }

        // Đồng bộ với View dropdown
        [Required, MaxLength(30)]
        public string TrangThai { get; set; } = "Đang hoạt động"; // "Đang hoạt động" | "Ngưng hoạt động" | "Khuyến mãi"

        [MaxLength(300)]
        public string? AnhDemo { get; set; }

        // Không bắt buộc nhưng hữu ích
        [Required, MaxLength(20)]
        public string LoaiGoi { get; set; } = "Thuong"; // "Thuong" | "VIP"

        // Nav
        public ICollection<DangKyGoiTap> DangKyGoiTaps { get; set; } = new List<DangKyGoiTap>();
    }
}
