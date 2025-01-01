using Dapper;
using Gestor.Controllers;
using Gestor.Models;
using Microsoft.Data.SqlClient;

namespace Gestor.Servicios
{
    public interface IRepositorioTipoCuentas
    {
        Task Crear(TipoCuenta miCuenta);
        Task<bool> Existe(string nombre, int idUsuario);

        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioID);

        Task Actualizar(TipoCuenta tipoCuenta);

        Task<TipoCuenta> ObtenerPorId(int id, int usuarioID);

        Task Borrar(int Id);
        Task Ordenar(IEnumerable<TipoCuenta> tiposOrdenados);
    }

    public class RepositorioTipoCuentas: IRepositorioTipoCuentas
    {
        private readonly string connectionString;

        public RepositorioTipoCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /* 
            Definimos un metodo que cree un tipo cuenta
            este metodo tiene que ser asincrono ya que representa la comunicacion entre dos sistemas
            una operacion i/O
            Reemplazamos 
            public  void Crear(TipoCuenta miCuenta)
            por su asyncrono
            Reemplazamos todos los void por Task
            Ademas finalizamos utilizando un stored procedure.
        */
        public  async Task Crear(TipoCuenta miCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("TipoCuentas_Insertar", 
                                                            new { usuarioId=miCuenta.UsuarioId, 
                                                            nombre = miCuenta.Nombre},
                                                            commandType: System.Data.CommandType.StoredProcedure);
            miCuenta.Id = id;
        }

        //Metodo que nos indica si existe un tipo cuenta dentro de la contabilidad del usario
        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe =  await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1 FROM 
            TipoCuentas WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId;", new {nombre, usuarioId});
            return existe == 1;   
        }

        /*
            Metodo para obtener los tipos de cuentas que posee el usuario
            en forma de IEnumerable
        */
        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                            FROM TipoCuentas
                                                            WHERE UsuarioId = @UsuarioId
                                                            ORDER  BY Orden", new {usuarioId});
        }

        /*Metodo para actualizar un tipo cuentas*/
        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection =  new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TipoCuentas SET Nombre = @Nombre 
                                            WHERE Id=@Id", tipoCuenta);
        }

        /*Metodo que devuelve un tipo cuentas de la tabla en base a su id y al id del usuario que esta trabajando*/
        public async Task<TipoCuenta> ObtenerPorId(int id,  int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden 
            FROM TipoCuentas WHERE Id=@Id AND UsuarioId = @usuarioId", new {id,usuarioId});
        }

        
        public async Task Borrar(int Id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE TipoCuentas WHERE Id=@Id", new {Id});
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tiposCuentaOrdenados)
        {
            using var connection = new SqlConnection(connectionString);
            var query = "UPDATE TiposCuentas SET Orden = @Orden Where Id=@Id;";
            //Dapper nos permite ejecutar el query por cada Tipo cuenta del enumerable
            //  solo utilizando el Execute Async
            await connection.ExecuteAsync(query, tiposCuentaOrdenados);
        }


    }
}