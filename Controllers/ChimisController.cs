using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tp_Negocio.Data;
using Tp_Negocio.Models;
using Tp_Negocio.ViewModel;

namespace Tp_Negocio.Controllers
{
    
    public class ChimisController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ChimisController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Chimis
        [Authorize]
        public async Task<IActionResult> Index(string buscarNombre, int pagina = 1)
        {
            // Genero los filtros
            IQueryable<Chimi> appDbContext = _context.Chimis;
            if (!string.IsNullOrEmpty(buscarNombre))
            {
                appDbContext = appDbContext.Where(x => x.Nombre!.Contains(buscarNombre));
            }

            // Paginación
            int RegistrosPorPagina = 2;
            var registroMostrar = appDbContext
                                .Skip((pagina - 1) * RegistrosPorPagina)
                                .Take(RegistrosPorPagina);

            // Generar el modelo
            ChimiViewModel modelo = new ChimiViewModel()
            {
                Chimis = await registroMostrar.ToListAsync(),
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

        // GET: Chimis/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chimi = await _context.Chimis
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chimi == null)
            {
                return NotFound();
            }

            return View(chimi);
        }

        // GET: Chimis/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Chimis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Cantidad,Ingredientes,Stock,NombreFoto")] Chimi chimi)
        {
            if (ModelState.IsValid)
            {

                var archivos = HttpContext.Request.Form.Files;
                if (archivos != null && archivos.Count > 0)
                {
                    var archivoFoto = archivos[0];
                    if (archivoFoto.Length > 0)
                    {
                        var pathDestino = Path.Combine(_env.WebRootPath, "imagenes\\Chimis");

                        var archivoDestino = Guid.NewGuid().ToString().Replace("-", "");
                        var extension = Path.GetExtension(archivoFoto.FileName);
                        archivoDestino += extension;

                        using (var filestream = new FileStream(Path.Combine(pathDestino, archivoDestino), FileMode.Create))
                        {
                            archivoFoto.CopyTo(filestream);
                            chimi.NombreFoto = archivoDestino;
                        }
                    }
                }

                _context.Add(chimi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }   
            return View(chimi);
        }

        // GET: Chimis/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chimi = await _context.Chimis.FindAsync(id);
            if (chimi == null)
            {
                return NotFound();
            }
            return View(chimi);
        }

        // POST: Chimis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Cantidad,Ingredientes,Stock,NombreFoto")] Chimi chimi)
        {
            if (id != chimi.Id)
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
                        var pathDestino = Path.Combine(_env.WebRootPath, "imagenes\\Chimis");

                        var archivoDestino = Guid.NewGuid().ToString().Replace("-", "");
                        var extension = Path.GetExtension(archivoFoto.FileName);
                        archivoDestino += extension;

                        using (var filestream = new FileStream(Path.Combine(pathDestino, archivoDestino), FileMode.Create))
                        {
                            archivoFoto.CopyTo(filestream);
                            if (chimi.NombreFoto != null)
                            {
                                var archivoViejo = Path.Combine(pathDestino, chimi.NombreFoto!);
                                if (System.IO.File.Exists(archivoViejo))
                                {
                                    System.IO.File.Delete(archivoViejo);
                                }
                            }
                            chimi.NombreFoto = archivoDestino;
                        }

                    }
                }
                try
                {
                    _context.Update(chimi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChimiExists(chimi.Id))
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
            return View(chimi);
        }

        // GET: Chimis/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chimi = await _context.Chimis
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chimi == null)
            {
                return NotFound();
            }

            return View(chimi);
        }

        // POST: Chimis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chimi = await _context.Chimis.FindAsync(id);
            if (chimi != null)
            {
                _context.Chimis.Remove(chimi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChimiExists(int id)
        {
            return _context.Chimis.Any(e => e.Id == id);
        }
    }
}
