using Gestor.Models;
using Gestor.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace Gestor.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTipoCuentas repositorioTipoCuentas;
        private readonly IRepositorioUsuarios repositorioUsuarios;
        private readonly IRepostorioCuentas repostorioCuentas;


        public CuentasController(IRepositorioTipoCuentas repositorioTipoCuentas,
            IRepositorioUsuarios repositorioUsuarios,IRepostorioCuentas repostorioCuentas) 
        {
           this.repositorioTipoCuentas = repositorioTipoCuentas;
           this.repositorioUsuarios = repositorioUsuarios;
           this.repostorioCuentas = repostorioCuentas;
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = repositorioUsuarios.ObtenerUsuarioId();
           
            var modelo = new CuentaCreacionViewModel();
            //Generamos la lista que empareja cada tipo cuenta con su id
            modelo.TiposCuenta = await ObtenerTiposCuenta(usuarioId);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel nuevaCuenta)
        {
            var usuarioId = repositorioUsuarios.ObtenerUsuarioId();
            var tipoCuentas = await repositorioTipoCuentas.Obtener(usuarioId);
            if(tipoCuentas is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                nuevaCuenta.TiposCuenta = await ObtenerTiposCuenta(usuarioId);
                return View(nuevaCuenta);

            }
            await repostorioCuentas.Crear(nuevaCuenta);
            return RedirectToAction("Index");
            
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuenta(int usuarioId)
        {
            var tiposCuentas = await repositorioTipoCuentas.Obtener(usuarioId);
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));

        }
    }
}
