using System.IdentityModel.Tokens.Jwt;
using Blog.Models;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Services
{
    public class TokenServices // Classe que vai gerar Token
    {
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor(); // Item que vai conter todas as informações do token.
            var token = tokenHandler.CreateToken(tokenDescriptor); // item que vai criar o token
            return tokenHandler.WriteToken(token); // vai retornar uma string
        }
    }
}