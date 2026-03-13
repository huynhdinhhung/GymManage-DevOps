using GYM_Manage.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GYM_Manage.Controllers.HLV
{
    [Area("HLV")]
    public class HLVDashboardController : Controller
    {
        private readonly GYM_DBcontext _context;

        public HLVDashboardController(GYM_DBcontext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var user = await _context.NguoiDungs
                .FirstOrDefaultAsync(x => x.MaNguoiDung == userId);

            if (user == null)
                return RedirectToAction("Login", "Auth");

            var hlv = await _context.HuanLuyenViens
                .Include(x => x.NguoiDung)
                .FirstOrDefaultAsync(x => x.MaNguoiDung == user.MaNguoiDung);

            if (hlv == null)
                return Content("Không tìm thấy thông tin HLV.");

            ViewBag.HLV = hlv;

            int hlvId = hlv.MaHuanLuyenVien;

            // === Dashboard Stats ===
            ViewBag.SoLich = await _context.LichLamViecHLVs
                .CountAsync(x => x.MaHuanLuyenVien == hlvId);

            ViewBag.SoYeuCauGui = await _context.YeuCauDoiLichs
                .CountAsync(x => x.MaHlvGui == hlvId);

            // Sửa trạng thái sai "ChoDuyet" -> "ChoHLV"
            ViewBag.SoYeuCauNhan = await _context.YeuCauDoiLichs
                .CountAsync(x => x.MaHlvNhan == hlvId && x.TrangThai == "ChoHLV");

            // Sửa sai thuộc tính DaDoc -> DaXem
            ViewBag.SoThongBao = await _context.ThongBaos
                .CountAsync(x => x.MaHuanLuyenVien == hlvId && !x.DaXem);

            return View("~/Views/HLV/Dashboard/Index.cshtml");
        }
    }
}
