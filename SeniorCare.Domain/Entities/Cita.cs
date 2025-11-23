using System;

namespace SeniorCare.Domain.Entities
{
    public class Cita
    {
        public Guid Id { get; set; }
        public Guid PacienteId { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }

        public string Proposito { get; set; } = string.Empty;

        
        public Paciente Paciente { get; set; } = default!;
    }
}

