namespace Gestor.Models
{
    public class ReporteTransaccionesDetalladas
    {
        public DateTime FechaInicio {get;set;}
        public DateTime FechaFin {get;set;}
        public IEnumerable<TransaccionPorFecha> TransaccionesAgrupadas {get;set;}
        public decimal BalanceDepositos {get;set;}
        public decimal BalanceRetiros {get;set;}

        public decimal Total => BalanceDepositos - BalanceRetiros;


        public class TransaccionPorFecha
        {
            public DateTime FechaTransaccion {get;set;}
            public IEnumerable<Transaccion> Transacciones {get;set;}
            public decimal BalanceDepositos => Transacciones.Where(x => x.tipoOperacionId == TipoOperacion.Ingreso)
            .Sum(x => x.Monto);

            public decimal BalanceRetiros => Transacciones.Where(x => x.tipoOperacionId == TipoOperacion.Gasto)
            .Sum(x => x.Monto);
        }

    }

}