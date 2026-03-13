using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GYM_Manage.Models
{
    public class HuanLuyenVien
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaHuanLuyenVien { get; set; }

        // FK → Người dùng (account)
        public int? MaNguoiDung { get; set; }
        public NguoiDung? NguoiDung { get; set; }

        // Thông tin cá nhân
        [Required, StringLength(100)]
        public string HoTen { get; set; } = null!;

        public string? AnhDaiDien { get; set; }

        [StringLength(100)]
        public string? ChuyenMon { get; set; }

        [StringLength(100)]
        public string? ChungChi { get; set; }

        public DateTime? NgayTuyenDung { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "HoatDong";

        // ==========================================================
        //                  NAVIGATION PROPERTIES
        // ==========================================================

        // 1. Lịch làm việc cố định
        public ICollection<LichLamViecHLV> LichLamViecs { get; set; }
            = new List<LichLamViecHLV>();

        // 2. Thành viên đang tập cùng HLV
        public ICollection<DangKyGoiTap> DangKyGoiTaps { get; set; }
            = new List<DangKyGoiTap>();

        // 3. Lịch tập của học viên (HLV chính hoặc thay thế)
        public ICollection<LichTapThanhVien> LichTapThanhViens { get; set; }
            = new List<LichTapThanhVien>();

        // ==========================================================
        //      QUAN HỆ ĐỔI LỊCH — CẦN INVERSEPROPERTY CHO EF CORE
        // ==========================================================

        // 4. Danh sách yêu cầu đổi lịch mà HLV GỬI
        [InverseProperty(nameof(YeuCauDoiLich.HlvGui))]
        public ICollection<YeuCauDoiLich> YeuCauDoiLichGui { get; set; }
            = new List<YeuCauDoiLich>();

        // 5. Danh sách yêu cầu đổi lịch mà HLV NHẬN
        [InverseProperty(nameof(YeuCauDoiLich.HlvNhan))]
        public ICollection<YeuCauDoiLich> YeuCauDoiLichNhan { get; set; }
            = new List<YeuCauDoiLich>();

        // 6. Thông báo
        public ICollection<ThongBao> ThongBaos { get; set; }
            = new List<ThongBao>();
    }
}
