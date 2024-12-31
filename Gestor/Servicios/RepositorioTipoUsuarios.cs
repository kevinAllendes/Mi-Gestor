using Dapper;
using Gestor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Gestor.Servicios
{
    public interface IRepositorioUsuarios
    {
        bool BuscarUsuario(TipoUsuarios usuarios);

        int ObtenerUsuarioId();

    }

    public class RepositorioTipoUsuarios : IRepositorioUsuarios
    {
        private readonly string connectionString;
        public RepositorioTipoUsuarios(IConfiguration configurationString)
        {
            this.connectionString = configurationString.GetConnectionString("DefaultConnection");
        }

        public bool BuscarUsuario(TipoUsuarios usuario)
        {
            return usuario.nombreUsuario == "Kevin" && usuario.Password == "1234";
            
        }

        public int ObtenerUsuarioId()
        //En contruccion: Este metodo nos dara el id del usuario que esta usando la aplicacion
        {
            return 1;
        }
    }

    
}
