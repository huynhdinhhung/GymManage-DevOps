using GYM_Manage.Data;
using GYM_Manage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GYM_Manage.Controllers.HLV
{
    [Area("HLV")]
    public class HLV_LichLamViecController : Controller
    {
        private readonly GYM_DBcontext _context;

        public HLV_LichLamViecController(GYM_DBcontext context)
        {
            _context = context;
        }

        // ==============================
        // Lấy HLV đăng nhập
        // ==============================
        private async Task<HuanLuyenVien?> GetLoggedHLV()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return null;

            var hlv = await _context.HuanLuyenViens
                .Include(h => h.NguoiDung)
                .FirstOrDefaultAsync(h => h.MaNguoiDung == userId);

            return hlv;
        }


        // ==============================
        // LỊCH LÀM VIỆC THEO NGÀY
        // ==============================
        public async Task<IActionResult> Index()
        {
            var hlv = await GetLoggedHLV();
            if (hlv == null)
                return RedirectToAction("Login", "Auth");

            var list = await _context.LichLamViecHLVs
                .Include(x => x.CaLamViec)
                .Where(x => x.MaHuanLuyenVien == hlv.MaHuanLuyenVien)
                .OrderBy(x => x.NgayLamViec)
                .ThenBy(x => x.CaLamViec.GioBatDau)
                .ToListAsync();

            ViewBag.HLV = hlv;

            return View("~/Views/HLV/LichLamViec/Index.cshtml", list);
        }

        // ==============================
        // Chi tiết lịch
        // ==============================
        public async Task<IActionResult> ChiTiet(int id)
        {
            var lich = await _context.LichLamViecHLVs
                .Include(x => x.CaLamViec)
                .Include(x => x.HuanLuyenVien)
                .FirstOrDefaultAsync(x => x.MaLich == id);

            if (lich == null) return NotFound();

            return View("~/Views/HLV/LichLamViec/ChiTiet.cshtml", lich);
        }
    }
}
