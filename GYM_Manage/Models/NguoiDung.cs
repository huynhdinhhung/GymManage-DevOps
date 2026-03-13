using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GYM_Manage.Models
{
    [Table("NguoiDungs")]
    [Index(nameof(TenDangNhap), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class NguoiDung
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaNguoiDung { get; set; }

        [Required, MaxLength(50)]
        public string TenDangNhap { get; set; } = null!;

        [Required, MaxLength(256)] // hash
        public string MatKhau { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required, MaxLength(100)]
        public string HoTen { get; set; } = null!;

        [Required, MaxLength(20)]
        public string VaiTro { get; set; } = "Member";

        [Required, MaxLength(20)]
        public string TrangThai { get; set; } = "HoatDong";

        [Column(TypeName = "datetime2")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Column(TypeName = "datetime2")]
        public DateTime? LanDangNhapCuoi { get; set; }

        // VIP
        public bool IsVip { get; set; } = false;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongChiTieu { get; set; } = 0m;

        [Column(TypeName = "datetime2")]
        public DateTime? VipSinceUtc { get; set; }

        // Nav
        public ICollection<ThanhVien> ThanhViens { get; set; } = new List<ThanhVien>();
    }
}
