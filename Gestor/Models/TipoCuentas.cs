using System.ComponentModel.DataAnnotations;
using Gestor.Controllers;
using Gestor.Validaciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gestor.Models{

    //Permite la clasificacion de cuentas en grupos.
    public class TipoCuenta
    {
        
        /* Aplicando Validaciones desde el modelo. Recordar siempre aplicar Model.State para verificar desde el controlador
        Que el modelo sea valido */
        public int id {get; set;}

        [Required (ErrorMessage ="El campo Nombre debe ingresarse")]
        /* Usamos la validacion prediseñada */
        [PrimeraLetraMayuscula]
        /*Usamos el metodo de validacion prediseñado que se realice de forma automatica*/
        [Remote(action: "VerificarExisteCuenta", controller: "TiposCuentas")]
        public string Nombre {get; set;}

        public int UsuarioId {get; set;}

        public int Orden {get; set;}
    }
}





