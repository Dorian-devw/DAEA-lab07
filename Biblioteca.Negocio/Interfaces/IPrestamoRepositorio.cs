using Biblioteca.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biblioteca.Negocio.Interfaces
{
    public interface IPrestamoRepositorio
    {
        Task<bool> RegistrarPrestamoAsync(Prestamo prestamo, List<int> librosIds);
        Task<bool> RegistrarDevolucionAsync(int prestamoId, int libroId, DateTime fechaDevolucion);
        Task<int> ObtenerLibrosPendientesPorSocioAsync(int socioId);
        Task<bool> ExistePrestamoPendienteParaLibroAsync(int libroId);
        Task<bool> ExistePrestamoPendienteParaSocioAsync(int socioId);
        Task<List<Prestamo>> ObtenerPrestamosConDetalleAsync(int socioId);
        // Para reportes
        Task<List<ReportePrestamoDto>> ObtenerReportePrestamosAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}
