using Microsoft.AspNetCore.Mvc;
using AppFacturas.Models;

namespace AppFacturas.Controllers
{
    public class GasController : Controller
    {
        // Este método busca el archivo "Registro.cshtml" en Views/Home/Gas
        public IActionResult Registro()
        {
            return View();
        }
    

        // Acción para recibir los datos
        [HttpPost]
        public IActionResult Resultado(Gas recibo)
        {
            if (!ModelState.IsValid)
            {
                return View("Registro", recibo); //Si hay error hasta aqui llega
            }
            recibo.ProcesarFactura();
            return View(recibo); // Al no poner nombre, busca 'Resultado.cshtml' dentro de Views/Gas/
        }
    }
}
