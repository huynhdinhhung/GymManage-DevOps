using GYM_Manage.Data;
using GYM_Manage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_Manage.Controllers.HLV
{
    [Area("HLV")]
    public class HLV_YeuCauDoiLichController : Controller
    {
        private readonly GYM_DBcontext _context;

        public HLV_YeuCauDoiLichController(GYM_DBcontext context)
        {
            _context = context;
        }

        // ==========================
        // Lấy HLV đăng nhập
        // ==========================
        private async Task<HuanLuyenVien?> GetLoggedHLV()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return null;

            return await _context.HuanLuyenViens
                .Include(x => x.NguoiDung)
                .FirstOrDefaultAsync(x => x.MaNguoiDung == userId);
        }

        private async Task<List<LichLamViecHLV>> GetLichByHLV(int hlvId)
        {
            return await _context.LichLamViecHLVs
                .Include(x => x.CaLamViec)
                .Where(x => x.MaHuanLuyenVien == hlvId)
                .OrderBy(x => x.NgayLamViec)
                .ThenBy(x => x.MaCa)
                .ToListAsync();
        }

        // ==========================
        // TRANG INDEX
        // ==========================
        public async Task<IActionResult> Index()
        {
            var hlv = await GetLoggedHLV();
            if (hlv == null) return RedirectToAction("Login", "Auth");

            int id = hlv.MaHuanLuyenVien;

            var list = await _context.YeuCauDoiLichs
                .Include(x => x.HlvGui)
                .Include(x => x.HlvNhan)
                .Include(x => x.LichGui).ThenInclude(l => l.CaLamViec)
                .Include(x => x.LichNhan).ThenInclude(l => l.CaLamViec)
                .Where(x => x.MaHlvGui == id || x.MaHlvNhan == id)
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();

            ViewBag.MyLich = await GetLichByHLV(id);

            ViewBag.OtherHLVs = await _context.HuanLuyenViens
                .Where(x => x.MaHuanLuyenVien != id)
                .OrderBy(x => x.HoTen)
                .ToListAsync();

            ViewBag.HLV = hlv;

            return View("~/Views/HLV/YeuCauDoiLich/Index.cshtml", list);
        }

        // ============================================================
        //  API mới — LẤY CA CỦA HLV BÊN KIA (Fix lỗi dropdown load hoài)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetLichTheoHLV(int hlvId)
        {
            var lich = await _context.LichLamViecHLVs
                .Include(x => x.CaLamViec)
                .Where(x => x.MaHuanLuyenVien == hlvId)
                .OrderBy(x => x.NgayLamViec)
                .ThenBy(x => x.MaCa)
                .ToListAsync();

            string html = "<option value=''>-- Chọn ca --</option>";

            foreach (var l in lich)
            {
                html += $"<option value='{l.MaLich}'>" +
                        $"{l.NgayLamViec:dd/MM} - {l.CaLamViec?.TenCa}" +
                        "</option>";
            }

            return Content(html, "text/html");
        }

        // ============================================================
        // GỬI YÊU CẦU
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> GuiYeuCau(int maLichGui, int maLichNhan, string lyDo)
        {
            var hlv = await GetLoggedHLV();
            if (hlv == null) return RedirectToAction("Login", "Auth");

            var lichGui = await _context.LichLamViecHLVs.FindAsync(maLichGui);
            var lichNhan = await _context.LichLamViecHLVs.FindAsync(maLichNhan);

            if (lichGui == null || lichNhan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch.";
                return RedirectToAction(nameof(Index));
            }

            if (lichGui.MaHuanLuyenVien == lichNhan.MaHuanLuyenVien)
            {
                TempData["ErrorMessage"] = "Không thể đổi ca với chính mình.";
                return RedirectToAction(nameof(Index));
            }

            // Check trùng yêu cầu
            bool existed = await _context.YeuCauDoiLichs.AnyAsync(x =>
                x.MaLichGui == maLichGui &&
                x.MaLichNhan == maLichNhan &&
                x.TrangThai != "TuChoi");

            if (existed)
            {
                TempData["ErrorMessage"] = "Yêu cầu này đã tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            var yc = new YeuCauDoiLich
            {
                MaHlvGui = hlv.MaHuanLuyenVien,
                MaHlvNhan = lichNhan.MaHuanLuyenVien,
                MaLichGui = maLichGui,
                MaLichNhan = maLichNhan,
                LyDo = lyDo,
                TrangThai = "ChoHLV",
                NgayTao = DateTime.Now
            };

            _context.YeuCauDoiLichs.Add(yc);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Gửi yêu cầu thành công.";

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // HLV NHẬN → ĐỒNG Ý
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> DongY(int idYeuCau)
        {
            var hlv = await GetLoggedHLV();
            if (hlv == null) return RedirectToAction("Login", "Auth");

            var yc = await _context.YeuCauDoiLichs.FindAsync(idYeuCau);

            if (yc == null || yc.MaHlvNhan != hlv.MaHuanLuyenVien)
            {
                TempData["ErrorMessage"] = "Không thể xử lý yêu cầu.";
                return RedirectToAction(nameof(Index));
            }

            yc.TrangThai = "ChoAdmin";
            yc.NgayXuLy = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã đồng ý yêu cầu.";

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // HLV NHẬN → TỪ CHỐI
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> TuChoi(int idYeuCau)
        {
            var hlv = await GetLoggedHLV();
            if (hlv == null) return RedirectToAction("Login", "Auth");

            var yc = await _context.YeuCauDoiLichs.FindAsync(idYeuCau);

            if (yc == null || yc.MaHlvNhan != hlv.MaHuanLuyenVien)
            {
                TempData["ErrorMessage"] = "Không thể xử lý yêu cầu.";
                return RedirectToAction(nameof(Index));
            }

            yc.TrangThai = "TuChoi";
            yc.NgayXuLy = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã từ chối yêu cầu.";

            return RedirectToAction(nameof(Index));
        }
    }
}
