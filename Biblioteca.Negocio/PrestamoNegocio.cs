using Biblioteca.Entidades;
using Biblioteca.Negocio.Excepciones;
using Biblioteca.Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biblioteca.Negocio
{
    public class PrestamoNegocio
    {
        private readonly IPrestamoRepositorio _prestamoRepositorio;
        private readonly ILibroRepositorio _libroRepositorio;
        private readonly ISocioRepositorio _socioRepositorio;

        public PrestamoNegocio(IPrestamoRepositorio prestamoRepositorio, ILibroRepositorio libroRepositorio, ISocioRepositorio socioRepositorio)
        {
            _prestamoRepositorio = prestamoRepositorio;
            _libroRepositorio = libroRepositorio;
            _socioRepositorio = socioRepositorio;
        }

        public async Task<bool> RegistrarPrestamoAsync(Prestamo prestamo, List<int> librosIds)
        {
            // 1. Validar que el socio existe y está activo
            var socio = await _socioRepositorio.ObtenerPorIdAsync(prestamo.SocioId);
            if (socio == null || !socio.Activo)
                throw new ReglaNegocioException("El socio no existe o está inactivo.");

            // 2. Validar que no exceda el límite de 3 libros pendientes
            int pendientes = await _prestamoRepositorio.ObtenerLibrosPendientesPorSocioAsync(prestamo.SocioId);
            if (pendientes + librosIds.Count > 3)
                throw new ReglaNegocioException($"El socio ya tiene {pendientes} libros pendientes. El límite es 3, por lo que no puede llevar {librosIds.Count} más.");

            // 3. Validar estado y ejemplares de los libros a prestar
            foreach (var id in librosIds)
            {
                var libro = await _libroRepositorio.ObtenerPorIdAsync(id);
                if (libro == null || !libro.Activo)
                    throw new ReglaNegocioException($"El libro con ID {id} no existe o está inactivo.");
                if (libro.Ejemplares <= 0)
                    throw new ReglaNegocioException($"No hay ejemplares disponibles para el libro: {libro.Titulo}.");
            }

            // 4. Si todo es correcto, grabar (transacción manejada en capa de datos)
            return await _prestamoRepositorio.RegistrarPrestamoAsync(prestamo, librosIds);
        }

        public async Task<decimal> RegistrarDevolucionAsync(int prestamoId, int libroId, DateTime fechaLimite)
        {
            DateTime fechaDevolucion = DateTime.Now;
            decimal multa = 0;

            // Calcular Multa (S/ 1.50 por día de retraso)
            if (fechaDevolucion.Date > fechaLimite.Date)
            {
                var diasRetraso = (fechaDevolucion.Date - fechaLimite.Date).Days;
                multa = diasRetraso * 1.50m;
            }

            await _prestamoRepositorio.RegistrarDevolucionAsync(prestamoId, libroId, fechaDevolucion);
            return multa;
        }
        public async Task<List<ReportePrestamoDto>> ObtenerReportePrestamosAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _prestamoRepositorio.ObtenerReportePrestamosAsync(fechaInicio, fechaFin);
        }
    }
}
