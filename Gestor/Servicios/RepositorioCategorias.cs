using Gestor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Drawing;

namespace Gestor.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Crear(Categoria miCategoria);

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
            using var connection = new  SqlConnection(connectionString);
            var resultado = await connection.QuerySingleAsync<int>(@"INSERT INTO Categorias (Nombre,TipoOperacion,UsuarioId) VALUES (@Nombre,@TipoOperacion,@UsuarioId);
                                                                            SELECT SCOPE_IDENTITY();",miCategoria);
            miCategoria.Id = resultado;
        }

    }
}
