using Microsoft.AspNetCore.Mvc.Rendering;
using Tp_Negocio.Models;


namespace Tp_Negocio.ViewModel
{
    public class SalViewModel
    {

        public List<Sal> sales { get; set; } = new List<Sal>();
        public string? buscarNombre { get; set; }
        public Paginador Paginador { get; set; } = new Paginador();
    }
    public class ChimiViewModel
    {
        public List<Chimi> Chimis { get; set; } = new List<Chimi>();
        public string? buscarNombre { get; set; }
        public Paginador Paginador { get; set; } = new Paginador();
    }
    public class Paginador
    {
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPagina => (int)Math.Ceiling((decimal)TotalRegistros / RegistrosPorPagina);
        public Dictionary<string, string> ValoresQueryString { get; set; } = new Dictionary<string, string>();
    }
    
}
