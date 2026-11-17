using System;
using System.Collections.Generic;
using System.Text;

namespace TurnosNailsWA.Shared.DTO
{
    public class TurnoListadoDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Estado { get; set; } = "";
        public string ServicioNombre { get; set; } = "";
        public string Categoria { get; set; } = "";
        public int Duracion { get; set; }
        public decimal Precio { get; set; }
        public string CodigoReserva { get; set; } = "";
        public string ClienteNombre { get; set; } = "";

        /// <summary>
        /// Representación estándar obligatoria: CATEGORÍA – NOMBRE DEL SERVICIO.
        /// Permite distinguir unívocamente servicios homónimos de MANOS y PIES.
        /// </summary>
        public string ServicioCompleto =>
            string.IsNullOrWhiteSpace(Categoria)
                ? ServicioNombre
                : $"{Categoria.ToUpper()} – {ServicioNombre}";

        /// <summary>
        /// Hora de finalización calculada a partir de la hora de inicio y la duración del servicio.
        /// </summary>
        public TimeSpan HoraFin => Hora.Add(TimeSpan.FromMinutes(Duracion > 0 ? Duracion : 30));
    }
}