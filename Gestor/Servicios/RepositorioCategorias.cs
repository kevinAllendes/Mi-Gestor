using Gestor.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Gestor.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Crear(Categoria miCategoria);
        Task<IEnumerable<Categoria>> Obtener(int usuarioId);
    }
    public class RepositorioCategorias: IRepositorioCategorias
    {
        private readonly string connectionString;

        public RepositorioCategorias(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Categoria miCategoria)
        {
            using var connection = new SqlConnection(connectionString);
            var resultado = await connection.QuerySingleAsync<int>(@"INSERT INTO Categorias (Nombre,TipoOperacionId,UsuarioId) VALUES (@Nombre,@TipoOperacionId,@UsuarioId);
                                                                            SELECT SCOPE_IDENTITY();", miCategoria);
            miCategoria.Id = resultado;
        }

        //Metodo que me devuelve todas las categorias de un usuario
        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>("SELECT * FROM " +
                "Categorias WHERE UsuarioId = @usuarioID", new { usuarioId });
        }

    }
}
