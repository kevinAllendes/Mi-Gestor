using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using MiGestor.Models;
using MiGestor.Servicios;

namespace MiGestor.Controllers;

public class TipoClientesController : Controller
{
    public TipoClientesController()
    {

    } 

    public IActionResult VerCliente(TipoClientes cliente)
    {
        return View();
    }
}