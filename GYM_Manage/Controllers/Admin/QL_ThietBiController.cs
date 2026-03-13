using GYM_Manage.Data;
using GYM_Manage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_Manage.Controllers.Admin
{
    [Area("Admin")]
    public class QL_ThietBiController : Controller
    {
        private readonly GYM_DBcontext _context;
        private readonly IWebHostEnvironment _env;

        public QL_ThietBiController(GYM_DBcontext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ================================
        // DANH SÁCH
        // ================================
        public async Task<IActionResult> Index()
        {
            var list = await _context.ThietBis
                .OrderBy(x => x.TenThietBi)
                .ToListAsync();

            return View("~/Views/Admin/QL_ThietBi/Index.cshtml", list);
        }


        // ================================
        // TÌM KIẾM
        // ================================
        public async Task<IActionResult> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập tên thiết bị.";
                return RedirectToAction(nameof(Index));
            }

            var list = await _context.ThietBis
                .Where(t => t.TenThietBi.Contains(keyword))
                .ToListAsync();

            if (!list.Any())
            {
                TempData["ErrorMessage"] = "Không tìm thấy thiết bị phù hợp.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = $"Tìm thấy {list.Count} thiết bị.";
            return View("~/Views/Admin/QL_ThietBi/Index.cshtml", list);
        }


        // ================================
        // CREATE
        // ================================
        public IActionResult Create()
        {
            return View("~/Views/Admin/QL_ThietBi/CreateOrEdit.cshtml", new ThietBi());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThietBi tb, IFormFile? hinhAnh)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/QL_ThietBi/CreateOrEdit.cshtml", tb);

            // Upload ảnh
            if (hinhAnh != null && hinhAnh.Length > 0)
            {
                string folder = Path.Combine(_env.WebRootPath, "images/thietbi");
                Directory.CreateDirectory(folder);

                string fileName = $"{Guid.NewGuid()}_{hinhAnh.FileName}";
                string path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                    await hinhAnh.CopyToAsync(stream);

                tb.HinhAnh = "/images/thietbi/" + fileName;
            }

            _context.ThietBis.Add(tb);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thêm thiết bị thành công!";
            return RedirectToAction(nameof(Index));
        }


        // ================================
        // EDIT
        // ================================
        public async Task<IActionResult> Edit(int id)
        {
            var tb = await _context.ThietBis.FindAsync(id);
            if (tb == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thiết bị.";
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Admin/QL_ThietBi/CreateOrEdit.cshtml", tb);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ThietBi tb, IFormFile? hinhAnh)
        {
            var old = await _context.ThietBis.AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaThietBi == id);

            if (old == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thiết bị.";
                return RedirectToAction(nameof(Index));
            }

            // Upload ảnh mới
            if (hinhAnh != null && hinhAnh.Length > 0)
            {
                string folder = Path.Combine(_env.WebRootPath, "images/thietbi");
                Directory.CreateDirectory(folder);

                // Xóa ảnh cũ
                if (!string.IsNullOrEmpty(old.HinhAnh))
                {
                    string oldPath = Path.Combine(_env.WebRootPath, old.HinhAnh.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                string fileName = $"{Guid.NewGuid()}_{hinhAnh.FileName}";
                string newPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                    await hinhAnh.CopyToAsync(stream);

                tb.HinhAnh = "/images/thietbi/" + fileName;
            }
            else
            {
                tb.HinhAnh = old.HinhAnh;
            }

            _context.ThietBis.Update(tb);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật thiết bị thành công!";
            return RedirectToAction(nameof(Index));
        }


        // ================================
        // DELETE
        // ================================
        public async Task<IActionResult> Delete(int id)
        {
            var tb = await _context.ThietBis.FindAsync(id);

            if (tb == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thiết bị.";
                return RedirectToAction(nameof(Index));
            }

            // Xóa ảnh
            if (!string.IsNullOrEmpty(tb.HinhAnh))
            {
                string path = Path.Combine(_env.WebRootPath, tb.HinhAnh.TrimStart('/'));
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            _context.ThietBis.Remove(tb);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã xóa thiết bị.";
            return RedirectToAction(nameof(Index));
        }


        private bool ThietBiExists(int id)
        {
            return _context.ThietBis.Any(e => e.MaThietBi == id);
        }
    }
}
