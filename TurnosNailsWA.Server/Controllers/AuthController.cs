using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TurnosNailsWA.Shared.DTO;

namespace TurnosNailsWA.Server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<bool>> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos de acceso incompletos.");
            }

            var adminUser = configuration["AdminAccount:Usuario"] ?? "admin@elibeautycare.com";
            var adminPassword = configuration["AdminAccount:Password"];

            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                return StatusCode(500, "La contraseña de administración no está configurada.");
            }

            if (!string.Equals(dto.Usuario?.Trim(), adminUser.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            if (dto.Password != adminPassword)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, adminUser),
                new Claim(ClaimTypes.Role, "Administradora")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok(true);
        }

        [HttpPost("logout")]
        public async Task<ActionResult<bool>> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(true);
        }

        [HttpGet("estado")]
        public ActionResult<bool> Estado()
        {
            var autenticado = User.Identity?.IsAuthenticated == true;
            return Ok(autenticado);
        }
    }
}
