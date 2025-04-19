using System.ComponentModel.DataAnnotations;

namespace Gestor.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [EmailAddress(ErrorMessage ="El campo debe ser un correo electronico valido")]
        public string Email {get;set;}
        [Required(ErrorMessage ="El campo {0} es requerido")]
        public string Password {get;set;}
        public bool Recuerdame {get;set;}

    }
}