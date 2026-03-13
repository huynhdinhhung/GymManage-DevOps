using GYM_Manage.Data;
using GYM_Manage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using GYM_Manage.Services;
using Microsoft.AspNetCore.Http;

namespace GYM_Manage.Controllers
{
    public class LichTapController : Controller
    {
        private readonly GYM_DBcontext _context;
        private readonly IEmailService _email;

        public LichTapController(GYM_DBcontext context, IEmailService email)
        {
            _context = context;
            _email = email;
        }

        // ============================================
        //  API: ĐẶT LỊCH GỬI EMAIL
        // ============================================
        [HttpPost]
        public IActionResult DatLichGuiEmail(string to, string subject, string body, DateTime sendTime)
        {
            if (string.IsNullOrEmpty(to))
            {
                TempData["Error"] = "Email nhận không hợp lệ!";
                return RedirectToAction("Index");
            }

            var sendUtc = TimeZoneInfo.ConvertTimeToUtc(sendTime, TimeZoneInfo.Local);

            BackgroundJob.Schedule(
                () => _email.SendEmailAsync(to, subject, body),
                sendUtc
            );

            TempData["Success"] = $"Đã đặt lịch gửi email lúc {sendTime:HH:mm dd/MM/yyyy}";
            return RedirectToAction("Index");
        }

        // ============================================
        //  TRANG LỊCH TẬP
        // ============================================
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            var today = DateTime.Today;

            // ==================================================
            // 1️ CHƯA ĐĂNG NHẬP → LỊCH CHUNG
            // ==================================================
            if (userId == null)
            {
                var lichThuong = await _context.LichLamViecHLVs
                    .Include(x => x.HuanLuyenVien)
                    .Include(x => x.CaLamViec)
                    .Where(x => x.NgayLamViec.Date >= today)
                    .OrderBy(x => x.NgayLamViec)
                    .ThenBy(x => x.MaCa)
                    .ToListAsync();

                return View("LichTapThuong", lichThuong);
            }

            // ==================================================
            // 2️ LẤY THÔNG TIN THÀNH VIÊN
            // ==================================================
            var tv = await _context.ThanhViens
                .Include(t => t.DangKyGoiTaps)
                .FirstOrDefaultAsync(t => t.MaNguoiDung == userId);

            if (tv == null)
            {
                var lichThuong = await _context.LichLamViecHLVs
                    .Include(x => x.HuanLuyenVien)
                    .Include(x => x.CaLamViec)
                    .Where(x => x.NgayLamViec.Date >= today)
                    .OrderBy(x => x.NgayLamViec)
                    .ThenBy(x => x.MaCa)
                    .ToListAsync();

                return View("LichTapThuong", lichThuong);
            }

            // ==================================================
            // 3️ THÔNG BÁO
            // ==================================================
            var thongBao = await _context.ThongBaos
                .Where(tb => tb.MaThanhVien == tv.MaThanhVien)
                .OrderByDescending(tb => tb.NgayGui)
                .ToListAsync();

            ViewBag.ThongBao = thongBao;
            ViewBag.ThongBaoMoi = thongBao.Count(tb => !tb.DaDoc);

            // ==================================================
            // 4️ GÓI TẬP ĐANG DÙNG
            // ==================================================
            var goiDangKy = tv.DangKyGoiTaps
                .OrderByDescending(x => x.NgayBatDau)
                .FirstOrDefault();

            if (goiDangKy == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa đăng ký gói tập nào.";
                return View("KhongCoLich");
            }

            // ==================================================
            // 5️ THÀNH VIÊN VIP → LỊCH VIP
            // ==================================================
            if (goiDangKy.MaHuanLuyenVien != null)
            {
                var lichVip = await _context.LichTapThanhViens
                    .Include(x => x.HuanLuyenVien)
                    .Include(x => x.HuanLuyenVienThayThe)
                    .Include(x => x.CaLamViec)
                    .Where(x =>
                        x.MaThanhVien == tv.MaThanhVien &&
                        x.NgayTap.Date >= today
                    )
                    .OrderBy(x => x.NgayTap)
                    .ThenBy(x => x.MaCa)
                    .ToListAsync();

                return View("LichTapVip", lichVip);
            }

            // ==================================================
            // 6️ THÀNH VIÊN THƯỜNG → LỊCH CHUNG
            // ==================================================
            var lichThuongForMember = await _context.LichLamViecHLVs
                .Include(x => x.HuanLuyenVien)
                .Include(x => x.CaLamViec)
                .Where(x => x.NgayLamViec.Date >= today)
                .OrderBy(x => x.NgayLamViec)
                .ThenBy(x => x.MaCa)
                .ToListAsync();

            return View("LichTapThuong", lichThuongForMember);
        }
    }
}
