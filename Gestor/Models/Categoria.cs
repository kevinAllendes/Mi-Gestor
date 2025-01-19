using System.ComponentModel.DataAnnotations;

namespace Gestor.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(maximumLength:50, ErrorMessage ="No puede ser mayor a {1}")]
        public string Nombre { get; set; }
        public TipoOperacion TipoOperacion { get; set; }
        public int UsuarioId { get; set; }
    }
}
