using GYM_Manage.Data;
using GYM_Manage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System;
using GYM_Manage.Helpers;

namespace GYM_Manage.Controllers.Admin
{
    [Area("Admin")]
    public class QL_HuanLuyenVienController : Controller
    {
        private readonly GYM_DBcontext _context;
        private readonly IWebHostEnvironment _env;

        public QL_HuanLuyenVienController(GYM_DBcontext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ==============================================================
        // DANH SÁCH HLV
        // ==============================================================
        public async Task<IActionResult> Index()
        {
            var list = await _context.HuanLuyenViens
                .Include(h => h.NguoiDung)
                .AsNoTracking()
                .OrderBy(h => h.HoTen)
                .ToListAsync();

            // Đếm yêu cầu đổi lịch đang chờ ADMIN duyệt
            ViewBag.YeuCauCounts = await _context.YeuCauDoiLichs
                .Where(x => x.TrangThai == "ChoAdmin")
                .GroupBy(x => x.MaHlvGui)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

            return View("~/Views/Admin/QL_HuanLuyenVien/Index.cshtml", list);
        }


        // ==============================================================
        // TÌM KIẾM
        // ==============================================================
        public async Task<IActionResult> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập từ khóa.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _context.HuanLuyenViens
                .Include(h => h.NguoiDung)
                .Where(h =>
                    h.HoTen.Contains(keyword) ||
                    (h.ChuyenMon ?? "").Contains(keyword) ||
                    (h.NguoiDung != null && h.NguoiDung.HoTen.Contains(keyword))
                )
                .ToListAsync();

            if (!result.Any())
            {
                TempData["ErrorMessage"] = "Không tìm thấy kết quả nào.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = $"Tìm thấy {result.Count} kết quả.";
            return View("~/Views/Admin/QL_HuanLuyenVien/Index.cshtml", result);
        }


        // ==============================================================
        // TẠO MỚI HLV
        // ==============================================================
        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Admin/QL_HuanLuyenVien/CreateOrEdit.cshtml", new HuanLuyenVien());
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            HuanLuyenVien hlv,
            IFormFile anhDaiDien,
            string tenDangNhap,
            string email,
            string matKhau)
        {
            ModelState.Remove("NguoiDung");
            ModelState.Remove("MaNguoiDung");

            if (!ModelState.IsValid)
                return View("~/Views/Admin/QL_HuanLuyenVien/CreateOrEdit.cshtml", hlv);

            // Check username trùng
            if (await _context.NguoiDungs.AnyAsync(x => x.TenDangNhap == tenDangNhap))
            {
                TempData["ErrorMessage"] = "Tên đăng nhập đã tồn tại.";
                return View("~/Views/Admin/QL_HuanLuyenVien/CreateOrEdit.cshtml", hlv);
            }

            var user = new NguoiDung
            {
                TenDangNhap = tenDangNhap,
                Email = email,
                MatKhau = PasswordHelper.HashPassword(matKhau),
                HoTen = hlv.HoTen,
                VaiTro = "Trainer",
                TrangThai = "HoatDong",
                NgayTao = DateTime.Now
            };

            _context.NguoiDungs.Add(user);
            await _context.SaveChangesAsync();

            hlv.MaNguoiDung = user.MaNguoiDung;

            // Upload ảnh
            if (anhDaiDien != null && anhDaiDien.Length > 0)
            {
                string folder = Path.Combine(_env.WebRootPath, "images", "huanluyenvien");
                Directory.CreateDirectory(folder);

                string fileName = $"{Guid.NewGuid()}_{anhDaiDien.FileName}";
                string path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                    await anhDaiDien.CopyToAsync(stream);

                hlv.AnhDaiDien = $"/images/huanluyenvien/{fileName}";
            }

            _context.HuanLuyenViens.Add(hlv);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thêm HLV thành công.";
            return RedirectToAction(nameof(Index));
        }


        // ==============================================================
        // CHỈNH SỬA
        // ==============================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var hlv = await _context.HuanLuyenViens.FindAsync(id);

