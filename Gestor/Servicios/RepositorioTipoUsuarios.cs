using Dapper;
using Gestor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Gestor.Servicios
{
    public interface IRepositorioUsuarios
    {
        Task<bool> BuscarUsuario(TipoUsuarios usuarios);

        int ObtenerUsuarioId();

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
            var connection =  new SqlConnection(connectionString);
            var Existe = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1 FROM Usuarios WHERE Usuario=@nombreUsuario AND Password=@Password ", new { usuario.nombreUsuario, usuario.Password} );
            return Existe==1;
            
        }

        public int ObtenerUsuarioId()
        //En contruccion: Este metodo nos dara el id del usuario que esta usando la aplicacion
        {
            return 1;
        }
    }

    
}
