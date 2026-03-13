using GYM_Manage.Data;
using GYM_Manage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_Manage.Controllers.Admin
{
    [Area("Admin")]
    public class QL_YeuCauDoiLichController : Controller
    {
        private readonly GYM_DBcontext _context;

        public QL_YeuCauDoiLichController(GYM_DBcontext context)
        {
            _context = context;
        }

        // =====================================================
        // 1. DANH SÁCH YÊU CẦU ĐỔI LỊCH
        // =====================================================
        public async Task<IActionResult> Index()
        {
            var list = await _context.YeuCauDoiLichs
                .Include(x => x.HlvGui)
                .Include(x => x.HlvNhan)
                .Include(x => x.LichGui).ThenInclude(l => l.CaLamViec)
                .Include(x => x.LichNhan).ThenInclude(l => l.CaLamViec)
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();

            return View("~/Views/Admin/QL_HuanLuyenVien/YeuCauDoiLich.cshtml", list);
        }

        // =====================================================
        // 2. ADMIN DUYỆT YÊU CẦU ĐỔI LỊCH
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> Duyet(int id)
        {
            var yc = await _context.YeuCauDoiLichs
                .Include(x => x.LichGui).ThenInclude(l => l.CaLamViec)
                .Include(x => x.LichNhan).ThenInclude(l => l.CaLamViec)
                .Include(x => x.HlvGui)
                .Include(x => x.HlvNhan)
                .FirstOrDefaultAsync(x => x.MaYeuCau == id);

            if (yc == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy yêu cầu.";
                return RedirectToAction(nameof(Index));
            }

            if (yc.TrangThai != "ChoAdmin")
            {
                TempData["ErrorMessage"] = "Yêu cầu chưa được HLV nhận xác nhận.";
                return RedirectToAction(nameof(Index));
            }

            if (yc.LichGui == null || yc.LichNhan == null)
            {
                TempData["ErrorMessage"] = "Dữ liệu lịch không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            // =====================================================
            // HOÁN ĐỔI HLV CHO 2 LỊCH
            // =====================================================
            int temp = yc.LichGui.MaHuanLuyenVien;
            yc.LichGui.MaHuanLuyenVien = yc.LichNhan.MaHuanLuyenVien;
            yc.LichNhan.MaHuanLuyenVien = temp;

            yc.TrangThai = "DaDuyet";
            yc.NgayXuLy = DateTime.Now;

            // =====================================================
            // CẬP NHẬT LỊCH TẬP THÀNH VIÊN
            // =====================================================
            var lichTap = await _context.LichTapThanhViens
                .Include(x => x.CaLamViec)
                .Where(x => x.MaLichTap == yc.MaLichGui || x.MaLichTap == yc.MaLichNhan)
                .ToListAsync();

            foreach (var lt in lichTap)
            {
                if (lt.MaLichTap == yc.MaLichGui)
                    lt.MaHuanLuyenVienThayThe = yc.MaHlvNhan;

                if (lt.MaLichTap == yc.MaLichNhan)
                    lt.MaHuanLuyenVienThayThe = yc.MaHlvGui;

                // =====================================================
                // GỬI THÔNG BÁO CHO THÀNH VIÊN BỊ ẢNH HƯỞNG
                // =====================================================
                _context.ThongBaos.Add(new ThongBao
                {
                    MaThanhVien = lt.MaThanhVien,
                    LoaiThongBao = "HLVThayThe",
                    TieuDe = "Lịch tập có HLV thay thế",
                    NoiDung = $"Buổi tập ngày {lt.NgayTap:dd/MM/yyyy} ca {lt.CaLamViec?.TenCa} " +
                              $"đã thay đổi HLV do đổi ca giữa {yc.HlvGui?.HoTen} và {yc.HlvNhan?.HoTen}.",
                    NgayGui = DateTime.Now,
                    IsImportant = true,
                    DaDoc = false
                });
            }

            // =====================================================
            // THÔNG BÁO CHO 2 HLV
            // =====================================================
            _context.ThongBaos.Add(new ThongBao
            {
                MaHuanLuyenVien = yc.MaHlvGui,
                LoaiThongBao = "DoiLich",
                TieuDe = "Yêu cầu đổi ca đã được duyệt",
                NoiDung = $"Yêu cầu đổi ca với {yc.HlvNhan?.HoTen} đã được ADMIN duyệt.",
                NgayGui = DateTime.Now
            });

            _context.ThongBaos.Add(new ThongBao
            {
                MaHuanLuyenVien = yc.MaHlvNhan,
                LoaiThongBao = "DoiLich",
                TieuDe = "Yêu cầu đổi ca đã được duyệt",
                NoiDung = $"Yêu cầu đổi ca với {yc.HlvGui?.HoTen} đã được ADMIN duyệt.",
                NgayGui = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đổi lịch thành công!";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // 3. ADMIN TỪ CHỐI YÊU CẦU
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> TuChoi(int id)
        {
            var yc = await _context.YeuCauDoiLichs
                .Include(x => x.HlvGui)
                .FirstOrDefaultAsync(x => x.MaYeuCau == id);

            if (yc == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy yêu cầu.";
                return RedirectToAction(nameof(Index));
            }

            yc.TrangThai = "TuChoi";
            yc.NgayXuLy = DateTime.Now;

            // =====================================================
            // THÔNG BÁO CHO HLV GỬI
            // =====================================================
            _context.ThongBaos.Add(new ThongBao
            {
                MaHuanLuyenVien = yc.MaHlvGui,
                LoaiThongBao = "DoiLich",
                TieuDe = "Yêu cầu đổi ca bị từ chối",
                NoiDung = "Yêu cầu đổi ca của bạn đã bị ADMIN từ chối.",
                NgayGui = DateTime.Now,
                DaDoc = false
            });

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã từ chối yêu cầu.";
            return RedirectToAction(nameof(Index));
        }
    }
}
