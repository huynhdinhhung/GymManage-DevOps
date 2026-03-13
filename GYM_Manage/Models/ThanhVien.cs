using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GYM_Manage.Models
{
    public class ThanhVien
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaThanhVien { get; set; }

        // FK → NguoiDung
        [ForeignKey(nameof(NguoiDung))]
        public int? MaNguoiDung { get; set; }
        public NguoiDung? NguoiDung { get; set; }

        public DateTime? NgaySinh { get; set; }

        [MaxLength(10)]
        public string? GioiTinh { get; set; }

        [MaxLength(20)]
        public string? SoDienThoai { get; set; }

        [MaxLength(300)]
        public string? DiaChi { get; set; }

        // ===== DANH SÁCH THANH TOÁN =====
        public ICollection<ThanhToan> ThanhToans { get; set; }
            = new List<ThanhToan>();

        // ===== GÓI TẬP ĐÃ ĐĂNG KÝ =====
        public ICollection<DangKyGoiTap> DangKyGoiTaps { get; set; }
            = new List<DangKyGoiTap>();

        // ===== LỊCH TẬP (bảng mới) =====
        public ICollection<LichTapThanhVien> LichTapThanhViens { get; set; }
            = new List<LichTapThanhVien>();

        // ===== THÔNG BÁO =====
        public ICollection<ThongBao> ThongBaos { get; set; }
            = new List<ThongBao>();
    }
}
