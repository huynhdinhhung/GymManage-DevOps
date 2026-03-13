using System;
using System.Threading.Tasks;
using GYM_Manage.Data;
using GYM_Manage.Models;
using GYM_Manage.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GYM_Manage.Controllers
{
    [Route("Auth")]
    public class AuthController : Controller
    {
        private readonly GYM_DBcontext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly IEmailService _email;

        public AuthController(GYM_DBcontext context, ILogger<AuthController> logger, IEmailService email)
        {
            _context = context;
            _logger = logger;
            _email = email;
        }

        // ============================== LOGIN (GET)
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        // ============================== LOGIN (POST)
        [HttpPost("Login")]
        public async Task<IActionResult> Login(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            var user = await _context.NguoiDungs
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.TenDangNhap == tenDangNhap);

            if (user == null || user.TrangThai != "HoatDong")
            {
                TempData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }

            string hashedPassword = PasswordHelper.HashPassword(matKhau);
            if (user.MatKhau != hashedPassword)
            {
                TempData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }

            var updateUser = await _context.NguoiDungs.FindAsync(user.MaNguoiDung);
            updateUser.LanDangNhapCuoi = DateTime.Now;
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("UserName", user.TenDangNhap);
            HttpContext.Session.SetString("UserRole", user.VaiTro);
            HttpContext.Session.SetInt32("UserId", user.MaNguoiDung);

            TempData["SuccessMessage"] = $"Chào mừng {user.HoTen}!";

            switch (user.VaiTro)
            {
                case "Admin":
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

                case "Staff":
                    return RedirectToAction("Index", "QL_ThanhVien", new { area = "Admin" });

                case "Trainer":
                    var hlv = await _context.HuanLuyenViens
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.MaNguoiDung == user.MaNguoiDung);

                    if (hlv == null)
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy thông tin huấn luyện viên.";
                        return RedirectToAction("Login");
                    }

                    HttpContext.Session.SetInt32("MaHuanLuyenVien", hlv.MaHuanLuyenVien);
                    return RedirectToAction("Index", "HLVDashboard", new { area = "HLV" });

                case "Member":
                default:
                    var thanhVien = await _context.ThanhViens
                        .FirstOrDefaultAsync(tv => tv.MaNguoiDung == user.MaNguoiDung);

                    if (thanhVien != null)
                    {
                        HttpContext.Session.SetInt32("MaThanhVien", thanhVien.MaThanhVien);
                    }

                    return RedirectToAction("Index", "Home");
            }
        }

        // ============================== QUÊN MẬT KHẨU (GET)
        [HttpGet("QuenMatKhau")]
        public IActionResult QuenMatKhau()
        {
            return View();
        }

        // ============================== GỬI OTP (POST)
        [HttpPost("SendOtp")]
        public async Task<IActionResult> SendOtp(string email)
        {
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                TempData["Error"] = "Email không tồn tại trong hệ thống!";
                return RedirectToAction("QuenMatKhau");
            }

            var otp = new Random().Next(100000, 999999).ToString();

            var otpCode = new OtpCode
            {
                Email = email,
                Code = otp,
                ExpiredAt = DateTime.Now.AddMinutes(5),
                Used = false
            };

            _context.OtpCodes.Add(otpCode);
            await _context.SaveChangesAsync();

            await _email.SendEmailAsync(email, "Mã OTP khôi phục mật khẩu",
                $"Mã OTP của bạn là: <b>{otp}</b>. Mã hết hạn trong 5 phút.");

            TempData["Success"] = "Mã OTP đã được gửi vào email.";
            return RedirectToAction("XacNhanOtp", new { email });
        }

        // ============================== XÁC NHẬN OTP (GET)
        [HttpGet("XacNhanOtp")]
        public IActionResult XacNhanOtp(string email)
        {
            return View(model: email);
        }

        // ============================== KIỂM TRA OTP (POST)
        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp(string email, string otp)
        {
            var otpCode = await _context.OtpCodes
                .Where(x => x.Email == email && !x.Used)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (otpCode == null || otpCode.Code != otp)
            {
                TempData["Error"] = "OTP không đúng!";
                return RedirectToAction("XacNhanOtp", new { email });
            }

            if (otpCode.ExpiredAt < DateTime.Now)
            {
                TempData["Error"] = "OTP đã hết hạn!";
                return RedirectToAction("XacNhanOtp", new { email });
            }

            otpCode.Used = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("DoiMatKhauMoi", new { email });
        }

        // ============================== ĐỔI MẬT KHẨU MỚI (GET)
        [HttpGet("DoiMatKhauMoi")]
        public IActionResult DoiMatKhauMoi(string email)
        {
            return View(model: email);
        }

        // ============================== CẬP NHẬT MẬT KHẨU (POST)
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string email, string newPassword)
        {
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
                return NotFound();

            user.MatKhau = PasswordHelper.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Login");
        }

        // ============================== REGISTER (GET)
        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        // ============================== REGISTER (POST)
        [HttpPost("Register")]
        public async Task<IActionResult> Register(NguoiDung model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin!";
                return View(model);
            }

            var emailExist = await _context.NguoiDungs
                .AnyAsync(x => x.Email == model.Email);

            if (emailExist)
            {
                TempData["ErrorMessage"] = "Email đã được sử dụng!";
                return View(model);
            }

            var userExist = await _context.NguoiDungs
                .AnyAsync(x => x.TenDangNhap == model.TenDangNhap);

            if (userExist)
            {
                TempData["ErrorMessage"] = "Tên đăng nhập đã tồn tại!";
                return View(model);
            }

            string hashedPw = PasswordHelper.HashPassword(model.MatKhau);

            var newUser = new NguoiDung
            {
                HoTen = model.HoTen,
                Email = model.Email,
                TenDangNhap = model.TenDangNhap,
                MatKhau = hashedPw,
                VaiTro = "Member",
                TrangThai = "HoatDong",
                NgayTao = DateTime.Now
            };

            _context.NguoiDungs.Add(newUser);
            await _context.SaveChangesAsync();

            await _email.SendEmailAsync(
                model.Email,
                "Chào mừng bạn đến với Gym!",
                $"Xin chào <b>{model.HoTen}</b>, bạn đã đăng ký tài khoản thành công!"
            );

            TempData["SuccessMessage"] = "Đăng ký thành công! Bạn có thể đăng nhập.";
            return RedirectToAction("Login");
        }

        // ============================== ĐĂNG XUẤT
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Đăng xuất thành công.";
            return RedirectToAction("Login");
        }
    }
}
