using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tp_Negocio.Models;

namespace Tp_Negocio.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Chimi> Chimis { get; set; }
        public DbSet<Sal> sales { get; set; }
        public DbSet<Proveedores> proveedores { get; set; }
    }
}
