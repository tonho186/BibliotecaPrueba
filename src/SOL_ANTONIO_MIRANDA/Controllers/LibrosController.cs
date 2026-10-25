using Microsoft.AspNetCore.Mvc;

namespace SOL_ANTONIO_MIRANDA.Controllers
{
    public class LibrosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
