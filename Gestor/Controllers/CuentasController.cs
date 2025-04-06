using Gestor.Models;
using Gestor.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using AutoMapper;

namespace Gestor.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTipoCuentas repositorioTipoCuentas;
        private readonly IRepositorioUsuarios repositorioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IMapper autoMapper;

        private readonly IRepositorioTransacciones repositorioTransacciones;

        public CuentasController(IRepositorioTipoCuentas repositorioTipoCuentas,
            IRepositorioUsuarios repositorioUsuarios,IRepositorioCuentas repostorioCuentas,
            IMapper autoMapper, IRepositorioTransaccioens repositorioTransacciones) 
        {
           this.repositorioTipoCuentas = repositorioTipoCuentas;
           this.repositorioUsuarios = repositorioUsuarios;
           this.repositorioCuentas = repostorioCuentas;
            this.autoMapper = autoMapper;
            this.repositorioTransacciones = repositorioTransacciones;
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
            /*Metodo para la creacion de una cuenta*/
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
            await repositorioCuentas.Crear(nuevaCuenta);
            return RedirectToAction("Index");
            
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuenta(int usuarioId)
        {
            var tiposCuentas = await repositorioTipoCuentas.Obtener(usuarioId);
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));

        }

        public async Task<IActionResult> Index()
        {
            var id = repositorioUsuarios.ObtenerUsuarioId();
            var cuentaConTipoCuenta = await repositorioCuentas.BuscarPorID(id);

            //Agrupamos por tipo cuenta y mapeamos
            var modelo = cuentaConTipoCuenta
                .GroupBy(x => x.TipoCuenta)
                .Select(grupo => new IndiceCuentasViewModel
                {
                    TipoCuenta = grupo.Key,
                    Cuentas = grupo.AsEnumerable()
                }).ToList();


            return View(modelo);
        }

        public async Task<IActionResult> Editar(int Id)
        {
            var userId = repositorioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(Id,userId);
            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            //Mapeamos con autommaper
            var modelo = autoMapper.Map<CuentaCreacionViewModel>(cuenta);

            modelo.TiposCuenta = await ObtenerTiposCuenta(userId);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel miCuenta)
        {
            var userId = repositorioUsuarios.ObtenerUsuarioId();

            //Obtengo la cuenta a editar
            var cuenta = await repositorioCuentas.ObtenerPorId(miCuenta.Id, userId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioCuentas.Actualizar(miCuenta);
            return RedirectToAction("Index");
        }

        //Metodo de apertura de vista Borrar
        public async Task<IActionResult> Borrar(int id)
        {
            var userID = repositorioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, userID);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(cuenta);
        }

        //Metodo Post
        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int IdCuenta)
        {
            var userID = repositorioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(IdCuenta, userID);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioCuentas.Borrar(IdCuenta);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detalle(int id,int mes, int año)
        {
            var usuarioId = repositorioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);
            if(cuenta is null){
                return RedirectToAction("NoEncontrado","Home");
            }

            DateTime fechaInicio;
            DateTime fechaFin;

            if(mes <= 0 || mes >= 12 || año <= 1900)
            {
                var hoy = DateTime.Today;
                fechaInicio =  new DateTime(hoy.Year, hoy.Month ,1);
            }
            else{
                fechaInicio =  new DateTime(año, mes ,1);
            }
            fechaFin =  fechaInicio.AddMonth(1).AddDays(-1);

            var obtenerTransaccionesPorCuenta  = new ObtenerTransaccionesPorCuenta()
            {
                CuentaId = id,
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var transacciones = await repositorioTransacciones.ObtenerPorCuentaId(obtenerTransaccionesPorCuenta);
            var modelo = new ReporteTransaccionesDetallladas();
            ViewBag.Cuenta = cuenta.Nombre;

            var TransaccionPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion)
            .GroupBy(x => x.FechaTransaccion)
            .Select(grupo => new ReporteTransaccionesDetalladas.TransaccionesPorFecha()
            {
                FechaTransaccion = grupo.Key,
                Transacciones = grupo.AsEnumerable()
            });

            modelo.TransaccionesAgrupadas = TransaccionPorFecha;
            modelo.FechaInicio = fechaInicio;
            modelo.FechaFin = fechaFin;

            ViewBag.mesAnterior = fechaInicio.AddMonth(-1).Month;
            ViewBag.añoAnterior = fechaInicio.AddMonth(-1).Year;
            ViewBag.mesPosterior = fechaInicio.AddMonth(1).Month;
            ViewBag.añoPosterior = fechaInicio.AddMonth(1).Year;

            return View(modelo);
        }
    }
}
