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
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no Servidor"));
            }
        }


        //fazer autenticação 
        // recuperar usuario no banco e comparar a senha dele
        public async Task<IActionResult> Login(
         [FromBody] LoginViewModel model,
         [FromServices] BlogDataContext context,
         [FromServices] TokenService tokenService) // este item depende do token service 
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = await context
              .Users
              .AsNoTracking()
              .Include(x => x.Roles)
              .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)

                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inaválido"));


            if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inaválido"));


            try
            {
                var token = tokenService.GenerateToken(user);
                return Ok(new ResultViewModel<string>(token, null));

            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05x04 - Falha interna no Servidor"));
            }

        }


    }
}