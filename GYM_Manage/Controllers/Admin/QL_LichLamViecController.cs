using GYM_Manage.Data;
using GYM_Manage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GYM_Manage.Controllers.Admin
{
    [Area("Admin")]
    public class QL_LichLamViecController : Controller
    {
        private readonly GYM_DBcontext _context;

        public QL_LichLamViecController(GYM_DBcontext context)
        {
            _context = context;
        }

        // =====================================================
        // DANH SÁCH LỊCH LÀM VIỆC
        // =====================================================
        public async Task<IActionResult> Index(int? hlvId)
        {
            try
            {
                var query = _context.LichLamViecHLVs
                    .Include(l => l.HuanLuyenVien)
                    .Include(l => l.CaLamViec)
                    .OrderBy(l => l.NgayLamViec)
                    .ThenBy(l => l.MaCa)
                    .AsQueryable();

                if (hlvId.HasValue)
                {
                    query = query.Where(l => l.MaHuanLuyenVien == hlvId.Value);
                    ViewBag.HLV = await _context.HuanLuyenViens.FindAsync(hlvId.Value);
                }

                var list = await query.AsNoTracking().ToListAsync();

                if (!list.Any())
                    TempData["InfoMessage"] = hlvId.HasValue
                        ? "Huấn luyện viên này chưa có lịch làm việc."
                        : "Chưa có lịch làm việc nào.";

                return View("~/Views/Admin/QL_HuanLuyenVien/LichLamViec.cshtml", list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Không thể tải danh sách: {ex.Message}";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        // =====================================================
        // TẠO / SỬA (GET)
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> CreateOrEdit(int? id, int? hlvId)
        {
            await LoadDropdownDataAsync();

            var model = id == null
                ? new LichLamViecHLV()
                : await _context.LichLamViecHLVs.FindAsync(id);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch!";
                return RedirectToAction(nameof(Index));
            }

            if (hlvId.HasValue && id == null)
                model.MaHuanLuyenVien = hlvId.Value;

            return View("~/Views/Admin/QL_HuanLuyenVien/CreateOrEdit_LichLamViec.cshtml", model);
        }

        // =====================================================
        // TẠO / SỬA (POST)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(LichLamViecHLV model)
        {
            await LoadDropdownDataAsync();

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return View("~/Views/Admin/QL_HuanLuyenVien/CreateOrEdit_LichLamViec.cshtml", model);
            }

            try
            {
                // Tính thứ trong tuần
                model.ThuTrongTuan = (int)model.NgayLamViec.DayOfWeek;
                if (model.ThuTrongTuan == 0) model.ThuTrongTuan = 7;

                // Kiểm tra trùng lịch
                bool existed = await _context.LichLamViecHLVs.AnyAsync(x =>
                    x.MaHuanLuyenVien == model.MaHuanLuyenVien &&
                    x.MaCa == model.MaCa &&
                    x.NgayLamViec.Date == model.NgayLamViec.Date &&
                    x.MaLich != model.MaLich);

                if (existed)
                {
                    TempData["ErrorMessage"] = "HLV đã có lịch trong ca này.";
                    return View("~/Views/Admin/QL_HuanLuyenVien/CreateOrEdit_LichLamViec.cshtml", model);
                }

                // Check lịch VIP
                bool hasVip = await _context.LichTapThanhViens.AnyAsync(tv =>
                    tv.MaHuanLuyenVien == model.MaHuanLuyenVien &&
                    tv.MaCa == model.MaCa &&
                    tv.NgayTap.Date == model.NgayLamViec.Date
                );

                if (hasVip)
                {
                    TempData["ErrorMessage"] = "Không thể chỉnh sửa lịch vì đang có học viên đăng ký!";
                    return View("~/Views/Admin/QL_HuanLuyenVien/CreateOrEdit_LichLamViec.cshtml", model);
                }

                // ================== CREATE ==================
                if (model.MaLich == 0)
                {
                    _context.LichLamViecHLVs.Add(model);
                    await _context.SaveChangesAsync();

                    await TaoThongBaoChoHLV(model.MaHuanLuyenVien, "Bạn đã được thêm lịch làm việc mới.");
                    TempData["SuccessMessage"] = "Thêm lịch thành công!";
                }
                else
                {
                    // ================== UPDATE ==================
                    var lich = await _context.LichLamViecHLVs.FindAsync(model.MaLich);
                    if (lich == null)
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy lịch!";
                        return RedirectToAction(nameof(Index));
                    }

                    lich.MaHuanLuyenVien = model.MaHuanLuyenVien;
                    lich.MaCa = model.MaCa;
                    lich.NgayLamViec = model.NgayLamViec;
                    lich.ThuTrongTuan = model.ThuTrongTuan;

                    await _context.SaveChangesAsync();

                    // Gửi thông báo cho HLV
                    await TaoThongBaChoHLVSua(model);

                    TempData["SuccessMessage"] = "Cập nhật lịch thành công!";
                }

                return RedirectToAction(nameof(Index), new { hlvId = model.MaHuanLuyenVien });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi lưu lịch: {ex.Message}";
                return View("~/Views/Admin/QL_HuanLuyenVien/CreateOrEdit_LichLamViec.cshtml", model);
            }
        }

        // =====================================================
        // XÓA LỊCH LÀM VIỆC
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var lich = await _context.LichLamViecHLVs.FindAsync(id);

                if (lich == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy lịch!";
                    return RedirectToAction(nameof(Index));
                }

                int hlvId = lich.MaHuanLuyenVien;

                // Lấy danh sách học viên bị ảnh hưởng
                var vipList = await _context.LichTapThanhViens
                    .Include(x => x.CaLamViec)
                    .Where(x =>
                        x.MaHuanLuyenVien == lich.MaHuanLuyenVien &&
                        x.MaCa == lich.MaCa &&
                        x.NgayTap.Date == lich.NgayLamViec.Date)
                    .ToListAsync();

                foreach (var vip in vipList)
                {
                    vip.IsHLVThayThe = true;
                    vip.MaHuanLuyenVienThayThe = null;

                    // Gửi thông báo cho học viên
                    _context.ThongBaos.Add(new ThongBao
                    {
                        MaThanhVien = vip.MaThanhVien,
                        LoaiThongBao = "HLVThayThe",
                        TieuDe = "Huấn luyện viên thay đổi",
                        NoiDung = $"Buổi tập ngày {vip.NgayTap:dd/MM/yyyy} ca {vip.CaLamViec?.TenCa} hiện chưa có huấn luyện viên. Chúng tôi sẽ sớm bố trí HLV thay thế.",
                        NgayGui = DateTime.Now,
                        IsImportant = true
                    });
                }

                _context.LichLamViecHLVs.Remove(lich);
                await _context.SaveChangesAsync();

                await TaoThongBaoChoHLV(hlvId, "Lịch làm việc của bạn đã bị xóa.");

                TempData["SuccessMessage"] = "Xóa lịch thành công!";
                return RedirectToAction(nameof(Index), new { hlvId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi xóa lịch: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }


        // =====================================================
        // LOAD DROPDOWN
        // =====================================================
        private async Task LoadDropdownDataAsync()
        {
            ViewBag.HLVList = new SelectList(
                await _context.HuanLuyenViens.OrderBy(h => h.HoTen).ToListAsync(),
                "MaHuanLuyenVien", "HoTen");

            ViewBag.CaList = new SelectList(
                await _context.CaLamViecs.OrderBy(c => c.TenCa).ToListAsync(),
                "MaCa", "TenCa");
        }

        // =====================================================
        // 🔔 TẠO THÔNG BÁO CHO HLV
        // =====================================================
        private async Task TaoThongBaoChoHLV(int maHLV, string noiDung)
        {
            _context.ThongBaos.Add(new ThongBao
            {
                MaHuanLuyenVien = maHLV,
                LoaiThongBao = "HeThong",
                TieuDe = "Lịch làm việc thay đổi",
                NoiDung = noiDung,
                NgayGui = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        // =====================================================
        // 🔔 THÔNG BÁO CHO HLV + CẢ HỌC VIÊN KHI UPDATE LỊCH
        // =====================================================
        private async Task TaoThongBaChoHLVSua(LichLamViecHLV model)
        {
            await TaoThongBaoChoHLV(model.MaHuanLuyenVien, "Lịch làm việc của bạn đã được cập nhật.");

            var vipList = await _context.LichTapThanhViens
                .Include(x => x.CaLamViec)
                .Where(x =>
                    x.MaHuanLuyenVien == model.MaHuanLuyenVien &&
                    x.MaCa == model.MaCa &&
                    x.NgayTap.Date == model.NgayLamViec.Date)
                .ToListAsync();

            foreach (var vip in vipList)
            {
                _context.ThongBaos.Add(new ThongBao
                {
                    MaThanhVien = vip.MaThanhVien,
                    LoaiThongBao = "HLVThayThe",
                    TieuDe = "Lịch tập thay đổi",
                    NoiDung = $"Buổi tập ngày {vip.NgayTap:dd/MM/yyyy} ca {vip.CaLamViec?.TenCa} có thay đổi từ phía HLV. Vui lòng kiểm tra lại lịch!",
                    NgayGui = DateTime.Now,
                    IsImportant = true
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
