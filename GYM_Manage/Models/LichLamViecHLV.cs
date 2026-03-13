using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GYM_Manage.Models
{
    public class LichLamViecHLV
    {
        [Key]
        public int MaLich { get; set; }

        // ===============================
        // HLV GỐC
        // ===============================
        [Required]
        public int MaHuanLuyenVien { get; set; }
        public HuanLuyenVien? HuanLuyenVien { get; set; }

        // ===============================
        // CA LÀM VIỆC
        // ===============================
        [Required]
        public int MaCa { get; set; }
        public CaLamViec? CaLamViec { get; set; }

        // ===============================
        // THỨ HOẶC NGÀY CỤ THỂ
        // ===============================
        [Range(1, 7)]
        public int ThuTrongTuan { get; set; }

        [Required]
        public DateTime NgayLamViec { get; set; }

        // ===============================
        // TRẠNG THÁI CA
        // ===============================
        [Required, StringLength(20)]
        public string TrangThai { get; set; } = "DangLam";

        // ===============================
        // THAY THẾ LỊCH (THÊM MỚI)
        // ===============================

        // Đây có phải là ca thay thế?
        public bool IsThayThe { get; set; } = false;

        // HLV thay thế
        public int? MaHuanLuyenVienThayThe { get; set; }
        public HuanLuyenVien? HuanLuyenVienThayThe { get; set; }

        // Lý do đổi ca
        [StringLength(200)]
        public string? LyDoThayDoi { get; set; }
    }
}
