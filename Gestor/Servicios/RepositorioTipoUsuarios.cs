using System.Security.Claims;
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
        private readonly HttpContext httpContext;
        public RepositorioTipoUsuarios(IConfiguration configurationString,IHttpContextAccessor httpContextAccesor)
        {
            this.connectionString = configurationString.GetConnectionString("DefaultConnection");
            this.httpContext = httpContextAccesor.HttpContext;
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
        //Metodo impementado utilziando httpContext
        {
            if(httpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = httpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                var id = int.Parse(idClaim.Value);
                return id;
            }
            else
            {
                throw new ApplicationException("El usuario no esta autenticado");
            }
        }

        public async Task<int> CrearUsuario(TipoUsuarios usuario)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"
            INSERT INTO Usuarios (Email, EmailNormalizado, HashCode)
            VALUES (@Emaill, @EmailNormalizado, @HashCode);
            SELECT SCOPE_IDENTITY();", usuario);

            await connection.ExecuteAsync("CrearDatosUsuarioNuevo", new{id},
            commandType: System.Data.CommandType.StoredProcedure);
            
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

    /**
        CREATE PROCEDURE CrearDatosUsuarioNuevo
            @userId int
        AS
        BEGIN
            --Defino unos tipos de cuentas basicas
            DECLARE @Efectivo nvarchar(50) = 'Efectivo';
            DECLARE @CuentasDeBanco nvarchar(50) = 'Cuentas de Banco';
            DECLARE @Tarjetas nvarchar(50) = 'Tarjetas';
            INSERT INTO TiposCuentas(Nombre, UsuarioId, Orden)
            VALUES (@Efectivo, @UsuarioId,1),
            (@CuentasDeBanco, @UsuarioId,2),
            (@Tarjetas, @UsuarioId,3);

            --Inserto en cuentas el resultado del select a Tiposcuentas
            INSERT INTO Cuentas (Nombre, Balance, TipoCuentaId)
            SELECT Nombre, 0 ,Id
            FROM TiposCuentas
            WHERE UsuarioId = @UsuarioId;

            --Inserto en Categorias las categorias iniciales
            INSERT INTO Categorias(Nombre, TipoOperacionId, UsuarioId)
            VALUES
            ('Libros',2,@UsuarioId),
            ('Salario',1,@UsuarioId),
            ('Mesada',1,@UsuarioId),
            ('Comida',2,@UsuarioId);



        END

    */
}
