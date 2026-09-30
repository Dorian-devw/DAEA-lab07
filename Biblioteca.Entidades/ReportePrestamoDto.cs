using System;

namespace Biblioteca.Entidades
{
    public class ReportePrestamoDto
    {
        public string Socio { get; set; }
        public string TituloLibro { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaLimite { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public string Estado { get; set; }
    }
}
