using Gestor.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Gestor.Models;
using System.Runtime.CompilerServices;
using AutoMapper;
using Microsoft.IdentityModel.Abstractions;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Data;
using ClosedXML.Excel;

namespace Gestor.Controllers
{
    public class TransaccionesController: Controller
    {
        private readonly IRepositorioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IRepositorioCategorias repositorioCategorias;

        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IMapper mapper;
        private readonly IServicioReportes servicioReportes;

        public TransaccionesController(IRepositorioUsuarios servicioUsuarios, 
        IRepositorioCuentas repositorioCuentas, IRepositorioCategorias repositorioCategorias,
        IRepositorioTransacciones repositorioTransacciones, IMapper mapper, IServicioReportes servicioReportes)
        {
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioCategorias = repositorioCategorias;
            this.repositorioTransacciones = repositorioTransacciones;
            this.mapper = mapper;
            this.servicioReportes = servicioReportes;
        }

        /** 
            Este metodo devuelve la lista en donde creare la nueva transaccion
        */
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var modelo =  new TransaccionCreacionViewModel();
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.Categorias =  await ObtenerCategorias(usuarioId, modelo.tipoOperacionId);
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
            var categoria =  await repositorioCategorias.ObtenerPorId(modelo.CategoriaId,usuarioId);
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

        public async Task<IActionResult> Semanal(int mes, int año)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            IEnumerable<ResultadoObtenerPorSemana> transaccionesPorSemana = await servicioReportes.ObtenerReporteSemanal(usuarioId,
            mes, año, ViewBag);
            var agrupado = transaccionesPorSemana.GroupBy(x => x.Semana).Select(x =>
                new ResultadoObtenerPorSemana()
                {
                    Semana = x.Key,
                    Ingresos = x.Where(x => x.TipoOperacionId == TipoOperacion.Ingreso)
                    .Select(x=>x.Monto).FirstOrDefault(),
                    Gastos = x.Where(x => x.TipoOperacionId == TipoOperacion.Gasto)
                    .Select(x=>x.Monto).FirstOrDefault()
                }).ToList();
                if(año == 0 || mes==0)
                {
                    var hoy =DateTime.Today;
                    año=  hoy.Year;
                    mes = hoy.Month;
                }

                var fechaReferencia  = new DateTime(año, mes, 1);
                var diasDelMes = Enumerable.Range(1, fechaReferencia.AddMonths(1).AddDays(-1).Day);
                var diasSegmentados = diasDelMes.Chunk(7).ToList();

                for(int i= 0; i <diasSegmentados.Count(); i++)
                {
                    var semana = i+1;
                    var fechaInicio =  new DateTime(año, mes , diasSegmentados[i].First());
                    var fechaFin =  new DateTime(año, mes, diasSegmentados[i].Last());
                    var grupoSemana = agrupado.FirstOrDefault(x => x.Semana == semana);

                    if(grupoSemana is null)
                    {
                        agrupado.Add(new ResultadoObtenerPorSemana()
                        {
                            Semana = semana,
                            FechaInicio = fechaInicio,
                            FechaFin = fechaFin

                        });

                    }
                    else
                    {
                        grupoSemana.FechaInicio = fechaInicio;
                        grupoSemana.FechaFin = fechaFin;
                    }
                }
            agrupado = agrupado.OrderByDescending(x => x.Semana).ToList();
            var modelo =  new ReporteSemanalViewModel();
            modelo.TransaccionesPorSemana = agrupado;
            modelo.FechaReferencia = fechaReferencia;

            return View();
        }

        public async Task<IActionResult> Mensual(int año)
        {
            var usuarioId =  servicioUsuarios.ObtenerUsuarioId();
            if(año == 0)
            {
                año = DateTime.Today.Year;
            }
            var transaccionesPorMes = await repositorioTransacciones.ObtenerPorMes(usuarioId,año);
            var transaccionesAgrupadas = transaccionesPorMes.GroupBy(x => x.Mes)
                .Select(x => new ResultadoObtenerPorMes()
                {
                    Mes = x.Key,
                    Ingreso = x.Where(x => x.TipoOperacionId == TipoOperacion.Ingreso)
                    .Select(x => x.Monto).FirstOrDefault(),
                    Gasto = x.Where(x => x.TipoOperacionId == TipoOperacion.Gasto)
                    .Select(x => x.Monto).FirstOrDefault()
                }).ToList();  
            for(int mes=1; mes <=12; mes++)
            {
                var transaccion = transaccionesAgrupadas.FirstOrDefault(x => x.Mes == mes);
                var fechaReferencia = new DateTime(año,mes,1);
                if(transaccion is null)
                {
                    transaccionesAgrupadas.Add(new ResultadoObtenerPorMes()
                    {
                        Mes = mes,
                        FechaReferencia = fechaReferencia
                    });
                }
                else{
                    transaccion.FechaReferencia = fechaReferencia;
                }
            }
            transaccionesAgrupadas = transaccionesAgrupadas.OrderByDescending(x => x.Mes).ToList();
            var modelo = new ReporteMensualViewModel();
            modelo.año = año;
            modelo.TransaccionesPorMes = transaccionesAgrupadas;
            return View();
        }

