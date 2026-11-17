using System;
using System.Linq;

namespace TurnosNailsWA.Shared.DTO
{
    /// <summary>
    /// Utilidad para validar y normalizar números de teléfono argentinos.
    /// Garantiza un formato nacional de 10 dígitos (área sin 0 y número sin 15)
    /// y genera el formato internacional requerido para enlaces de WhatsApp.
    /// </summary>
    public static class TelefonoHelper
    {
        /// <summary>
        /// Valida y normaliza un número de teléfono argentino a un formato nacional de 10 dígitos.
        /// Acepta formatos con/sin +54, con/sin 9, con/sin 0, con/sin 15, guiones, espacios y paréntesis.
        /// Rechaza números incompletos o con dígitos excedentes.
        /// </summary>
        public static bool ValidarYNormalizar(string? input, out string telefonoNormalizado, out string error)
        {
            telefonoNormalizado = "";
            error = "";

            if (string.IsNullOrWhiteSpace(input))
            {
                error = "El teléfono es obligatorio.";
                return false;
            }

            // 1. Extraer exclusivamente dígitos numéricos
            string digitos = new string(input.Where(char.IsDigit).ToArray());

            if (string.IsNullOrEmpty(digitos))
            {
                error = "El teléfono debe contener únicamente dígitos numéricos.";
                return false;
            }

            // 2. Quitar prefijo internacional argentino (+549 o 549)
            if (digitos.StartsWith("549") && digitos.Length >= 13)
            {
                digitos = digitos.Substring(3);
            }
            // Quitar +54 o 54
            else if (digitos.StartsWith("54") && digitos.Length >= 12)
            {
                digitos = digitos.Substring(2);
                if (digitos.StartsWith("9") && digitos.Length >= 11)
                {
                    digitos = digitos.Substring(1);
                }
            }

            // 3. Quitar prefijo interurbano '0' inicial si estuviera presente
            if (digitos.StartsWith("0"))
            {
                digitos = digitos.Substring(1);
            }

            // 4. Manejar el prefijo móvil '15' si fue ingresado en números de 12 dígitos
            // En Argentina: Área (2, 3 o 4 dígitos) + 15 + Local (8, 7 o 6 dígitos) = 12 dígitos
            if (digitos.Length == 12)
            {
                // Código de área 11 (AMBA) + 15 + 8 dígitos
                if (digitos.StartsWith("1115"))
                {
                    digitos = "11" + digitos.Substring(4);
                }
                // Códigos de área de 3 dígitos (ej: 351, 341, 261, 381...) + 15 + 7 dígitos
                else if (digitos.Length >= 5 && digitos.Substring(3, 2) == "15")
                {
                    digitos = digitos.Substring(0, 3) + digitos.Substring(5);
                }
                // Códigos de área de 4 dígitos (ej: 2966, 3873...) + 15 + 6 dígitos
                else if (digitos.Length >= 6 && digitos.Substring(4, 2) == "15")
                {
                    digitos = digitos.Substring(0, 4) + digitos.Substring(6);
                }
            }

            // 5. El número normalizado debe tener EXACTAMENTE 10 dígitos
            if (digitos.Length < 10)
            {
                error = $"El teléfono ingresado está incompleto ({digitos.Length} dígitos). Debe tener 10 dígitos (código de área sin 0 y número sin 15. Ej: 3515548895).";
                return false;
            }

            if (digitos.Length > 10)
            {
                error = $"El teléfono ingresado tiene dígitos adicionales ({digitos.Length} dígitos). Debe tener exactamente 10 dígitos (ej: 3515548895).";
                return false;
            }

            // 6. Validación de códigos de área argentinos válidos (comienzan con 11, 2 o 3)
            if (!digitos.StartsWith("11") && !digitos.StartsWith("2") && !digitos.StartsWith("3"))
            {
                error = "El código de área ingresado no es válido para Argentina (debe comenzar con 11, 2 o 3).";
                return false;
            }

            telefonoNormalizado = digitos;
            return true;
        }

        /// <summary>
        /// Genera el formato internacional requerido para enlaces wa.me: 549 + 10 dígitos.
        /// </summary>
        public static string FormatoWhatsApp(string? telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono)) return "";

            if (ValidarYNormalizar(telefono, out var normalizado, out _))
            {
                return "549" + normalizado;
            }

            // Si ya venía con formato internacional o dígitos válidos
            string digitos = new string(telefono.Where(char.IsDigit).ToArray());
            if (digitos.StartsWith("549") && digitos.Length == 13)
            {
                return digitos;
            }
            if (digitos.Length == 10)
            {
                return "549" + digitos;
            }

            return digitos;
        }
    }
}