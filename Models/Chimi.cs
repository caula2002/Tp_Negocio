using System.ComponentModel.DataAnnotations;

namespace Tp_Negocio.Models
{
    public class Chimi
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }

        public string? Cantidad { get; set; }

        public string? Ingredientes { get; set; }
        public int Stock { get; set; }
        [Display(Name = "Foto")]
        public string? NombreFoto { get; set; }
    }
}
