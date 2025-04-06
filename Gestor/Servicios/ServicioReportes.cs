using Gestor.Models;

namespace Gestor.Servicios
{
    public interface IServicioReportes
    {
        Task<ReporteTransaccionesDetalladas>  ObtenerReporteTransaccionesDetalladasPorCuenta(int usuarioId, int cuentaId,int mes, int año, dynamic ViewBag);
        Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladas(int usuarioID, int mes, int año, dynamic ViewBag);

    }

    public class ServicioReportes : IServicioReportes
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly HttpContext httpContext;
        public ServicioReportes(IRepositorioTransacciones repositorioTransacciones, HttpContextAccessor httpContextAccessor)
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.httpContext = httpContextAccessor.HttpContext;

        }

        public async Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladas(int usuarioID, int mes, int año, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechaInicioYFin(mes, año);
            
            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                UsuarioId = usuarioID,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

             var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(parametro);
             var modelo = GenerarReporte(fechaInicio, fechaFin, transacciones);
             AsignarValores(ViewBag, fechaInicio);
             return modelo;


        }

        public async Task<ReporteTransaccionesDetalladas> 
        ObtenerReporteTransaccionesDetalladasPorCuenta(int usuarioId, int cuentaId,
        int mes, int año, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechaInicioYFin(mes, año);

            var obtenerTransaccionesPorCuenta = new ObtenerTransaccionesPorCuenta()
            {
                CuentaId = cuentaId,
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };
            var transacciones = await repositorioTransacciones.ObtenerPorCuentaId(obtenerTransaccionesPorCuenta);

            var modelo = GenerarReporte(fechaInicio, fechaFin, transacciones);
            AsignarValores(ViewBag, fechaInicio);
            return modelo;

        }

        private void AsignarValores(dynamic ViewBag, DateTime fechaInicio)
        {
            ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;
            ViewBag.añoAnterior = fechaInicio.AddMonths(-1).Year;
            ViewBag.mesPosterior = fechaInicio.AddMonths(1).Month;
            ViewBag.añoPosterior = fechaInicio.AddMonths(1).Year;
            ViewBag.urlRetorno = httpContext.Request.Path + httpContext.Request.QueryString;
        }

        private static ReporteTransaccionesDetalladas GenerarReporte(DateTime fechaInicio, DateTime fechaFin, IEnumerable<Transaccion> transacciones)
        {
            var modelo = new ReporteTransaccionesDetalladas();


            var TransaccionPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion)
            .GroupBy(x => x.FechaTransaccion)
            .Select(grupo => new ReporteTransaccionesDetalladas.TransaccionPorFecha()
            {
                FechaTransaccion = grupo.Key,
                Transacciones = grupo.AsEnumerable()
            });

            modelo.TransaccionesAgrupadas = TransaccionPorFecha;
            modelo.FechaInicio = fechaInicio;
            modelo.FechaFin = fechaFin;
            return modelo;
        }

        private (DateTime fechaInicio, DateTime fechaFin) GenerarFechaInicioYFin(int mes, int año)
        {
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
            fechaFin =  fechaInicio.AddMonths(1).AddDays(-1);
            return (fechaInicio,fechaFin);

        }
    }

}