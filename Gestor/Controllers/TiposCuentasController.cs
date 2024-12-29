using Gestor.Models;
using Gestor.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Gestor.Controllers{

    public class TiposCuentasController : Controller
    {
        //Definimos en el contructor del controlador el repositorio como campo para utilizarlo
        private readonly IRepositorioTipoCuentas repositorioTipoCuentas;
        public TiposCuentasController(IRepositorioTipoCuentas repositorioTipoCuentas)
        {
            this.repositorioTipoCuentas = repositorioTipoCuentas;

        }
        public IActionResult Crear()
        {
            return View();
        }
        
        //Accion que responde al posteo de la informacion
        //como esta funcion devuelve algo de froma asyncrona. 
        //Lo que devuelve se tiene que encapsular entre corchetes
        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta miCuenta)
        {
            if(!ModelState.IsValid)
            //Si el modelo es invalido
            {
                return View(miCuenta);
            }
            miCuenta.UsuarioId = 1;
            //Se utiliza await por que Crear es un metodo asyncrono
            
            /*Verifico que el tipo de cuenta no existe. Si existe mandamos un error de modelo*/
            var existe = await repositorioTipoCuentas.Existe(miCuenta.Nombre,miCuenta.UsuarioId);
            if(!existe)
            {
                //Enviamos el error a nivel de modelo
                ModelState.AddModelError(nameof(TipoCuenta.Nombre),
                $"El nombre {miCuenta.Nombre} ya existe");
                return View(miCuenta);
                
            }
            await repositorioTipoCuentas.Crear(miCuenta);

            return View();
        }

    }
}