using Gestor.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Gestor.Models;
using System.Runtime.CompilerServices;

namespace Gestor.Controllers
{
    public class TransaccionesController: Controller
    {
        private readonly IRepositorioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IRepositorioCategorias repositorioCategorias;

        public TransaccionesController(IRepositorioUsuarios servicioUsuarios, 
        IRepositorioCuentas repositorioCuentas, IRepositorioCategorias repositorioCategorias)
        {
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioCategorias = repositorioCategorias;
        }

        /** 
            Este metodo devuelve la lista en donde creare la nueva transaccion
        */
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var modelo =  new TransaccionCreacionViewModel();
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            return View(modelo);

        }

        /** 
            Este metodo devuelve las cuentas en un lista con el nombre y el id (como string)
        */
        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioID)
        {
            var cuentas = await repositorioCuentas.BuscarPorID(usuarioID);
            return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        //Creo un metodo que me traiga las categorias de forma privada
        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId,
        TipoOperacion tipoOperacion)
        {
            var categorias = await repositorioCategorias.Obtener(usuarioId, tipoOperacion);
        }


        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
        }
    }

}