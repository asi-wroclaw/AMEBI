using System;
using System.Threading.Tasks;
using AMEBI.Domain.Services;
using AMEBI.Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMEBI.Mvc.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwt;

        public AccountController(IUserService userService, IJwtService jwt)
        {
            _userService = userService;
            _jwt = jwt;
        }
        
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {   
            var user = await _userService.FindAsync(model.Username);

            if(user == null)
            {
                await _userService.AddAsync(model.Username, model.Password);
                user = await _userService.FindAsync(model.Username);
            }

            _userService.LoginAsync(model.Username, model.Password);

            var token = _jwt.CreateToken(user.Id ,"user");

            return Json(token);
        }
    }
}