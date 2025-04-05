using System.Data;
using Dapper;
using Gestor.Models;
using Microsoft.Data.SqlClient;

namespace Gestor.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int CuentaAnteriorId);
        Task Crear(Transaccion transaccion);


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

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {
            /**
                 Este metodo utlizara un store procedure
                CREATE PROCEDURE Transacciones_Actualizar
                    @Id int,
                    @FechaTransaccion datetime,
                    @Monto decimal(18,2),
                    @MontoAnterior decimal(18,2),
                    @CuentaId int,
                    @CuentaAnteriorId int,
                    @CategoriaId int,
                    @Nota nvarchar(1000) = NULL
                AS
                BEGIN

                    SET NOCOUNT ON;

                    --Revertir transacciones anterior

                    UPDATE Cuentas SET Balanc -= @MontoAnterior WHERE Id = @CuentaAnteriorId;

                    --Realizar nueva transaccion

                    UPDATE Cuentas SET Balance += @Monto WHERE Id = @CuentaId;

                    UPDATE Transacciones SET Monto = ABS(@Monto), FechaTransaccion = @FechaTransaccion,
                    CategoriaId = @CategoriaId, CuentaId = @CuentaId, Nota = @Nota
                    WHERE Id = @Id;
                END
            */
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar",
                new
                {
                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota,
                    montoAnterior,
                    cuentaAnteriorId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection =  new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(@"
            SELECT * FROM Transacciones INNER JOIN Categorias cat ON cat.Id = Transacciones.CategoriaId
            WHERE Transacciones.Id = @Id AND Transacciones.UsuarioId = @UsuarioId", new {id, usuarioId});
            
            
        }

    }
}