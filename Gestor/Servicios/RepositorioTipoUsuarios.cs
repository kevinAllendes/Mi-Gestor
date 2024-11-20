using Dapper;
using Gestor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Gestor.Servicios
{
    public interface IRepositorioUsuarios
    {
        Task BuscarUsuario(TipoUsuarios usuarios);

    }

    public class RepositorioTipoUsuarios : IRepositorioUsuarios
    {
        private readonly string connectionString;
        public RepositorioTipoUsuarios(IConfiguration configurationString)
        {
            this.connectionString = configurationString.GetConnectionString("DefaultConnection");
        }

        public async Task BuscarUsuario(TipoUsuarios usuario)
        {
            using var connection = new SqlConnection(connectionString);
            var id =  await connection.QuerySingle($@"SELECT IdUsuario FROM Usuarios WHERE Usuario = @nombreUsuario", usuario.nombreUsuario);
            if (id != null)
            {
                usuario.IdUser = 0;
            }
            else
            {
                usuario.IdUser = id;
            }
        }
    }

    
}
