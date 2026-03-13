using GYM_Manage.Models;
using Microsoft.EntityFrameworkCore;

namespace GYM_Manage.Data
{
    public class GYM_DBcontext : DbContext
    {
        public GYM_DBcontext(DbContextOptions<GYM_DBcontext> options) : base(options) { }

        // ========== DANH SÁCH BẢNG ==========
        public DbSet<NguoiDung> NguoiDungs { get; set; } = null!;
        public DbSet<ThanhVien> ThanhViens { get; set; } = null!;
        public DbSet<HuanLuyenVien> HuanLuyenViens { get; set; } = null!;
        public DbSet<CaLamViec> CaLamViecs { get; set; } = null!;
        public DbSet<LichLamViecHLV> LichLamViecHLVs { get; set; } = null!;
        public DbSet<GoiTap> GoiTaps { get; set; } = null!;
        public DbSet<DangKyGoiTap> DangKyGoiTaps { get; set; } = null!;
        public DbSet<ThanhToan> ThanhToans { get; set; } = null!;
        public DbSet<HoaDon> HoaDons { get; set; } = null!;
        public DbSet<ThietBi> ThietBis { get; set; } = null!;
        public DbSet<BaiViet> BaiViets { get; set; } = null!;

        public DbSet<YeuCauDoiLich> YeuCauDoiLichs { get; set; } = null!;
        public DbSet<ThongBao> ThongBaos { get; set; } = null!;
        public DbSet<LichTapThanhVien> LichTapThanhViens { get; set; } = null!;
        public DbSet<OtpCode> OtpCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UNIQUE USERNAME
            modelBuilder.Entity<NguoiDung>()
                .HasIndex(u => u.TenDangNhap)
                .IsUnique();

            // UNIQUE PHONE
            modelBuilder.Entity<ThanhVien>()
                .HasIndex(tv => tv.SoDienThoai)
                .IsUnique()
                .HasFilter("[SoDienThoai] IS NOT NULL");

            // DECIMAL CONFIG
            modelBuilder.Entity<GoiTap>()
                .Property(g => g.GiaTien)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<HoaDon>()
                .Property(h => h.TongSoTien)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ThanhToan>()
                .Property(t => t.SoTien)
                .HasColumnType("decimal(18,2)");

            // ============================
            // QUAN HỆ ĐĂNG KÝ GÓI TẬP
            // ============================
            modelBuilder.Entity<DangKyGoiTap>()
                .HasOne(dk => dk.ThanhVien)
                .WithMany(tv => tv.DangKyGoiTaps)
                .HasForeignKey(dk => dk.MaThanhVien)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DangKyGoiTap>()
                .HasOne(dk => dk.GoiTap)
                .WithMany(gt => gt.DangKyGoiTaps)
                .HasForeignKey(dk => dk.MaGoiTap)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DangKyGoiTap>()
                .HasOne(dk => dk.HuanLuyenVien)
                .WithMany(hlv => hlv.DangKyGoiTaps)
                .HasForeignKey(dk => dk.MaHuanLuyenVien)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DangKyGoiTap>()
                .HasOne(dk => dk.CaLamViec)
                .WithMany()
                .HasForeignKey(dk => dk.MaCa)
                .OnDelete(DeleteBehavior.NoAction);

            // ============================
            // THANH TOÁN
            // ============================
            modelBuilder.Entity<ThanhToan>()
                .HasOne(t => t.ThanhVien)
                .WithMany(tv => tv.ThanhToans)
                .HasForeignKey(t => t.MaThanhVien)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThanhToan>()
       .HasOne(t => t.DangKyGoiTap)
       .WithMany(dk => dk.ThanhToans)
       .HasForeignKey(t => t.MaDangKyGoiTap)
       .OnDelete(DeleteBehavior.NoAction);



            // ============================
            // LỊCH LÀM VIỆC HLV
            // ============================
            modelBuilder.Entity<LichLamViecHLV>()
                .HasOne(l => l.HuanLuyenVien)
                .WithMany(h => h.LichLamViecs)
                .HasForeignKey(l => l.MaHuanLuyenVien)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LichLamViecHLV>()
                .HasOne(l => l.CaLamViec)
                .WithMany(c => c.LichLamViecs)
                .HasForeignKey(l => l.MaCa)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================
            // LỊCH TẬP THÀNH VIÊN
            // ============================
            modelBuilder.Entity<LichTapThanhVien>()
                .HasOne(l => l.DangKyGoiTap)
                .WithMany(dk => dk.LichTapThanhViens)
                .HasForeignKey(l => l.MaDangKy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LichTapThanhVien>()
                .HasOne(l => l.ThanhVien)
                .WithMany(tv => tv.LichTapThanhViens)
                .HasForeignKey(l => l.MaThanhVien)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<LichTapThanhVien>()
                .HasOne(l => l.HuanLuyenVien)
                .WithMany()
                .HasForeignKey(l => l.MaHuanLuyenVien)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<LichTapThanhVien>()
                .HasOne(l => l.HuanLuyenVienThayThe)
                .WithMany()
                .HasForeignKey(l => l.MaHuanLuyenVienThayThe)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<LichTapThanhVien>()
                .HasOne(l => l.CaLamViec)
                .WithMany()
                .HasForeignKey(l => l.MaCa)
                .OnDelete(DeleteBehavior.NoAction);

            // ============================
            // THÔNG BÁO
            // ============================
            modelBuilder.Entity<ThongBao>()
                .HasOne(tb => tb.HuanLuyenVien)
                .WithMany(hlv => hlv.ThongBaos)
                .HasForeignKey(tb => tb.MaHuanLuyenVien)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThongBao>()
                .HasOne(tb => tb.ThanhVien)
                .WithMany(tv => tv.ThongBaos)
                .HasForeignKey(tb => tb.MaThanhVien)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThongBao>()
                .HasOne(tb => tb.YeuCauDoiLich)
                .WithMany()
                .HasForeignKey(tb => tb.MaYeuCauDoiLich)
                .OnDelete(DeleteBehavior.NoAction);

            // ============================
            // YÊU CẦU ĐỔI LỊCH
            // ============================

            // HLV GỬI YÊU CẦU
            modelBuilder.Entity<YeuCauDoiLich>()
                .HasOne(yc => yc.HlvGui)
                .WithMany(hlv => hlv.YeuCauDoiLichGui)
                .HasForeignKey(yc => yc.MaHlvGui)
                .OnDelete(DeleteBehavior.Restrict);

            // HLV NHẬN YÊU CẦU
            modelBuilder.Entity<YeuCauDoiLich>()
                .HasOne(yc => yc.HlvNhan)
                .WithMany(hlv => hlv.YeuCauDoiLichNhan)
                .HasForeignKey(yc => yc.MaHlvNhan)
                .OnDelete(DeleteBehavior.Restrict);

            // Lịch gửi
            modelBuilder.Entity<YeuCauDoiLich>()
                .HasOne(yc => yc.LichGui)
                .WithMany()
                .HasForeignKey(yc => yc.MaLichGui)
                .OnDelete(DeleteBehavior.Restrict);

            // Lịch nhận (nullable)
            modelBuilder.Entity<YeuCauDoiLich>()
                .HasOne(yc => yc.LichNhan)
                .WithMany()
                .HasForeignKey(yc => yc.MaLichNhan)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
