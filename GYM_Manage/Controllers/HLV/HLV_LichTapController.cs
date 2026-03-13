using GYM_Manage.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_Manage.Controllers.HLV
{
    [Area("HLV")]
    public class HLV_LichTapController : Controller
    {
        private readonly GYM_DBcontext _context;

        public HLV_LichTapController(GYM_DBcontext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? idHLV = HttpContext.Session.GetInt32("MaHuanLuyenVien");
            if (idHLV == null)
                return RedirectToAction("Login", "Auth");

            var lich = await _context.LichTapThanhViens
                .Where(l => l.MaHuanLuyenVien == idHLV
                            || l.MaHuanLuyenVienThayThe == idHLV)
                .Include(l => l.ThanhVien)
                    .ThenInclude(tv => tv.NguoiDung)
                .Include(l => l.CaLamViec)
                .Include(l => l.HuanLuyenVien)
                .Include(l => l.HuanLuyenVienThayThe)
                .Include(l => l.DangKyGoiTap)
                .OrderBy(l => l.NgayTap)
                .ThenBy(l => l.MaCa)
                .AsNoTracking()
                .ToListAsync();

            return View("~/Views/HLV/LichTap/Index.cshtml", lich);
        }
    }
}
