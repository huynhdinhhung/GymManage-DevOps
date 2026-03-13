using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GYM_Manage.Models
{
    public class ThongBao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaThongBao { get; set; }

        // ======================================
        // NGƯỜI NHẬN THÔNG BÁO
        // ======================================

        // Thành viên nhận
        public int? MaThanhVien { get; set; }
        [ForeignKey(nameof(MaThanhVien))]
        public ThanhVien? ThanhVien { get; set; }

        // Huấn luyện viên nhận
        public int? MaHuanLuyenVien { get; set; }
        [ForeignKey(nameof(MaHuanLuyenVien))]
        public HuanLuyenVien? HuanLuyenVien { get; set; }

        // ======================================
        // THÔNG TIN THÔNG BÁO
        // ======================================

        [StringLength(200)]
        public string? TieuDe { get; set; }

        /*
         * Loại thông báo:
         * - HeThong: Mặc định
         * - LichTapTrongNgay: Nhắc lịch tập mỗi ngày
         * - HLVThayThe: Lịch tập có HLV thay thế
         * - DoiLich: Yêu cầu đổi lịch
         * - DangKyGoiTap: Thành viên đăng ký gói tập
         * - AdminDuyet: Quản trị duyệt yêu cầu
         */
        [Required, StringLength(50)]
        public string LoaiThongBao { get; set; } = "HeThong";

        [Required, StringLength(500)]
        public string NoiDung { get; set; } = null!;

        // ======================================
        // THÔNG BÁO LIÊN QUAN ĐỔI LỊCH
        // ======================================

        public int? MaYeuCauDoiLich { get; set; }
        [ForeignKey(nameof(MaYeuCauDoiLich))]
        public YeuCauDoiLich? YeuCauDoiLich { get; set; }

        // Trường này đảm bảo biết HLV nào thay thế
        public int? MaHuanLuyenVienThayThe { get; set; }
        [ForeignKey(nameof(MaHuanLuyenVienThayThe))]
        public HuanLuyenVien? HuanLuyenVienThayThe { get; set; }

        // ======================================
        // TRẠNG THÁI THÔNG BÁO
        // ======================================

        public bool DaDoc { get; set; } = false;         // Đã đọc chưa?
        public bool IsImportant { get; set; } = false;   // Thông báo quan trọng
        public bool DaXem { get; set; } = false;         // Thành viên đã mở xem chưa

        // ======================================
        // THỜI GIAN
        // ======================================

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayGui { get; set; } = DateTime.Now;
    }
}
