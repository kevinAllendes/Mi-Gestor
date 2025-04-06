using Gestor.Models;
using Gestor.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Gestor.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IRepositorioUsuarios repositorioUsuarios;
        public CategoriasController(IRepositorioCategorias repositorioCategorias, IRepositorioUsuarios repositorioUsuarios) 
        { 
            this.repositorioCategorias = repositorioCategorias;
            this.repositorioUsuarios = repositorioUsuarios;
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categorias categoria)
        {
            if(!ModelState.IsValid)
            {
                return View(categoria);
            }

            var usuarioId = repositorioUsuarios.ObtenerUsuarioId();
            categoria.UsuarioId = usuarioId;
            await repositorioCategorias.Crear(categoria);
            return RedirectToAction("Index");
        }


        //Retornamos la vista de edicion de categorias a partir de su id
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = repositorioUsuarios.ObtenerUsuarioId();
            var categoria = await repositorioCategorias.ObtenerPorId(id, usuarioId);

            if(categoria is null)
            {
                return RedirectToAction("No encontrados","Home");
            }
            return View(categoria);

        }

        //Retornamos la vista de indice de categorias luego de la edicion satisfactoria
        [HttpPost]
        public async Task<IActionResult> Editar(Categorias categoria)
        {
            var usuarioId = repositorioUsuarios.ObtenerUsuarioId();
            var categoriaAEditar =  await repositorioCategorias.ObtenerPorId(categoria.Id,usuarioId);
            if(categoria is null)
            {
                return RedirectToAction("No encontrados","Home");
            }
            
            categoriaAEditar.UsuarioId = usuarioId;
            await repositorioCategorias.Actualizar(categoriaAEditar);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId =  repositorioUsuarios.ObtenerUsuarioId();
            var categoriaABorrar =  await repositorioCategorias.ObtenerPorId(id, usuarioId);
            if(categoriaABorrar is null)
            {
                return RedirectToAction("No encontrados","Home");
            }
            
            return View("Borrar");
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCategorias(Categorias categoria)
        {
            var usuarioId = repositorioUsuarios.ObtenerUsuarioId();
            var categoriaABorrar =  await repositorioCategorias.ObtenerPorId(categoria.Id,usuarioId);
            if(categoriaABorrar is null)
            {
                return RedirectToAction("No encontrados","Home");
            }
            
            categoriaABorrar.UsuarioId = usuarioId;
            await repositorioCategorias.Borrar(categoriaABorrar.Id);
            return RedirectToAction("Index");

        }



        public async Task<IActionResult> Index()
        {
            var usuarioId = repositorioUsuarios.ObtenerUsuarioId();
            var misCategorias = await repositorioCategorias.Obtener(usuarioId);
            return View(misCategorias);
        }

    }
}
