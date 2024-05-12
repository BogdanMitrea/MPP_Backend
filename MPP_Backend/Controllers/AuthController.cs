using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MPP_Backend.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MPP_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthRepository _authrepository;
        public AuthController(AuthRepository _authrepository)
        {
            this._authrepository = _authrepository;

        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var isValidUser = this._authrepository.Login(model.Username, model.Password);
            if (!isValidUser)
            {
                return Unauthorized();
            }
            var token = this._authrepository.GetTokenByUsername(model.Username);

            if (!string.IsNullOrEmpty(token))
            {
                return Ok(new { token });
            }

            token = GenerateJwtToken(model.Username,model.Email);
            this._authrepository.SaveOrUpdateToken(model.Username, token);

            return Ok(new { token });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] LoginModel model)
        {
            return Ok(this._authrepository.Register(model.Username, model.Password, model.Email));
        }

        private string GenerateJwtToken(string username,string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("poiuytrewqghfjdkslamznxbcvqwertyuiop"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("email", email)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
