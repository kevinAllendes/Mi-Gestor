using Dapper;
using Gestor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Gestor.Servicios
{
    public interface IRepositorioUsuarios
    {
    }

    public class RepositorioTipoUsuarios : IRepositorioUsuarios
    {
        private readonly string connectionString;
        public RepositorioTipoUsuarios(IConfiguration configurationString)
        {
            this.connectionString = configurationString.GetConnectionString("DefaultConnection");
        }
    }

    
}
