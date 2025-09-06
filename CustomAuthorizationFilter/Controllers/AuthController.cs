using CustomAuthorizationFilter.DTOs;
using CustomAuthorizationFilter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CustomAuthorizationFilter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("public")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            var user = UserStore.Users.FirstOrDefault(u => u.Password == loginDTO.Password 
                                                      && u.Email.Equals(loginDTO.Email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("UserId", user.Id.ToString()),
                new Claim("SubscriptionLevel", user.SubscriptionLevel ?? "Free"),
                new Claim("Department", user.Department ?? "None")
            };

            if (user.SubscriptionExpiresOn != null)
                claims.Add(new Claim("SubscriptionExpiresOn", user.SubscriptionExpiresOn.Value.ToString()));

            // Read the JWT secret key from configuration; fallback to hardcoded key if missing (for development)
            var secretKey = _configuration.GetValue<string>("JwtSettings:SecretKey") ?? "d06e9577107f86a83a2698ce2fa7a998c94e3ed45eefbb65";

            // Convert the secret key string into a byte array and create a SymmetricSecurityKey instance
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // Create signing credentials specifying the key and the HMAC SHA256 algorithm
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create the JWT token object with claims, expiration (30 mins), and signing credentials
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            // Serialize the JWT token to a string
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { Token = tokenString });

        }
    }
}
