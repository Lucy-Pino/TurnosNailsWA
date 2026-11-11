using System;
using System.Collections.Generic;
using System.Text;

namespace TurnosNailsWA.Shared.DTO
{
    public class ServicioListadoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Categoria { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public int Duracion { get; set; }
        public decimal Precio { get; set; }

    }
}
