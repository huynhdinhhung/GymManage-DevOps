// GYM_Manage/Data/GYMContext.cs
using Microsoft.EntityFrameworkCore;
using GYM_Manage.Models;

namespace GYM_Manage.Data
{
    public class GYMContext : DbContext
    {
        public GYMContext(DbContextOptions<GYMContext> options) : base(options) { }

        public DbSet<GoiTap> GoiTaps { get; set; } = default!;
    }
}
