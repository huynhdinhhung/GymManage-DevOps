    using GYM_Manage.Data;
    using GYM_Manage.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Hosting;

    namespace GYM_Manage.Controllers.Admin
    {
        [Area("Admin")]
        public class QL_GoiTapController : Controller
        {
            private readonly GYM_DBcontext _context;
            private readonly IWebHostEnvironment _env;

            public QL_GoiTapController(GYM_DBcontext context, IWebHostEnvironment env)
            {
                _context = context;
                _env = env;
            }

            // ================================
            // HIỂN THỊ DANH SÁCH GÓI TẬP
            // ================================
            public async Task<IActionResult> Index()
            {
                // Seed giống bản cũ nhưng thêm LoaiGoi
                if (!await _context.GoiTaps.AnyAsync())
                {
                    var sample = new List<GoiTap>
                    {
                        new GoiTap { TenGoiTap = "Gói Cơ Bản", MoTa = "Phù hợp cho người mới", ThoiHan = 30, GiaTien = 500000, TrangThai = "HoatDong", LoaiGoi = "Thuong", AnhDemo = "/images/goiTap/goitap_coban.jpg" },
                        new GoiTap { TenGoiTap = "Gói Nâng Cao", MoTa = "Dành cho người có kinh nghiệm", ThoiHan = 30, GiaTien = 800000, TrangThai = "HoatDong", LoaiGoi = "Thuong", AnhDemo = "/images/goiTap/goitap_nangcao.jpg" },
                        new GoiTap { TenGoiTap = "Gói VIP", MoTa = "PT riêng & quyền lợi VIP", ThoiHan = 30, GiaTien = 1500000, TrangThai = "HoatDong", LoaiGoi = "VIP", AnhDemo = "/images/goiTap/goitap_vip.jpg" }
                    };

                    await _context.GoiTaps.AddRangeAsync(sample);
                    await _context.SaveChangesAsync();
                }

                var list = await _context.GoiTaps.ToListAsync();
                return View("~/Views/Admin/QL_GoiTap/Index.cshtml", list);
            }


            // ================================
            // TÌM KIẾM
            // ================================
            public async Task<IActionResult> Search(string keyword)
            {
                keyword ??= "";

                var query = _context.GoiTaps
                    .Where(g =>
                        g.TenGoiTap.Contains(keyword) ||
                        g.MoTa.Contains(keyword) ||
                        g.LoaiGoi.Contains(keyword)
                    );

                var result = await query.ToListAsync();

                if (result.Count == 0)
                    TempData["ErrorMessage"] = "Không tìm thấy gói tập phù hợp!";
                else
                    TempData["SuccessMessage"] = $"Đã tìm thấy {result.Count} gói tập phù hợp.";

                return View("~/Views/Admin/QL_GoiTap/Index.cshtml", result);
            }


            // ================================
            // CREATE - GET
            // ================================
            public IActionResult Create()
            {
                return View("~/Views/Admin/QL_GoiTap/CreateOrEdit.cshtml", new GoiTap());
            }


            // ================================
            // CREATE - POST
            // ================================
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(GoiTap goiTap, IFormFile? anhDemo)
            {
                ModelState.Remove("AnhDemo");

                if (!ModelState.IsValid)
                    return View("~/Views/Admin/QL_GoiTap/CreateOrEdit.cshtml", goiTap);

                // UPLOAD ẢNH
                if (anhDemo != null && anhDemo.Length > 0)
                {
                    string dir = Path.Combine(_env.WebRootPath, "images", "goiTap");
                    Directory.CreateDirectory(dir);

                    string fileName = Guid.NewGuid() + "_" + anhDemo.FileName;
                    string path = Path.Combine(dir, fileName);

                    using (var fs = new FileStream(path, FileMode.Create))
                        await anhDemo.CopyToAsync(fs);

                    goiTap.AnhDemo = "/images/goiTap/" + fileName;
                }

                _context.GoiTaps.Add(goiTap);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm gói tập thành công!";
                return RedirectToAction(nameof(Index));
            }


            // ================================
            // EDIT - GET
            // ================================
            public async Task<IActionResult> Edit(int id)
            {
                var goiTap = await _context.GoiTaps.FindAsync(id);
                if (goiTap == null) return NotFound();

                return View("~/Views/Admin/QL_GoiTap/CreateOrEdit.cshtml", goiTap);
            }


            // ================================
            // EDIT - POST
            // ================================
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, GoiTap goiTap, IFormFile? anhDemo)
            {
                if (id != goiTap.MaGoiTap) return NotFound();

                ModelState.Remove("AnhDemo");
                if (!ModelState.IsValid)
                    return View("~/Views/Admin/QL_GoiTap/CreateOrEdit.cshtml", goiTap);

                var old = await _context.GoiTaps.AsNoTracking().FirstOrDefaultAsync(x => x.MaGoiTap == id);
                if (old == null) return NotFound();

                // Nếu upload ảnh mới → xóa ảnh cũ
                if (anhDemo != null && anhDemo.Length > 0)
                {
                    if (!string.IsNullOrEmpty(old.AnhDemo))
                    {
                        string oldPath = Path.Combine(_env.WebRootPath, old.AnhDemo.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    // Lưu ảnh mới
                    string dir = Path.Combine(_env.WebRootPath, "images", "goiTap");
                    Directory.CreateDirectory(dir);

                    string fileName = Guid.NewGuid() + "_" + anhDemo.FileName;
                    string path = Path.Combine(dir, fileName);

                    using (var fs = new FileStream(path, FileMode.Create))
                        await anhDemo.CopyToAsync(fs);

                    goiTap.AnhDemo = "/images/goiTap/" + fileName;
                }
                else
                {
                    goiTap.AnhDemo = old.AnhDemo;
                }

                _context.Update(goiTap);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật gói tập thành công!";
                return RedirectToAction(nameof(Index));
            }


            // ================================
            // DELETE
            // ================================
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Delete(int id)
            {
                var entity = await _context.GoiTaps.FindAsync(id);
                if (entity == null)
                {
                    TempData["ErrorMessage"] = "Xoá thất bại! Không tìm thấy gói tập.";
                    return RedirectToAction(nameof(Index));
                }

                if (!string.IsNullOrEmpty(entity.AnhDemo))
                {
                    string path = Path.Combine(_env.WebRootPath, entity.AnhDemo.TrimStart('/'));
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                _context.GoiTaps.Remove(entity);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xoá gói tập thành công!";
                return RedirectToAction(nameof(Index));
            }
        }
    }
