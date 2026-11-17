using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

namespace TurnosNailsWA.Cliente.Servicios
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient http;

        public CustomAuthStateProvider(HttpClient http)
        {
            this.http = http;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var estaAutenticado = await http.GetFromJsonAsync<bool>("api/auth/estado");
                if (estaAutenticado)
                {
                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, "Administradora"),
                        new Claim(ClaimTypes.Role, "Administradora")
                    }, "CookieAuth");

                    return new AuthenticationState(new ClaimsPrincipal(identity));
                }
            }
            catch
            {
                // Si la llamada falla o no hay conexión, se considera anónimo.
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public void NotificarEstado()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
