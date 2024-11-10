using System;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tp_Negocio.Data;
using Tp_Negocio.Models;

namespace Tp_Negocio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ImportarExelSalController : Controller
    {

        private readonly ApplicationDbContext _context;
        public ImportarExelSalController(ApplicationDbContext context)
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

                List<Sal> sales = new List<Sal>();
                for (int i = primeraFila ; i <= ultimaFila; i++)
                {
                    var fila = hoja.Row(i);
                    Sal sal = new Sal();
                    sal.Nombre = fila.Cell(1).GetString();
                    sal.Cantidad = fila.Cell(2).GetString();
                    sal.Ingredientes = fila.Cell(3).GetString();
                    sal.Stock = fila.Cell(4).GetValue<int>();
                    sales.Add(sal);
                }
                _context.sales.AddRange(sales);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return RedirectToAction("Index", "Sales");
        }
    }
}
