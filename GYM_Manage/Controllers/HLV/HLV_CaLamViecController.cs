using GYM_Manage.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GYM_Manage.Controllers.HLV
{
    [Area("HLV")]
    public class HLV_CaLamViecController : Controller
    {
        private readonly GYM_DBcontext _context;

        public HLV_CaLamViecController(GYM_DBcontext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var caLamViecs = await _context.CaLamViecs
                .OrderBy(c => c.GioBatDau)
                .ToListAsync();

            return View("~/Views/HLV/CaLamViec/Index.cshtml", caLamViecs);
        }
    }
}
