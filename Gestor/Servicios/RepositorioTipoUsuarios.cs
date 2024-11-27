using Dapper;
using Gestor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Gestor.Servicios
{
    public interface IRepositorioUsuarios
    {
        Task<bool> BuscarUsuario(TipoUsuarios usuarios);

    }

    public class RepositorioTipoUsuarios : IRepositorioUsuarios
    {
        private readonly string connectionString;
        public RepositorioTipoUsuarios(IConfiguration configurationString)
        {
            this.connectionString = configurationString.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> BuscarUsuario(TipoUsuarios usuario)
        {
            using var connection = new SqlConnection(connectionString);
            var id =  await connection.QueryFirstOrDefaultAsync<int>($@"SELECT 1 FROM Usuarios WHERE nombreUsuarios = @nombreUsuario AND UsrPassword = @Password", usuario);
            return (id == 1);
        }
    }

    
}
