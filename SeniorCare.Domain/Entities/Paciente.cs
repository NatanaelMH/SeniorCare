using System;
using System.Collections.Generic;

namespace SeniorCare.Domain.Entities
{
    public class Paciente
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string TipoSangre { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;

        public DateTime FechaNacimiento { get; set; }

        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}
