using System;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tp_Negocio.Data;
using Tp_Negocio.Models;

namespace Tp_Negocio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ImportarExelChimiController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ImportarExelChimiController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SubirExcel(IFormFile excel)
        {
            try
            {
                var workbook = new XLWorkbook(excel.OpenReadStream());
                var hoja = workbook.Worksheet(1);
                var primeraFila = hoja.FirstRowUsed().RangeAddress.FirstAddress.RowNumber;
                var ultimaFila = hoja.LastRowUsed().RangeAddress.LastAddress.RowNumber;

                List<Chimi> chimis = new List<Chimi>();
                for (int i = primeraFila; i <= ultimaFila; i++)
                {
                    var fila = hoja.Row(i);
                    Chimi chimi = new Chimi();
                    chimi.Nombre = fila.Cell(1).GetString();
                    chimi.Cantidad = fila.Cell(2).GetString();
                    chimi.Ingredientes = fila.Cell(3).GetString();
                    chimi.Stock = fila.Cell(4).GetValue<int>();
                    chimis.Add(chimi);
                }
                _context.Chimis.AddRange(chimis);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return RedirectToAction("Index", "Chimis");
        }
    }
}