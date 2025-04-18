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
            this.repositorioUsuarios = repositorioUsuarios;
        }
        public IActionResult LogInUsuarios()
        {
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> LogInUsuarios(TipoUsuarios tipoUsuarios)
        {
            var existe = await repositorioUsuarios.BuscarUsuario(tipoUsuarios);
            if(existe)
            {
                return RedirectToAction("Index","Home");
            }
            return View();
        }

        [HttpGet]
        public IActionResult LogoutUsuarios()
        {
            return View();
        }

        //Devolvemos el formulario de registro
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            /**
                Este metodo se utilizo con el framework identity
            */
            if(!ModelState.IsValid)
            {
                return View(modelo);
            }
            
            return RedirectToAction("Index","Transacciones");
        }

        

    }
}