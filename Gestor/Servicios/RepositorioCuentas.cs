using Dapper;
using Gestor.Models;
using Microsoft.Data.SqlClient;

namespace Gestor.Servicios
{
    public interface IRepostorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuentaEditada);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> BuscarPorID(int idUser);
        Task Crear(Cuenta cuenta);

        Task<Cuenta> ObtenerPorId(int id, int userID);

    }
    public class RepositorioCuentas : IRepostorioCuentas
    {
        private readonly string connectionString;
        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task Crear(Cuenta cuenta)
        {
            //Metodo que me permite crear una nueva cuenta
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                @"INSERT INTO Cuentas (Nombre, TipoCuentaId, Descripcion, Balance)
            VALUES (@Nombre, @tipoCuentaId,@Descripcion, @Balance); SELECT SCOPE_IDENTITY();", cuenta);
            cuenta.Id = id;
        }

        /*Necesito un metodo para obtener el listado de cuentas de un usuario*/
        public async Task<IEnumerable<Cuenta>> BuscarPorID(int idUser)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(@"SELECt Cuentas.Id, Cuentas.Nombre, Balance, tc.Nombre as TipoCuenta FROM  Cuentas
                                                        INNER JOIN TipoCuentas tc
                                                        On tc.Id = Cuentas.TipoCuentaId
                                                        WHERE tc.UsuarioId = @idUser
                                                        ORDER BY tc.Orden", new { idUser });

        }

        //Procedimiento para editar cuentas
        public async Task<Cuenta> ObtenerPorId(int idCuenta, int UsuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"SELECT Cuentas.Id, 
                                                        Cuentas.Nombre, Balance, Descripcion, 
                                                        tc.Id FROM  Cuentas
                                                        INNER JOIN TipoCuentas tc
                                                        On tc.Id = Cuentas.TipoCuentaId
                                                        WHERE tc.UsuarioId = @UsuarioId
                                                        AND Cuentas.Id = @idCuenta", 
                                                        new { idCuenta, UsuarioId });
        }

        //Metodo para editar cuenta
        public async Task Actualizar(CuentaCreacionViewModel cuentaEditada)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Cuentas SET Nombre = @Nombre, Balance = @Balance, 
                                            Descripcion = @Descripcion, TipoCuentaId =@TipoCuentaId
                                            WHERE Id = @Id",cuentaEditada);

        }

        //Metodo para borrado de cuenta
        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE Cuentas WHERE Id=@Id",new { id} );
        }
    }
}

