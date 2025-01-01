using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gestor.Models
{
    public class CuentaCreacionViewModel: Cuenta
    {
        public IEnumerable<SelectListItem> TiposCuenta { get; set; }
    }
}
