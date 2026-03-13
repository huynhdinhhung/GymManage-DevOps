using GYM_Manage.Data;
using GYM_Manage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_Manage.Controllers
{
    public class ThongBaoController : Controller
    {
        private readonly GYM_DBcontext _context;

        public ThongBaoController(GYM_DBcontext context)
        {
            _context = context;
        }

        // ================================
        // DANH SÁCH THÔNG BÁO
        // ================================
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem thông báo.";
                return RedirectToAction("Index", "LichTap");
            }

            // lấy thông tin thành viên theo userId
            var tv = await _context.ThanhViens
                .FirstOrDefaultAsync(x => x.MaNguoiDung == userId);

            // lấy thông tin HLV nếu có
            var hlv = await _context.HuanLuyenViens
                .FirstOrDefaultAsync(x => x.MaNguoiDung == userId);

            // TRUY VẤN ĐÚNG CHO MỌI TRƯỜNG HỢP
            var list = await _context.ThongBaos
                .Where(tb =>
                       // nếu là thành viên → lấy thông báo gửi cho member
                       (tv != null && tb.MaThanhVien == tv.MaThanhVien)
                       ||
                       // nếu là HLV → lấy thông báo gửi cho HLV
                       (hlv != null && tb.MaHuanLuyenVien == hlv.MaHuanLuyenVien)
                       ||
                       // thông báo hệ thống chung
                       tb.LoaiThongBao == "HeThong"
                 )
                .OrderByDescending(tb => tb.NgayGui)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.TV = tv;
            ViewBag.HLV = hlv;

            return View(list);
        }

        // ================================
        // ĐÁNH DẤU TẤT CẢ ĐÃ ĐỌC
        // ================================
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập.";
                return RedirectToAction("Index", "LichTap");
            }

            var tv = await _context.ThanhViens
                .FirstOrDefaultAsync(t => t.MaNguoiDung == userId);

            var hlv = await _context.HuanLuyenViens
                .FirstOrDefaultAsync(t => t.MaNguoiDung == userId);

            var notifications = await _context.ThongBaos
                .Where(tb =>
                        (!tb.DaDoc) &&
                        (
                            (tv != null && tb.MaThanhVien == tv.MaThanhVien) ||
                            (hlv != null && tb.MaHuanLuyenVien == hlv.MaHuanLuyenVien)
                        )
                )
                .ToListAsync();

            if (notifications.Any())
            {
                notifications.ForEach(n => n.DaDoc = true);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
