using System.ComponentModel.DataAnnotations;

namespace Gestor.Models
{
    public class Transaccion
    {
        public int Id {get;set;}
        public int UsuarioId {get; set;}
        //Remember:  yyyy-MM-dd hh:MM tt is equal to g!
        [Display(Name = "Fecha Transaccion")]
        [DataType(DataType.DateTime)]
        public DateTime FechaTransaccion{get; set;} = DateTime.Parse(DateTime.Now.ToString("g"));
        public decimal Monto {get;set;}
        //Categoria Id
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe Seleccionar una categoria")]
        [Display(Name ="Categoria")]
        public int CategoriaId {get;set;}
        //Nota
        [StringLength(maximumLength: 1000, ErrorMessage = "La nota no puede pasar de {1} caracteres")]
        public string Nota {get;set;}
        //Cuenta Id
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una cuenta")]
        [Display(Name ="Cuenta")]
        public int CuentaId {get; set;} 
        [Display(Name ="Tipo Operacion")]
        public TipoOperacion tipoOperacionId {get; set;} = TipoOperacion.Ingreso;
        public string Cuenta{get; set;}
        public string Categoria {get;set;}

    }
}