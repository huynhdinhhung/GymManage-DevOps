using GYM_Manage.Data;
using GYM_Manage.Models;
using Microsoft.EntityFrameworkCore;

namespace GYM_Manage.Services
{
    public class ThongBaoService
    {
        private readonly GYM_DBcontext _context;

        public ThongBaoService(GYM_DBcontext context)
        {
            _context = context;
        }

        // ==========================================
        // TẠO THÔNG BÁO NHẮC LỊCH TẬP HÀNG NGÀY
        // ==========================================
        public async Task TaoThongBaoLichTapTrongNgay()
        {
            var today = DateTime.Today;

            // Lấy danh sách lịch tập hôm nay
            var lichHomNay = await _context.LichTapThanhViens
                .Include(l => l.ThanhVien)
                .Include(l => l.CaLamViec)
                .Include(l => l.HuanLuyenVien)
                .Where(l => l.NgayTap.Date == today)
                .ToListAsync();

            if (!lichHomNay.Any())
                return;

            // Lấy danh sách thông báo đã tạo hôm nay để tránh trùng
            var thongBaoDaTao = await _context.ThongBaos
                .Where(tb =>
                       tb.LoaiThongBao == "LichTapTrongNgay" &&
                       tb.NgayGui.Date == today)
                .ToListAsync();

            foreach (var lich in lichHomNay)
            {
                // Bảo vệ null
                if (lich.CaLamViec == null || lich.HuanLuyenVien == null)
                    continue;

                bool existed = thongBaoDaTao.Any(tb =>
                    tb.MaThanhVien == lich.MaThanhVien);

                if (existed) continue;

                var tb = new ThongBao
                {
                    MaThanhVien = lich.MaThanhVien,
                    LoaiThongBao = "LichTapTrongNgay",
                    TieuDe = "Nhắc lịch tập hôm nay",
                    NoiDung =
                        $"Hôm nay {today:dd/MM/yyyy}, bạn có buổi tập ca {lich.CaLamViec.TenCa} " +
                        $"({lich.CaLamViec.GioBatDau:hh\\:mm} - {lich.CaLamViec.GioKetThuc:hh\\:mm}) " +
                        $"với HLV {lich.HuanLuyenVien.HoTen}.",
                    NgayGui = DateTime.Now,
                    NgayTao = DateTime.Now,
                    DaDoc = false,
                    IsImportant = true
                };

                _context.ThongBaos.Add(tb);
            }

            await _context.SaveChangesAsync();
        }
    }
}
