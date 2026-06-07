using Microsoft.AspNetCore.Mvc;

namespace EnvironmentExample.Controllers
{
    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
