using Gestor.Models;
using Gestor.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Gestor.Controllers{

    public class TiposCuentasController : Controller
    {
        //Definimos en el contructor del controlador el repositorio como campo para utilizarlo
        private readonly IRepositorioTipoCuentas repositorioTipoCuentas;

        private readonly IRepositorioUsuarios repositorioUsuarios;
        public TiposCuentasController(IRepositorioTipoCuentas repositorioTipoCuentas, IRepositorioUsuarios repositorioUsuarios)
        {
            this.repositorioTipoCuentas = repositorioTipoCuentas;

            this.repositorioUsuarios = repositorioUsuarios;

        }

        public async Task<IActionResult> IndiceDeCuentas()
        {
            var usuarioID = repositorioUsuarios.ObtenerUsuarioId();
            var TiposCuentas = repositorioTipoCuentas.obtenerCuentasSinBDD(usuarioID);
            //var TiposCuentas = await repositorioTipoCuentas.Obtener(usuarioID);
            return View(TiposCuentas);
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
            miCuenta.UsuarioId = repositorioUsuarios.ObtenerUsuarioId();
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

        /* 
            Vamos a crear un metodo con una verificacion personalizada
            utilizando Json (informacion que mandamos desde el navegador)
        */
        [HttpGet]
        public async Task<IActionResult> VerificarExisteCuenta(string nombre)
        {
            var usuarioId = repositorioUsuarios.ObtenerUsuarioId();
            var yaExisteTipoCuenta = await repositorioTipoCuentas.Existe(nombre,usuarioId);
            if(yaExisteTipoCuenta)
            {
                //Formato para representar datos como cadena de texto para comunicacion entre C# y JavaScript
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);
        }
        
        //Defino un metodo que me permita editar una cuenta
        [HttpGet]
        public async Task<IActionResult> Editar(int idCuentaAEditar)
        {
            var usuarioId = repositorioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTipoCuentas.ObtenerPorId(idCuentaAEditar,usuarioId);
             if (tipoCuenta is null)
             {
                return RedirectToAction("No Encontrado","Home");
             }
             return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioId =  repositorioUsuarios.ObtenerUsuarioId();
            //Verifico un modelo valido
            if(!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }
            //Verifico que la cuenta exista asociada al id del usuario 
            var tipoCuentaExiste = await repositorioTipoCuentas.ObtenerPorId(tipoCuenta.Id,usuarioId);
            if(tipoCuentaExiste is null)
            {
                return RedirectToAction("No Encontrado","Home");
            }
            /*Se Actualiza la cuenta*/
            await repositorioTipoCuentas.Actualizar(tipoCuenta);
            
            /*Se redirige hacia el indice*/
            return RedirectToAction("IndiceDeCuentas");

        }

        public async Task<IActionResult> Borrar(int id)
        /*
            Creamos un metodo en el cual abrimos la pagina de borrado en donde
            solicitamos la confirmacion del usuario
        */
        {
            var usuarioId = repositorioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTipoCuentas.ObtenerPorId(id,usuarioId);
            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }
            return View(tipoCuenta);
        }

        /*
            Creamos el metodo en donde se realizara le borrado de la cuenta y luego 
            redirigimos a la vista de indice de cuentas
        */
        [HttpPost]
        public async Task<IActionResult> BorradoDeCuenta(int id)
        {
            await repositorioTipoCuentas.Borrar(id);
            return RedirectToAction("IndiceDeCuentas");
        }




    }
}