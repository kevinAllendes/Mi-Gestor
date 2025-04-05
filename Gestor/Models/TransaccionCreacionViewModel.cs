using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gestor.Models
{
    public class TransaccionCreacionViewModel: Transaccion
    {
        //Agrego dos enumerables a la clase heredada
        public IEnumerable<SelectListItem> Cuentas {get; set;}
        public IEnumerable<SelectListItem> Categorias {get; set;}

        
    }

}