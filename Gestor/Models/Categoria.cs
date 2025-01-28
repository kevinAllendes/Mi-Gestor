using System.ComponentModel.DataAnnotations;

namespace Gestor.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(maximumLength:50, ErrorMessage ="No puede ser mayor a {1}")]
        public string Nombre { get; set; }
        [Display(Name ="Tipo de Categoria")]
        public TipoOperacion TipoOperacionId { get; set; }
        public int UsuarioId { get; set; }
    }
}
