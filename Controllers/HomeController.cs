using AppFacturas.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AppFacturas.Controllers;

// Esta es la clase principal
public class HomeController : Controller
{
    // 1. La página de inicio
    public IActionResult Index()
    {
        return View();
    }


    // Opcional, Acción para manejar errores
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }



}
