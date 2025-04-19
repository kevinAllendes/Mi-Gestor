using Microsoft.AspNetCore.Mvc;
using Gestor.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using Gestor.Servicios;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Gestor.Controllers
{
    public class TipoUsuariosController : Controller
    {
        private readonly string connectionString;
        private readonly IRepositorioUsuarios repositorioUsuarios;
        private readonly UserManager<TipoUsuarios> userManager;
        private readonly SignInManager<TipoUsuarios> signInManager;
        public TipoUsuariosController(IConfiguration configuration, IRepositorioUsuarios repositorioUsuarios, UserManager<TipoUsuarios> userManager,
            SignInManager<TipoUsuarios> signInManager)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            this.repositorioUsuarios = repositorioUsuarios;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [AllowAnonymous]
        public IActionResult LogInUsuarios()
        {
            return View(); 
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LogInUsuarios(LoginViewModel modelo)
        {
            if(!ModelState.IsValid)
            {
                return View(modelo);
            }
            var resultado = await signInManager.PasswordSignInAsync(modelo.Email,
            modelo.Password, modelo.Recuerdame, lockoutOnFailure: false);

            if(resultado.Succeeded)
            {
                return RedirectToAction("Index","Transacciones");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o password incorrecto!");
                return View(modelo);

            }
        }

        [HttpGet]
        public async Task<IActionResult> LogoutUsuarios()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return View("Index","Transaccioens");
        }

        //Devolvemos el formulario de registro
        [AllowAnonymous]
        public IActionResult Registro()
        {
            return View();
        }
        [AllowAnonymous]
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
            var usuario = new TipoUsuarios() {Email = modelo.Email};
            var resultado = await userManager.CreateAsync(usuario, password: modelo.Password);

            if(resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index","Transacciones");
            }
            else
            {
                foreach(var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            return View(modelo);
            }
        }
    }
}