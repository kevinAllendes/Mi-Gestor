namespace MiGestor.Models;

public class TipoUsuarios
{
    public int IdUser {get; set;}
    public string nombreUsuario {get; set;}
    public string Password {get; set;}
    public string PasswordNoramlizado {get; set;}
    public string HashCode {get; set;}
    public int IdPermisos {get; set;}

}
