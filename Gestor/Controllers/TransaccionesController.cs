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

        private readonly IRepositorioTransacciones repositorioTransacciones;

        public TransaccionesController(IRepositorioUsuarios servicioUsuarios, 
        IRepositorioCuentas repositorioCuentas, IRepositorioCategorias repositorioCategorias,
        IRepositorioTransacciones repositorioTransacciones)
        {
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioCategorias = repositorioCategorias;
            this.repositorioTransacciones = repositorioTransacciones;
        }

        /** 
            Este metodo devuelve la lista en donde creare la nueva transaccion
        */
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var modelo =  new TransaccionCreacionViewModel();
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.Categorias =  await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
            return View(modelo);

        }

        [HttpPost]
        public async Task<IActionResult> Crear(TransaccionCreacionViewModel modelo)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if(!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.tipoOperacionId);
                return View(modelo);
            }

            var cuenta = await repositorioCuentas.ObtenerPorId(modelo.CuentaId, usuarioId);
            if(cuenta is null)
            {
                return RedirectToAction("No Encontrado","Home");
            }
            var categoria =  await repositorioCategorias.ObtenerPorId(modelo.CuentaId,usuarioId);
            if(categoria is null)
            {
                return RedirectToAction("No Encontrado","Home");
            }
            modelo.UsuarioId = usuarioId;
            if(modelo.tipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.Monto*= -1;
            }
           await repositorioTransacciones.Crear(modelo);
           return RedirectToAction("Index");
           

        }

        public IActionResult Index()
        {
            return View();
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
            return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }


        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var categorias = await ObtenerCategorias(usuarioId, tipoOperacion);
            return Ok(categorias);
        }
    }

}