using AMEBI.Domain.Services;
using AMEBI.WebApi.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMEBI.WebApi.Controllers
{
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public AccountController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public IActionResult Login([FromBody]LoginViewModel model)
        {
            var user = _userService.Login(model.Username, model.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            var token = _jwtService.CreateToken(user.Id, "user");

            return new JsonResult(token);
        }
    }
}