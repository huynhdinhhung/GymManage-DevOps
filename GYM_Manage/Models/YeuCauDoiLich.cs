using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GYM_Manage.Models
{
    public class YeuCauDoiLich
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaYeuCau { get; set; }

        // ======================================================
        // HLV GỬI YÊU CẦU
        // ======================================================
        [Required]
        public int MaHlvGui { get; set; }

        [ForeignKey(nameof(MaHlvGui))]
        [InverseProperty(nameof(HuanLuyenVien.YeuCauDoiLichGui))]
        public HuanLuyenVien HlvGui { get; set; } = null!;

        // ======================================================
        // HLV NHẬN YÊU CẦU
        // ======================================================
        [Required]
        public int MaHlvNhan { get; set; }

        [ForeignKey(nameof(MaHlvNhan))]
        [InverseProperty(nameof(HuanLuyenVien.YeuCauDoiLichNhan))]
        public HuanLuyenVien HlvNhan { get; set; } = null!;

        // ======================================================
        // LỊCH GỬI (BẮT BUỘC)
        // ======================================================
        public int MaLichGui { get; set; }

        [ForeignKey(nameof(MaLichGui))]
        public LichLamViecHLV LichGui { get; set; } = null!;

        // ======================================================
        // LỊCH NHẬN (TÙY CHỌN)
        // ======================================================
        public int? MaLichNhan { get; set; }

        [ForeignKey(nameof(MaLichNhan))]
        public LichLamViecHLV? LichNhan { get; set; }

        // ======================================================
        // LOẠI ĐỔI LỊCH
        // ======================================================
        
        [StringLength(20)]
        public string LoaiDoi { get; set; } = "Swap";

        // ======================================================
        // LIÊN QUAN THÀNH VIÊN (DÙNG CHO GỬI THÔNG BÁO)
        // ======================================================
        public int? MaThanhVien { get; set; }

        [ForeignKey(nameof(MaThanhVien))]
        public ThanhVien? ThanhVien { get; set; }

        // ======================================================
        // MÔ TẢ & TRẠNG THÁI
        // ======================================================
        [StringLength(500)]
        public string? LyDo { get; set; }

       
        [Required, StringLength(20)]
        public string TrangThai { get; set; } = "ChoHLV";

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayXuLy { get; set; }
        public DateTime NgayGui { get; set; } = DateTime.Now;
    }
}
