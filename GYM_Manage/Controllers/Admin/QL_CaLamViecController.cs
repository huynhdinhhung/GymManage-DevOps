using GYM_Manage.Data;
using GYM_Manage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GYM_Manage.Controllers.Admin
{
    [Area("Admin")]
    public class QL_CaLamViecController : Controller
    {
        private readonly GYM_DBcontext _context;

        public QL_CaLamViecController(GYM_DBcontext context)
        {
            _context = context;
        }

        // ===================== DANH SÁCH CA LÀM VIỆC =====================
        public async Task<IActionResult> Index(int? hlvId)
        {
            try
            {
                // Lấy tên HLV nếu truyền hlvId
                ViewBag.HLV = hlvId.HasValue
                    ? await _context.HuanLuyenViens.FindAsync(hlvId)
                    : null;

                var caLamViecs = await _context.CaLamViecs
                    .OrderBy(c => c.GioBatDau)
                    .AsNoTracking()
                    .ToListAsync();

                if (!caLamViecs.Any())
                    TempData["InfoMessage"] = "Chưa có ca làm việc nào trong hệ thống.";

                return View("~/Views/Admin/QL_HuanLuyenVien/CaLamViec.cshtml", caLamViecs);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải danh sách ca làm việc: {ex.Message}";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        // ===================== TẠO / SỬA =====================
        [HttpGet]
        public async Task<IActionResult> CreateOrEdit(int? id)
        {
            try
            {
                if (id == null)
                    return View("~/Views/Admin/QL_HuanLuyenVien/CreateOrEdit_CaLamViec.cshtml", new CaLamViec());

                var ca = await _context.CaLamViecs.FindAsync(id);

                if (ca == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy ca làm việc!";
                    return RedirectToAction(nameof(Index));
                }

                return View("~/Views/Admin/QL_HuanLuyenVien/CreateOrEdit_CaLamViec.cshtml", ca);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải dữ liệu ca làm việc: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(CaLamViec model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin nhập!";
                return View("~/Views/Admin/QL_HuanLuyenVien/CreateOrEdit_CaLamViec.cshtml", model);
            }

            try
            {
                bool isDuplicate = await _context.CaLamViecs
                    .AnyAsync(c => c.GioBatDau == model.GioBatDau &&
                                   c.GioKetThuc == model.GioKetThuc &&
                                   c.MaCa != model.MaCa);

                if (isDuplicate)
                {
                    TempData["ErrorMessage"] = "Ca làm việc bị trùng thời gian!";
                    return View("~/Views/Admin/QL_HuanLuyenVien/CreateOrEdit_CaLamViec.cshtml", model);
                }

                if (model.MaCa == 0)
                {
                    _context.CaLamViecs.Add(model);
                    TempData["SuccessMessage"] = "Thêm ca làm việc thành công!";
                }
                else
                {
                    _context.CaLamViecs.Update(model);
                    TempData["SuccessMessage"] = "Cập nhật ca làm việc thành công!";
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi lưu ca làm việc: {ex.Message}";
                return View("~/Views/Admin/QL_HuanLuyenVien/CreateOrEdit_CaLamViec.cshtml", model);
            }
        }

        // ===================== XÓA =====================
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ca = await _context.CaLamViecs.FindAsync(id);
                if (ca == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy ca làm việc cần xóa!";
                    return RedirectToAction(nameof(Index));
                }

                _context.CaLamViecs.Remove(ca);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa ca làm việc thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa ca làm việc: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // ===================== AJAX PARTIAL =====================
        [HttpGet]
        public async Task<IActionResult> GetCaLamViecPartial()
        {
            var caLamViecs = await _context.CaLamViecs
                .OrderBy(c => c.GioBatDau)
                .AsNoTracking()
                .ToListAsync();

            return PartialView("~/Views/Admin/QL_HuanLuyenVien/_CaLamViec.cshtml", caLamViecs);
        }
    }
}
