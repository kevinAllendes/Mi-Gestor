using Dapper;
using Gestor.Models;
using Microsoft.Data.SqlClient;

namespace Gestor.Servicios
{
    public interface IRepostorioCuentas
    {
        Task Crear(Cuenta cuenta);
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


    }
}

