using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GYM_Manage.Models
{
    public class LichTapThanhVien
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaLichTap { get; set; }

        // FK: ĐĂNG KÝ GÓI TẬP
        public int MaDangKy { get; set; }

        [ForeignKey(nameof(MaDangKy))]
        public DangKyGoiTap DangKyGoiTap { get; set; } = null!;

        // THÀNH VIÊN
        public int MaThanhVien { get; set; }

        [ForeignKey(nameof(MaThanhVien))]
        public ThanhVien ThanhVien { get; set; } = null!;

        // HLV CHÍNH
        public int MaHuanLuyenVien { get; set; }

        [ForeignKey(nameof(MaHuanLuyenVien))]
        public HuanLuyenVien HuanLuyenVien { get; set; } = null!;

        // CA TẬP
        public int MaCa { get; set; }

        [ForeignKey(nameof(MaCa))]
        public CaLamViec CaLamViec { get; set; } = null!;

        // NGÀY TẬP
        public DateTime NgayTap { get; set; }

        // HLV THAY THẾ
        public bool IsHLVThayThe { get; set; }
        public int? MaHuanLuyenVienThayThe { get; set; }

        [ForeignKey(nameof(MaHuanLuyenVienThayThe))]
        public HuanLuyenVien? HuanLuyenVienThayThe { get; set; }
    }
}
