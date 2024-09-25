using Microsoft.AspNetCore.Mvc;
using Gestor.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Gestor.Controllers
{
    public class TipoUsuariosController : Controller
    {
        private readonly string connectionString;

        public TipoUsuariosController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public IActionResult LogInUsuarios()
        {
            return View(); 
        }

        [HttpPost]
        public IActionResult LogInUsuarios(TipoUsuarios tipoUsuarios)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = connection.Query("SELECT 1").FirstOrDefault();
            }

            return View();
        }

    }
}