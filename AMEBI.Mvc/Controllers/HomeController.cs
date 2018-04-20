using Microsoft.AspNetCore.Mvc;

namespace AMEBI.Mvc.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
