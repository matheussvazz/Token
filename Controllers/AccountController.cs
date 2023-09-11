using Microsoft.AspNetCore.Mvc;
using Blog.Services;
using Blog.ViewModels;
using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using SecureIdentity.Password;
using Microsoft.EntityFrameworkCore;


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

            var password = PasswordGenerator.Generate(25); // Vai gerar uma senha
            user.PasswordHash = PasswordHasher.Hash(password); // vai gerar um hash dessa senha

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    user = user.Email,
                    password
                }));
            }
            catch (DbUpdateException)
            {
                return StatusCode(400, new ResultViewModel<string>("05X99 - Este E-mail já está cadastrado"));
            }
        }


        [HttpPost("v1/login")]
        public IActionResult Login([FromServices] TokenService tokenService) // dependem do item TokenService
        {
            var token = tokenService.GenerateToken(null); // gerando token baseado no tokenService

            return Ok(token);
        }


    }
}