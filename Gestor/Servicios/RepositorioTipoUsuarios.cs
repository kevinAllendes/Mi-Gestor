using Dapper;
using Gestor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Gestor.Servicios
{
    public interface IRepositorioUsuarios
    {
        bool BuscarUsuario(TipoUsuarios usuarios);

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
    }

    
}
