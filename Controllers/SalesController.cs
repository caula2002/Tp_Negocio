using System;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tp_Negocio.Data;
using Tp_Negocio.Models;
using Tp_Negocio.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace Tp_Negocio.Controllers
{
    
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SalesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Sales
        [Authorize]
        public async Task<IActionResult> Index(string buscarNombre, int pagina = 1)
        {
            // Genero los filtros
            IQueryable<Sal> appDbContext = _context.sales;
            if (!string.IsNullOrEmpty(buscarNombre))
            {
                appDbContext = appDbContext.Where(x => x.Nombre!.Contains(buscarNombre));
            }

            // Paginación
            int RegistrosPorPagina = 4;
            var registroMostrar = appDbContext
                                .Skip((pagina - 1) * RegistrosPorPagina)
                                .Take(RegistrosPorPagina);

            // Generar el modelo
            SalViewModel modelo = new SalViewModel()
            {
                sales = await registroMostrar.ToListAsync(),
                buscarNombre = buscarNombre
            };


            modelo.Paginador.PaginaActual = pagina;
            modelo.Paginador.RegistrosPorPagina = RegistrosPorPagina;
            modelo.Paginador.TotalRegistros = await appDbContext.CountAsync();

            if (!string.IsNullOrEmpty(buscarNombre))
            {
                modelo.Paginador.ValoresQueryString.Add("buscarNombre", buscarNombre);
            }

            return View(modelo);
        }
        // GET: Sales/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sal = await _context.sales
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sal == null)
            {
                return NotFound();
            }

            return View(sal);
        }
        // GET: Sales/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Cantidad,Ingredientes,Stock,NombreFoto")] Sal sal)
        {
            if (ModelState.IsValid)
            {
            
                    
                        var archivos = HttpContext.Request.Form.Files;
                        if (archivos != null && archivos.Count > 0)
                        {
                            var archivoFoto = archivos[0];
                            if (archivoFoto.Length > 0)
                            {
                                var pathDestino = Path.Combine(_env.WebRootPath, "imagenes\\sales");

                                var archivoDestino = Guid.NewGuid().ToString().Replace("-", "");
                                var extension = Path.GetExtension(archivoFoto.FileName);
                                archivoDestino += extension;

                                using (var filestream = new FileStream(Path.Combine(pathDestino, archivoDestino), FileMode.Create))
                                {
                                    archivoFoto.CopyTo(filestream);
                                    sal.NombreFoto = archivoDestino;
                                }
                            }
                        }

                        _context.Add(sal);
                        await _context.SaveChangesAsync();
                     return RedirectToAction(nameof(Index));
                
            }
            return View(sal);
        }

        // GET: Sales/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sal = await _context.sales.FindAsync(id);
            if (sal == null)
            {
                return NotFound();
            }
            return View(sal);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Cantidad,Ingredientes,Stock,NombreFoto")] Sal sal)
        {
            if (id != sal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var archivos = HttpContext.Request.Form.Files;
                if (archivos != null && archivos.Count > 0)
                {
                    var archivoFoto = archivos[0];
                    if (archivoFoto.Length > 0)
                    {
                        var pathDestino = Path.Combine(_env.WebRootPath, "imagenes\\sales");

                        var archivoDestino = Guid.NewGuid().ToString().Replace("-", "");
                        var extension = Path.GetExtension(archivoFoto.FileName);
                        archivoDestino += extension;

                        using (var filestream = new FileStream(Path.Combine(pathDestino, archivoDestino), FileMode.Create))
                        {
                            archivoFoto.CopyTo(filestream);
                            if (sal.NombreFoto != null)
                            {
                                var archivoViejo = Path.Combine(pathDestino, sal.NombreFoto!);
                                if (System.IO.File.Exists(archivoViejo))
                                {
                                    System.IO.File.Delete(archivoViejo);
                                }
                            }
                            sal.NombreFoto = archivoDestino;
                        }

                    }
                }
                try
                {
                    _context.Update(sal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalExists(sal.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sal);
        }
        // GET: Sales/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sal = await _context.sales
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sal == null)
            {
                return NotFound();
            }

            return View(sal);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sal = await _context.sales.FindAsync(id);
            if (sal != null)
            {
                _context.sales.Remove(sal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalExists(int id)
        {
            return _context.sales.Any(e => e.Id == id);
        }
    }
}
