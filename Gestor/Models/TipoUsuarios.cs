namespace Gestor.Models;

public class TipoUsuarios
{
    public int IdUser {get; set;}
    public string nombreUsuario {get; set;}
    public string Email {get;set;}
    public string EmailNormalizado {get;set;}
    public string Password {get; set;}
    public string HashCode {get; set;}
    public int IdPermisos {get; set;}

}
