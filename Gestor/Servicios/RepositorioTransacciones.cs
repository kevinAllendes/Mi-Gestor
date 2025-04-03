using System.Data;
using Dapper;
using Gestor.Models;
using Microsoft.Data.SqlClient;

namespace Gestor.Servicios
{
    public interface IRepositorioTransacciones
    {

    }

    public class RepositorioTransacciones: IRepositorioTransacciones
    {
        private readonly string connectionString;
        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            /** 
                Procedimiento almacenado utilizado: 
                ALTER PROCEDURE [dbo].[Transacciones_Insertar]
                    @UsuarioId int,
                    @FechaTransaccion date, 
                    @Monto decimal(18,2),
                    @CategoriaId int,
                    @CuentaId int, 
                    @Nota nvarchar(1000) = NULL
                AS
                BEGIN
                    INSERT INTO Transacciones(UsuarioId, FechaTransaccion, Monto, CategoriaId,
                    CuentaId, Nota)
                    --Al estar dividido entre gastos y ingresos se supone un gasto como negativo pero tomo su valor absoluto
                    VALUES(@UsuarioId, @FechaTransaccion, ABS(@Monto), @CategoriaId, @CuentaId, @Nota)

                    --Actualziamos el balance. 
                    UPDATE Cuentas
                    SET Balance += @Monto
                    WHERE Id= @CuentaId;

                    --Devolvemos el id de la transaccion recien creada
                    SELECT SCOPE_INDENTITY();

                END


            */
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Transacion_Insertar",
                new
                {
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota
                },
                commandType: System.Data.CommandType.StoredProcedure);
            transaccion.Id = id;
        }

    }
}