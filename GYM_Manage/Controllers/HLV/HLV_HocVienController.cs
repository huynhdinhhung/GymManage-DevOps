using GYM_Manage.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_Manage.Controllers.HLV
{
    [Area("HLV")]
    public class HLV_HocVienController : Controller
    {
        private readonly GYM_DBcontext _context;

        public HLV_HocVienController(GYM_DBcontext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? idHLV = HttpContext.Session.GetInt32("MaHuanLuyenVien");
            if (idHLV == null)
                return RedirectToAction("Login", "Auth");

            var hocViens = await _context.DangKyGoiTaps
                .Where(x => x.MaHuanLuyenVien == idHLV)

                // Load Thành viên + Người dùng
                .Include(x => x.ThanhVien)
                    .ThenInclude(tv => tv.NguoiDung)

                // Load gói tập
                .Include(x => x.GoiTap)

                // Load ca tập đã đăng ký
                .Include(x => x.CaLamViec)

                // Load lịch tập của học viên (nếu có)
                .Include(x => x.ThanhVien)
                    .ThenInclude(tv => tv.LichTapThanhViens)
                        .ThenInclude(lt => lt.CaLamViec)

                .OrderByDescending(x => x.NgayBatDau)
                .AsNoTracking()
                .ToListAsync();

            return View("~/Views/HLV/HocVien/Index.cshtml", hocViens);
        }
    }
}
