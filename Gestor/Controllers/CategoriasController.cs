using Gestor.Models;
using Microsoft.AspNetCore.Mvc;
using Gestor.Servicios;

namespace Gestor.Controllers{

    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategorias RepositorioCategorias;
        public CategoriasController(IRepositorioCategorias repositorioCategorias)
        {
            this.RepositorioCategorias = repositorioCategorias;
        }


    }

}
