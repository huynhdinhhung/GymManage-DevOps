using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GYM_Manage.Models
{
    public class DangKyGoiTap
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaDangKy { get; set; }

        // THÀNH VIÊN
        public int MaThanhVien { get; set; }
        public ThanhVien ThanhVien { get; set; } = null!;

        // GÓI TẬP
        public int MaGoiTap { get; set; }
        public GoiTap GoiTap { get; set; } = null!;

        // THỜI HẠN
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string TrangThai { get; set; } = "HoatDong";

        // VIP OPTIONS
        public int? MaHuanLuyenVien { get; set; }
        public HuanLuyenVien? HuanLuyenVien { get; set; }

        public int? MaCa { get; set; }
        public CaLamViec? CaLamViec { get; set; }

        public int? ThuTrongTuan { get; set; }
        public DateTime? NgayTap { get; set; }

        // LỊCH TẬP
        public ICollection<LichTapThanhVien>? LichTapThanhViens { get; set; }
            = new List<LichTapThanhVien>();

        // THANH TOÁN  (Sửa lỗi ở đây)
        public ICollection<ThanhToan> ThanhToans { get; set; }
            = new List<ThanhToan>();
    }
}
