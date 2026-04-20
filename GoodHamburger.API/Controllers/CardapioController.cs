using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers
{
    public class CardapioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
