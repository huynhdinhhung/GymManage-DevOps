using GYM_Manage.Data;
using GYM_Manage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GYM_Manage.Controllers.Admin
{
    [Area("Admin")]
    public class QL_BaiVietController : Controller
    {
        private readonly GYM_DBcontext _context;
        private readonly IWebHostEnvironment _env;

        public QL_BaiVietController(GYM_DBcontext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index()
        {
            var list = await _context.BaiViets
                .Include(x => x.NguoiTao)
                .OrderByDescending(x => x.NgayDang)
                .ToListAsync();

            return View("~/Views/Admin/QL_BaiViet/Index.cshtml", list);
        }

        // ================= DETAILS =================
        public async Task<IActionResult> Details(int id)
        {
            var model = await _context.BaiViets
                .Include(x => x.NguoiTao)
                .FirstOrDefaultAsync(x => x.MaBaiViet == id);

            if (model == null) return NotFound();

            return View("~/Views/Admin/QL_BaiViet/Details.cshtml", model);
        }

        // ================= CREATE GET =================
        public IActionResult Create()
        {
            return View("~/Views/Admin/QL_BaiViet/CreateOrEdit.cshtml", new BaiViet());
        }

        // ================= CREATE POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BaiViet model, IFormFile hinhAnh)
        {
            ModelState.Remove("NguoiTao");
            ModelState.Remove("HinhAnh");

            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/QL_BaiViet/CreateOrEdit.cshtml", model);
            }

            model.NgayDang = DateTime.Now;
            model.IDNguoiTao = 1; // ✅ ADMIN

            if (hinhAnh != null && hinhAnh.Length > 0)
            {
                string folder = Path.Combine(_env.WebRootPath, "images", "baiViet");
                Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid() + "_" + hinhAnh.FileName;
                string path = Path.Combine(folder, fileName);

                using var fs = new FileStream(path, FileMode.Create);
                await hinhAnh.CopyToAsync(fs);

                model.HinhAnh = "/images/baiViet/" + fileName;
            }

            _context.BaiViets.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng bài viết thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT GET =================
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.BaiViets.FindAsync(id);
            if (model == null) return RedirectToAction(nameof(Index));

            return View("~/Views/Admin/QL_BaiViet/CreateOrEdit.cshtml", model);
        }

        // ================= EDIT POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BaiViet input, IFormFile hinhAnh)
        {
            ModelState.Remove("NguoiTao");
            ModelState.Remove("HinhAnh");

            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/QL_BaiViet/CreateOrEdit.cshtml", input);
            }

            var model = await _context.BaiViets.FirstOrDefaultAsync(x => x.MaBaiViet == id);
            if (model == null) return NotFound();

            model.TieuDe = input.TieuDe;
            model.MoTaNgan = input.MoTaNgan;
            model.NoiDung = input.NoiDung;
            model.TrangThai = input.TrangThai;
            model.NgayCapNhat = DateTime.Now;

            // ❗ KHÔNG ĐỤNG IDNguoiTao

            if (hinhAnh != null && hinhAnh.Length > 0)
            {
                string folder = Path.Combine(_env.WebRootPath, "images", "baiViet");
                Directory.CreateDirectory(folder);

                if (!string.IsNullOrEmpty(model.HinhAnh))
                {
                    string oldPath = Path.Combine(_env.WebRootPath, model.HinhAnh.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                string fileName = Guid.NewGuid() + "_" + hinhAnh.FileName;
                string path = Path.Combine(folder, fileName);

                using var fs = new FileStream(path, FileMode.Create);
                await hinhAnh.CopyToAsync(fs);

                model.HinhAnh = "/images/baiViet/" + fileName;
            }

            _context.Update(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật bài viết thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE =================
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.BaiViets.FindAsync(id);
            if (model == null) return RedirectToAction(nameof(Index));

            if (!string.IsNullOrEmpty(model.HinhAnh))
            {
                string path = Path.Combine(_env.WebRootPath, model.HinhAnh.TrimStart('/'));
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            _context.BaiViets.Remove(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa bài viết thành công!";
            return RedirectToAction(nameof(Index));
        }
        // ================= SEARCH =================
        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            var query = _context.BaiViets
                .Include(x => x.NguoiTao)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                    x.TieuDe.Contains(keyword) ||
                    x.MaBaiViet.ToString().Contains(keyword)
                );
            }

            var list = await query
                .OrderByDescending(x => x.NgayDang)
                .ToListAsync();

            // DÙNG LẠI VIEW INDEX
            return View("~/Views/Admin/QL_BaiViet/Index.cshtml", list);
        }

    }
}
