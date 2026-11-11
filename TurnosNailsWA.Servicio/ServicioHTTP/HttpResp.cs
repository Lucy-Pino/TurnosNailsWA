using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace TurnosNailsWA.Servicio.ServicioHTTP
{
    public class HttpResp<TResp>
    {
        public bool Error { get; set; }
        public TResp? Respuesta { get; set; }
        public HttpResponseMessage? Response { get; set; }
        private string? _mensaje;
        public string? Mensaje
        {
            get => !string.IsNullOrEmpty(_mensaje) ? _mensaje : ObtenerError();
            set => _mensaje = value;
        }

        public HttpResp(TResp? dato, bool error, HttpResponseMessage? response, string? mensaje)
        {
            Respuesta = dato;
            Error = error;
            Response = response;
            Mensaje = mensaje;
        }

        /// <summary>
        /// Intenta leer el mensaje que devolvio el servidor en una respuesta no exitosa.
        /// Si el cuerpo esta vacio, no es legible o no contiene un mensaje aprovechable,
        /// devuelve null para que se use el mensaje generico segun el codigo HTTP.
        /// Un error al leer el cuerpo nunca interrumpe el manejo de la respuesta.
        /// </summary>
        public static async Task<string?> LeerMensajeAsync(HttpResponseMessage? response)
        {
            if (response == null || response.IsSuccessStatusCode)
            {
                return null;
            }

            try
            {
                var contenido = await response.Content.ReadAsStringAsync();
                return ObtenerMensajeAprovechable(contenido);
            }
            catch
            {
                return null;
            }
        }

        private static string? ObtenerMensajeAprovechable(string? contenido)
        {
            if (string.IsNullOrWhiteSpace(contenido))
            {
                return null;
            }

            var texto = contenido.Trim();

            if (texto.Length == 0 || texto.StartsWith("<"))
            {
                return null;
            }

            // El cuerpo puede ser un string JSON: "mensaje"
            if (texto.StartsWith("\"") && texto.EndsWith("\""))
            {
                try
                {
                    var cadena = JsonSerializer.Deserialize<string>(texto);
                    if (!string.IsNullOrWhiteSpace(cadena))
                    {
                        return cadena;
                    }
                }
                catch
                {
                    // Si no se puede deserializar se sigue con el texto original.
                }

                return null;
            }

            // El cuerpo puede ser un objeto JSON (ProblemDetails o similar).
            if (texto.StartsWith("{"))
            {
                try
                {
                    using var documento = JsonDocument.Parse(texto);
                    foreach (var nombre in new[] { "mensaje", "message", "title" })
                    {
                        if (documento.RootElement.TryGetProperty(nombre, out var valor)
                            && valor.ValueKind == JsonValueKind.String)
                        {
                            var cadena = valor.GetString();
                            if (!string.IsNullOrWhiteSpace(cadena))
                            {
                                return cadena;
                            }
                        }
                    }

                    return null;
                }
                catch
                {
                    return texto;
                }
            }

            return texto;
        }

        public string ObtenerError()
        {
            if (_mensaje != null && _mensaje != string.Empty)
            {
                return _mensaje;
            }

            if (Response == null)
            {
                return "No se obtuvo respuesta del servidor.";
            }
            else if (Response.IsSuccessStatusCode)
            {
                return string.Empty;
            }
            else
            {
                string mensaje = "";
                var statusCode = Response.StatusCode;

                #region Mensaje segun StatusCode
                switch (statusCode)
                {
                    case System.Net.HttpStatusCode.Continue:
                        break;
                    case System.Net.HttpStatusCode.SwitchingProtocols:
                        break;
                    case System.Net.HttpStatusCode.Processing:
                        break;
                    case System.Net.HttpStatusCode.EarlyHints:
                        break;
                    case System.Net.HttpStatusCode.OK:
                        mensaje = "Solicitud exitosa.";
                        break;
                    case System.Net.HttpStatusCode.Created:
                        break;
                    case System.Net.HttpStatusCode.Accepted:
                        break;
                    case System.Net.HttpStatusCode.NonAuthoritativeInformation:
                        break;
                    case System.Net.HttpStatusCode.NoContent:
                        break;
                    case System.Net.HttpStatusCode.ResetContent:
                        break;
                    case System.Net.HttpStatusCode.PartialContent:
                        break;
                    case System.Net.HttpStatusCode.MultiStatus:
                        break;
                    case System.Net.HttpStatusCode.AlreadyReported:
                        break;
                    case System.Net.HttpStatusCode.IMUsed:
                        break;
                    case System.Net.HttpStatusCode.Ambiguous:
                        break;
                    case System.Net.HttpStatusCode.Moved:
                        break;
                    case System.Net.HttpStatusCode.Found:
                        break;
                    case System.Net.HttpStatusCode.RedirectMethod:
                        break;
                    case System.Net.HttpStatusCode.NotModified:
                        break;
                    case System.Net.HttpStatusCode.UseProxy:
                        break;
                    case System.Net.HttpStatusCode.Unused:
                        break;
                    case System.Net.HttpStatusCode.RedirectKeepVerb:
                        break;
                    case System.Net.HttpStatusCode.PermanentRedirect:
                        mensaje = "Redirección permanente.";
                        break;
                    case System.Net.HttpStatusCode.BadRequest:
                        mensaje = "Solicitud incorrecta - Intente nuevamente.";
                        break;
                    case System.Net.HttpStatusCode.Unauthorized:
                        mensaje = "No autorizado.";
                        break;
                    case System.Net.HttpStatusCode.PaymentRequired:
                        break;
                    case System.Net.HttpStatusCode.Forbidden:
                        mensaje = "Acceso denegado.";
                        break;
                    case System.Net.HttpStatusCode.NotFound:
                        mensaje = "Recurso no encontrado.";
                        break;
                    case System.Net.HttpStatusCode.MethodNotAllowed:
                        break;
                    case System.Net.HttpStatusCode.NotAcceptable:
                        break;
                    case System.Net.HttpStatusCode.ProxyAuthenticationRequired:
                        break;
                    case System.Net.HttpStatusCode.RequestTimeout:
                        break;
                    case System.Net.HttpStatusCode.Conflict:
                        break;
                    case System.Net.HttpStatusCode.Gone:
                        break;
                    case System.Net.HttpStatusCode.LengthRequired:
                        break;
                    case System.Net.HttpStatusCode.PreconditionFailed:
                        break;
                    case System.Net.HttpStatusCode.RequestEntityTooLarge:
                        break;
                    case System.Net.HttpStatusCode.RequestUriTooLong:
                        break;
                    case System.Net.HttpStatusCode.UnsupportedMediaType:
                        break;
                    case System.Net.HttpStatusCode.RequestedRangeNotSatisfiable:
                        break;
                    case System.Net.HttpStatusCode.ExpectationFailed:
                        break;
                    case System.Net.HttpStatusCode.MisdirectedRequest:
                        break;
                    case System.Net.HttpStatusCode.UnprocessableEntity:
                        break;
                    case System.Net.HttpStatusCode.Locked:
                        break;
                    case System.Net.HttpStatusCode.FailedDependency:
                        break;
                    case System.Net.HttpStatusCode.UpgradeRequired:
                        break;
                    case System.Net.HttpStatusCode.PreconditionRequired:
                        break;
                    case System.Net.HttpStatusCode.TooManyRequests:
                        break;
                    case System.Net.HttpStatusCode.RequestHeaderFieldsTooLarge:
                        break;
                    case System.Net.HttpStatusCode.UnavailableForLegalReasons:
                        break;
                    case System.Net.HttpStatusCode.InternalServerError:
                        break;
                    case System.Net.HttpStatusCode.NotImplemented:
                        break;
                    case System.Net.HttpStatusCode.BadGateway:
                        break;
                    case System.Net.HttpStatusCode.ServiceUnavailable:
                        break;
                    case System.Net.HttpStatusCode.GatewayTimeout:
                        break;
                    case System.Net.HttpStatusCode.HttpVersionNotSupported:
                        break;
                    case System.Net.HttpStatusCode.VariantAlsoNegotiates:
                        break;
                    case System.Net.HttpStatusCode.InsufficientStorage:
                        break;
                    case System.Net.HttpStatusCode.LoopDetected:
                        break;
                    case System.Net.HttpStatusCode.NotExtended:
                        break;
                    case System.Net.HttpStatusCode.NetworkAuthenticationRequired:
                        break;
                    default:
                        break;
                }
                #endregion

                return mensaje;
            }
        }
    }
}
