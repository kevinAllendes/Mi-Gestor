using System.ComponentModel.DataAnnotations;

namespace Gestor.Validaciones
{
    /*
        Generamos un atributo propio llamado PrimeraLetraMayuscula
        Que si el nombre ingresado no empieza con una mayuscula da error. 
        
        Important: the new validation class name must be called with the word Attribute added in the end ! 
    */
    public class PrimeraLetraMayusculaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }
            var primerLetraMayuscula = value.ToString()[0].ToString();
            if(primerLetraMayuscula != primerLetraMayuscula.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayuscula");
            }

            return ValidationResult.Success;
        }
    }

}