        public IActionResult ExcelReporte()
        {
            return View();
        }

        [HttpGet]
        public async Task<FileResult> ExportarExcelPorMes(int mes, int año)
        {
            var fechaInicio = new DateTime(año, mes, 1);
            var fechaFin =  fechaInicio.AddMonths(1).AddDays(-1);
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(
                new ParametroObtenerTransaccionesPorUsuario
                {
                    UsuarioId = usuarioId,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                });
            var nombreArchivo = $"MAnejo Presupuesto - {fechaInicio.ToString("MMM yyy")}.xlsx";
            return GenerarExcel(nombreArchivo,transacciones);
    
        }

        private FileResult GenerarExcel(string nombreArchivo,
            IEnumerable<Transaccion> transacciones)
        {
            DataTable dataTable = new DataTable("Transacciones");
            dataTable.Columns.AddRange(new DataColumn[]{
                new DataColumn("Fecha"),
                new DataColumn("Cuenta"),
                new DataColumn("Categoria"),
                new DataColumn("Nota"),
                new DataColumn("Monto"),
                new DataColumn("Ingreso/Gasto"),
            });
            foreach(var transaccion in transacciones)
            {
                dataTable.Rows.Add(transaccion.FechaTransaccion,
                transaccion.Cuenta,
                transaccion.Categoria,
                transaccion.Nota,
                transaccion.Monto,
                transaccion.tipoOperacionId);
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                }
            }

        }

        public IActionResult Calendario()
        {
            return View();
        }

        public async Task<IActionResult> Index(int mes, int año)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioId();
            var modelo = await servicioReportes.ObtenerReporteTransaccionesDetalladas(usuarioID,mes,año,ViewBag);
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
            return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }


        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var categorias = await ObtenerCategorias(usuarioId, tipoOperacion);
            return Ok(categorias);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id, string urlRetorno =  null)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var transaccion =  await repositorioTransacciones.ObtenerPorId(id, usuarioId);

            if(transaccion is null)
            {
                return RedirectToAction("No encontrado","Home");
            }
            var modelo = mapper.Map<TransaccionActualizacionViewModel>(transaccion);
            if(modelo.tipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.MontoAnterior =  modelo.Monto *-1;
            }
            modelo.CuentaAnteriorId = transaccion.CuentaId;
            modelo.Categorias = await ObtenerCategorias(usuarioId, transaccion.tipoOperacionId);
            modelo.Cuentas =await ObtenerCuentas(usuarioId);
            modelo.urlRetorno = urlRetorno;
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TransaccionActualizacionViewModel modelo)
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
                return RedirectToAction("NoEncontrado","Home");
            }

            var categoria = await repositorioCategorias.ObtenerPorId(modelo.CategoriaId, usuarioId);
            if(categoria is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }
            var transaccion =  mapper.Map<Transaccion>(modelo);
            if(modelo.tipoOperacionId == TipoOperacion.Gasto)
            {
                transaccion.Monto *= -1;
            }
            await repositorioTransacciones.Actualizar(transaccion, modelo.MontoAnterior, 
            modelo.CuentaAnteriorId);

            if(string.IsNullOrEmpty(modelo.urlRetorno))
            {
                return RedirectToAction("Index");
            }
            else{
                return LocalRedirect(modelo.urlRetorno);
            }
            

        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id, string urlRetorno =  null)
        {
            var usuarioId =  servicioUsuarios.ObtenerUsuarioId();
            var transaccion = await repositorioTransacciones.ObtenerPorId(id, usuarioId);
            if(transaccion is null) { 
                return RedirectToAction("NoEncontrado","Home");
            }

            await repositorioTransacciones.Borrar(id);
            
            if(string.IsNullOrEmpty(urlRetorno))
            {
                return RedirectToAction("Index");
            }
            else{
                return LocalRedirect(urlRetorno);
            }

        }




    }

}