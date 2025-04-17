using Gestor.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Gestor.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Crear(Categorias miCategoria);
        Task<IEnumerable<Categorias>> Obtener(int usuarioId);

        Task<IEnumerable<Categorias>> Obtener(int usuarioId, TipoOperacion tipoOperacionId);
        Task Actualizar(Categorias categoria);
        Task<Categorias> ObtenerPorId(int id, int usuarioId);

        Task Borrar(int id);
    }
    public class RepositorioCategorias: IRepositorioCategorias
    {
        private readonly string connectionString;

        public RepositorioCategorias(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Categorias miCategoria)
        {
            using var connection = new SqlConnection(connectionString);
            var resultado = await connection.QuerySingleAsync<int>(@"INSERT INTO Categorias (Nombre,TipoOperacionId,UsuarioId) VALUES (@Nombre,@TipoOperacionId,@UsuarioId);
                                                                            SELECT SCOPE_IDENTITY();", miCategoria);
            miCategoria.Id = resultado;
        }

        //Metodo que me devuelve todas las categorias de un usuario
        public async Task<IEnumerable<Categorias>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categorias>("SELECT * FROM " +
                "Categorias WHERE UsuarioId = @usuarioID", new { usuarioId });
        }

        public async Task<IEnumerable<Categorias>> Obtener(int usuarioId, TipoOperacion tipoOperacionId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categorias>(
                @"SELECT * FROM " +
                "Categorias WHERE UsuarioId = @usuarioID AND TipoOperacionId = @tipoOperacionId", 
                new { usuarioId, tipoOperacionId });
        }


        //Devuelve solo una categoria del usuario indicado (la que le corresponde el id)
        public async Task<Categorias> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categorias>(
                @"Select * From Categorias Where Id = @Id AND UsuarioId = @UsuarioId",
                new { id, usuarioId});
        }

        public async Task Actualizar(Categorias categoria)
        {
            using var connection  = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Categorias 
            SET Nombre = @Nombre, TipoOperacionId = @TipoOperacionId
            WHERE Id = @Id", categoria);
        }

        //Metodo de borrado de categorias
        public async Task Borrar(int id)
        {
            using var connection =  new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE Categorias WHERE Id = @Id", new {id});

        }







    }
}
