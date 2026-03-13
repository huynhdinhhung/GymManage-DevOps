using GYM_Manage.Data;
using GYM_Manage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GYM_Manage.Controllers.HLV
{
    [Area("HLV")]
    public class HLV_ThongBaoController : Controller
    {
        private readonly GYM_DBcontext _context;

        public HLV_ThongBaoController(GYM_DBcontext context)
        {
            _context = context;
        }

        // ==============================
        // Lấy HLV đang đăng nhập
        // ==============================
        private async Task<HuanLuyenVien?> GetLoggedHLV()
        {
            int? idHLV = HttpContext.Session.GetInt32("MaHuanLuyenVien");
            if (idHLV == null) return null;

            return await _context.HuanLuyenViens
                .Include(h => h.NguoiDung)
                .FirstOrDefaultAsync(h => h.MaHuanLuyenVien == idHLV);
        }

        // ==============================
        // Danh sách thông báo
        // ==============================
        public async Task<IActionResult> Index()
        {
            var hlv = await GetLoggedHLV();
            if (hlv == null) return RedirectToAction("Login", "Auth");

            var list = await _context.ThongBaos
                .Where(x => x.MaHuanLuyenVien == hlv.MaHuanLuyenVien)
                .OrderByDescending(x => x.NgayGui)
                .ToListAsync();

            ViewBag.HLV = hlv;

            return View("~/Views/HLV/ThongBao/Index.cshtml", list);
        }

        // ==============================
        // Đánh dấu tất cả đã xem
        // ==============================
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var hlv = await GetLoggedHLV();
            if (hlv == null) return RedirectToAction("Login", "Auth");

            var list = await _context.ThongBaos
                .Where(x => x.MaHuanLuyenVien == hlv.MaHuanLuyenVien && !x.DaDoc)
                .ToListAsync();

            foreach (var tb in list)
                tb.DaDoc = true;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tất cả thông báo đã được đánh dấu là đã xem.";
            return RedirectToAction(nameof(Index));
        }

        // ==============================
        // Xóa 1 thông báo
        // ==============================
        public async Task<IActionResult> Delete(int id)
        {
            var hlv = await GetLoggedHLV();
            if (hlv == null) return RedirectToAction("Login", "Auth");

            var tb = await _context.ThongBaos
                .FirstOrDefaultAsync(x => x.MaThongBao == id && x.MaHuanLuyenVien == hlv.MaHuanLuyenVien);

            if (tb == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông báo.";
                return RedirectToAction(nameof(Index));
            }

            _context.ThongBaos.Remove(tb);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã xóa thông báo.";
            return RedirectToAction(nameof(Index));
        }
    }
}
