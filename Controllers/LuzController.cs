using AppFacturas.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppFacturas.Controllers
{
    public class LuzController : Controller
    {
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Resultado(Luz recibo)
        {
            if (!ModelState.IsValid)
            {
                return View("Registro", recibo);
            }
            return View(recibo);
        }
    }
}
