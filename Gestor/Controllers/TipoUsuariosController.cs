using Microsoft.AspNetCore.Mvc;
using Gestor.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using Gestor.Servicios;

namespace Gestor.Controllers
{
    public class TipoUsuariosController : Controller
    {
        private readonly string connectionString;

        private readonly IRepositorioUsuarios repositorioUsuarios;

        public TipoUsuariosController(IConfiguration configuration, IRepositorioUsuarios repositorioUsuarios)
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
            await repositorioUsuarios.BuscarUsuario(tipoUsuarios);
            if(tipoUsuarios.IdUser != null )
            {
                RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public IActionResult LogoutUsuarios()
        {
            return View();
        }
       
       

    }
}