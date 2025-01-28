using Gestor.Models;
using Gestor.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Gestor.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IRepositorioUsuarios usuarios;
        public CategoriasController(IRepositorioCategorias repositorioCategorias, IRepositorioUsuarios repositorioUsuarios) 
        { 
            this.repositorioCategorias = repositorioCategorias;
            this.usuarios = repositorioUsuarios;
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if(!ModelState.IsValid)
            {
                return View(categoria);
            }

            var usuarioId = usuarios.ObtenerUsuarioId();
            categoria.UsuarioId = usuarioId;
            await repositorioCategorias.Crear(categoria);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = usuarios.ObtenerUsuarioId();
            var misCategorias = await repositorioCategorias.Obtener(usuarioId);
            return View(misCategorias);
        }

    }
}
