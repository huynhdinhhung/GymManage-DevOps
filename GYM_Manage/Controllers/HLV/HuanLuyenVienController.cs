using GYM_Manage.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


public class HuanLuyenVienController : Controller
{
    private readonly GYM_DBcontext _context;

    public HuanLuyenVienController(GYM_DBcontext context)
    {
        _context = context;
    }

    public IActionResult Details(int id)
    {
        var hlv = _context.HuanLuyenViens
            .Include(x => x.NguoiDung)
            .FirstOrDefault(x => x.MaHuanLuyenVien == id); // SỬA Ở ĐÂY!!!

        if (hlv == null)
            return NotFound();

        return View(hlv);
    }
}
