using Microsoft.AspNetCore.Mvc;
using Blog.Services;
using Blog.ViewModels;
using Blog.Data;
using Blog.Extensions;
using Blog.Models;


namespace Blog.Controllers
{

    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost("v1/accounts/login")]
        public async Task<IActionResult> Post(
            [FromBody] RegisterViewModel model,
            [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-").Replace(".", "-")
            };

        }

        [HttpPost("v1/login")]
        public IActionResult Login([FromServices] TokenService tokenService) // dependem do item TokenService
        {
            var token = tokenService.GenerateToken(null); // gerando token baseado no tokenService

            return Ok(token);
        }


    }
}