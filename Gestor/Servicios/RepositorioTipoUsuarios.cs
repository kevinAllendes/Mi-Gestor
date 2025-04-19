using Dapper;
using Gestor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Gestor.Servicios
{
    public interface IRepositorioUsuarios
    {
        Task<bool> BuscarUsuario(TipoUsuarios usuarios);
        int ObtenerUsuarioId();
        Task<TipoUsuarios> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<int> CrearUsuario(TipoUsuarios usuario);


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
            var Existe = await connection.QueryFirstOrDefaultAsync<int>(@"
            SELECT 1 FROM Usuarios WHERE Usuario=@nombreUsuario AND Password=@Password 
            ", new { usuario.nombreUsuario, usuario.Password} );
            return Existe==1;
            
        }
        public int ObtenerUsuarioId()
        //En contruccion: Este metodo nos dara el id del usuario que esta usando la aplicacion
        {
            return 1;
        }

        public async Task<int> CrearUsuario(TipoUsuarios usuario)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"
            INSERT INTO Usuarios (Email, EmailNormalizado, HashCode)
            VALUES (@Emaill, @EmailNormalizado, @HashCode);
            SELECT SCOPE_IDENTITY();", usuario);
            return id;
        }

        public async Task<TipoUsuarios> BuscarUsuarioPorEmail(string emailNormalizado)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QuerySingleOrDefaultAsync<TipoUsuarios>(@"
            SELECT * FROM Usuarios WHERE EmailNormalizado = @emailNormalizado",
            new {emailNormalizado});
        }
    }

    
}
