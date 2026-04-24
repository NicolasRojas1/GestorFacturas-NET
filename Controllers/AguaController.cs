using AppFacturas.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppFacturas.Controllers
{
    public class AguaController : Controller
    {
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Resultado(Agua recibo)
        {
            if (!ModelState.IsValid)
            {
                return View("Registro", recibo);
            }
            return View(recibo);
        }
    }
}
