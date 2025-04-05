namespace Gestor.Models
{
    public class IndiceCuentasViewModel
    {
        public string TipoCuenta {  get; set; }
        public IEnumerable<Cuenta> Cuentas { get; set;}
        //Propiedad que tiene la suma de las balances entre todas las cuentas del enumerable
        public decimal Balance => Cuentas.Sum(x => x.Balance);

    }
}
