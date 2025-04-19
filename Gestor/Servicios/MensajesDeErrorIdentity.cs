using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Identity;

namespace Gestor.Servicios
{
    public class ManejoDeErrorIdentity: IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
        {
            return new IdentityError {Code = nameof(DefaultError), Description = $"Ha Ocurrido un error."}; 
        }

        public override IdentityError PasswordMismatch()
        {
            return new IdentityError {Code = nameof(PasswordMismatch), Description = $"Password incorrecta"}; 
        }
        public override IdentityError LoginAlreadyAssociated()
        {
            return new IdentityError {Code = nameof(LoginAlreadyAssociated), Description=$"Email ya asociado!. "};
        }

    }
}