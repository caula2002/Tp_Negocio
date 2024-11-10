using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tp_Negocio.Data;
using Tp_Negocio.Models;

namespace Tp_Negocio.Controllers
{
    [Authorize]
    public class CatalogoController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        public CatalogoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new Catalogo
            {
                Chimis = _context.Chimis.ToList(),
                Sales = _context.sales.ToList()
            };
            return View(model);
        }
    }
}
