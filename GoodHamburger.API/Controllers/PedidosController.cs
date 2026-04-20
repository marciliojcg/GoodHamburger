using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers
{
    public class PedidosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
