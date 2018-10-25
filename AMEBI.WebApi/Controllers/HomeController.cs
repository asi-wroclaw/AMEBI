using Microsoft.AspNetCore.Mvc;

namespace AMEBI.WebApi.Controllers
{
    public class HomeController : ControllerBase
    {
        [Route("")]
        public ActionResult Get()
        {
            return new JsonResult("value");
        }
    }
}
