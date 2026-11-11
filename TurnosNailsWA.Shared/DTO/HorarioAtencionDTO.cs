using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TurnosNailsWA.Shared.DTO
{
    public class HorarioAtencionDTO : IValidatableObject
    {
        public int Id { get; set; }

        // 0 = Domingo, 1 = Lunes, 2 = Martes, 3 = Miércoles,
        // 4 = Jueves, 5 = Viernes, 6 = Sábado
        [Range(0, 6, ErrorMessage = "El día de la semana debe estar entre 0 (domingo) y 6 (sábado).")]
        public int DiaSemana { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        public bool Activo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HoraFin <= HoraInicio)
            {
                yield return new ValidationResult(
                    "La hora de fin debe ser mayor que la hora de inicio.",
                    new[] { nameof(HoraFin) });
            }
        }
    }
}
