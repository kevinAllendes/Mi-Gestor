using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Gestor.Models
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [EmailAddress(ErrorMessage ="El campo debe ser un correo electronico valido")]
        public string Email {get;set;}
        [Required(ErrorMessage ="El campo {0} es requerido")]
        public string Password {get;set;}

    }
}