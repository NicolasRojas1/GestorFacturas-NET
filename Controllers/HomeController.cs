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

    // 2. Acción para mostrar los formularios
    // Este método busca el archivo "RegistroGas.cshtml" en Views/Home
    public IActionResult RegistroGas()
    {
        return View();
    }

    public IActionResult RegistroLuz()
    {
        return View();
    }

    public IActionResult RegistroAgua()
    {
        return View();
    }

    // Acción para recibir los datos
    [HttpPost]
    public IActionResult ProcesarGas(Gas recibo)
    {
        if (!ModelState.IsValid)
        {
            return View("RegistroGas", recibo);
        }
        return View("ResultadoGas", recibo); // Enviamos el objeto recibo a la vista ResultadoGas
    }

    [HttpPost]
    public IActionResult ProcesarLuz(Luz recibo)
    {
        if (!ModelState.IsValid)
        {
            return View("RegistroLuz", recibo);
        }
        return View("ResultadoLuz", recibo);
    }
    [HttpPost]
    public IActionResult ProcesarAgua(Agua recibo)
    {
        if (!ModelState.IsValid)
        {
            return View("RegistroAgua", recibo);
        }
        return View("ResultadoAgua", recibo);
    }

    // 3. Opcional, Acción para manejar errores
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }



}