            if (hlv == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy HLV.";
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Admin/QL_HuanLuyenVien/CreateOrEdit.cshtml", hlv);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(HuanLuyenVien hlv, IFormFile anhDaiDien)
        {
            var old = await _context.HuanLuyenViens.AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaHuanLuyenVien == hlv.MaHuanLuyenVien);

            if (old == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy HLV.";
                return RedirectToAction(nameof(Index));
            }

            // Upload ảnh mới
            if (anhDaiDien != null && anhDaiDien.Length > 0)
            {
                string folder = Path.Combine(_env.WebRootPath, "images/huanluyenvien");
                Directory.CreateDirectory(folder);

                // Xoá ảnh cũ
                if (!string.IsNullOrEmpty(old.AnhDaiDien))
                {
                    string oldPath = Path.Combine(_env.WebRootPath, old.AnhDaiDien.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                string fileName = $"{Guid.NewGuid()}_{anhDaiDien.FileName}";
                string newPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                    await anhDaiDien.CopyToAsync(stream);

                hlv.AnhDaiDien = "/images/huanluyenvien/" + fileName;
            }
            else
            {
                hlv.AnhDaiDien = old.AnhDaiDien;
            }

            _context.HuanLuyenViens.Update(hlv);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật thành công.";
            return RedirectToAction(nameof(Index));
        }


        // ==============================================================
        // XÓA
        // ==============================================================
        public async Task<IActionResult> Delete(int id)
        {
            var hlv = await _context.HuanLuyenViens.FindAsync(id);

            if (hlv == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy HLV.";
                return RedirectToAction(nameof(Index));
            }

            if (await _context.LichLamViecHLVs.AnyAsync(x => x.MaHuanLuyenVien == id))
            {
                TempData["ErrorMessage"] = "HLV đang có lịch, không thể xóa.";
                return RedirectToAction(nameof(Index));
            }

            // Xóa ảnh
            if (!string.IsNullOrEmpty(hlv.AnhDaiDien))
            {
                string path = Path.Combine(_env.WebRootPath, hlv.AnhDaiDien.TrimStart('/'));
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            _context.HuanLuyenViens.Remove(hlv);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa thành công.";
            return RedirectToAction(nameof(Index));
        }


        // ==============================================================
        // YÊU CẦU ĐỔI LỊCH
        // ==============================================================
        public async Task<IActionResult> YeuCauDoiLich()
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


        [HttpPost]
        public async Task<IActionResult> Duyet(int id)
        {
            var yc = await _context.YeuCauDoiLichs
                .Include(x => x.LichGui)
                .Include(x => x.LichNhan)
                .FirstOrDefaultAsync(x => x.MaYeuCau == id);

            if (yc == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy yêu cầu.";
                return RedirectToAction(nameof(YeuCauDoiLich));
            }

            if (yc.LichNhan == null)
            {
                TempData["ErrorMessage"] = "HLV nhận không có lịch để đổi.";
                return RedirectToAction(nameof(YeuCauDoiLich));
            }

            // Hoán đổi
            int temp = yc.LichGui.MaHuanLuyenVien;
            yc.LichGui.MaHuanLuyenVien = yc.LichNhan.MaHuanLuyenVien;
            yc.LichNhan.MaHuanLuyenVien = temp;

            yc.TrangThai = "DaDuyet";
            yc.NgayXuLy = DateTime.Now;

            _context.LichLamViecHLVs.Update(yc.LichGui);
            _context.LichLamViecHLVs.Update(yc.LichNhan);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã duyệt yêu cầu.";
            return RedirectToAction(nameof(YeuCauDoiLich));
        }


        [HttpPost]
        public async Task<IActionResult> TuChoi(int id)
        {
            var yc = await _context.YeuCauDoiLichs.FindAsync(id);

            if (yc == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy yêu cầu.";
                return RedirectToAction(nameof(YeuCauDoiLich));
            }

            yc.TrangThai = "TuChoi";
            yc.NgayXuLy = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã từ chối yêu cầu.";
            return RedirectToAction(nameof(YeuCauDoiLich));
        }
    }
}
