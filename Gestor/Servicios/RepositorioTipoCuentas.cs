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

        //Metodo provisorio para usuar en el trabajo
        IEnumerable<TipoCuenta> obtenerCuentasSinBDD(int usario);
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
        */
        public  async Task Crear(TipoCuenta miCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>($@"INSERT INTO TiposCuentas (Nombre, UsuarioId, Orden) 
                                                    Values (@Nombre, @UsuarioId, 0);
                                                    SELECT SCOPE_IDENTITY();", miCuenta);
            miCuenta.id = id;
        }

        //Metodo que nos indica si existe un tipo cuenta dentro de la contabilidad del usario
        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe =  await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1 FROM 
            TiposCuentas WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId;", new {nombre, usuarioId});
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
                                                            FROM TiposCuentas
                                                            WHERE UsuarioId = @UsuarioId", new {usuarioId});
        }

        /*Creo un metodo que me devuelva una lista de cuentas ya que en la fabrica no tengo acceso a la BDD*/
        public IEnumerable<TipoCuenta> obtenerCuentasSinBDD(int usarioID)
        {
            var cuenta1 = new TipoCuenta(){Nombre="Tarjetas", Orden=1};
            var cuenta2 = new TipoCuenta(){Nombre="Sueldo", Orden=1};
            var cuenta3 = new TipoCuenta(){Nombre="Garantia", Orden=1};
            List<TipoCuenta> misCuentas = new List<TipoCuenta>();
            misCuentas.Add(cuenta1);
            misCuentas.Add(cuenta2);
            misCuentas.Add(cuenta3);
            return misCuentas;
        }
    }
}