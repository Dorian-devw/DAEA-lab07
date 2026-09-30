using Biblioteca.Entidades;
using Biblioteca.Negocio.Excepciones;
using Biblioteca.Negocio.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biblioteca.Negocio
{
    public class SocioNegocio
    {
        private readonly ISocioRepositorio _socioRepositorio;
        private readonly IPrestamoRepositorio _prestamoRepositorio;

        public SocioNegocio(ISocioRepositorio socioRepositorio, IPrestamoRepositorio prestamoRepositorio)
        {
            _socioRepositorio = socioRepositorio;
            _prestamoRepositorio = prestamoRepositorio;
        }

        public async Task<List<Socio>> ObtenerTodosAsync()
        {
            return await _socioRepositorio.ObtenerTodosAsync();
        }

        public async Task<int> InsertarAsync(Socio socio)
        {
            if (string.IsNullOrWhiteSpace(socio.Nombre) || string.IsNullOrWhiteSpace(socio.DNI))
                throw new ReglaNegocioException("El nombre y DNI son obligatorios.");

            var existente = await _socioRepositorio.ObtenerPorDniAsync(socio.DNI);
            if (existente != null)
                throw new ReglaNegocioException("Ya existe un socio con este DNI.");

            return await _socioRepositorio.InsertarAsync(socio);
        }

        public async Task<bool> ActualizarAsync(Socio socio)
        {
            if (string.IsNullOrWhiteSpace(socio.Nombre) || string.IsNullOrWhiteSpace(socio.DNI))
                throw new ReglaNegocioException("El nombre y DNI son obligatorios.");

            var existente = await _socioRepositorio.ObtenerPorDniAsync(socio.DNI);
            if (existente != null && existente.SocioId != socio.SocioId)
                throw new ReglaNegocioException("Ya existe otro socio con este DNI.");

            return await _socioRepositorio.ActualizarAsync(socio);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            // Validar que no tenga prestamos pendientes
            if (await _prestamoRepositorio.ExistePrestamoPendienteParaSocioAsync(id))
            {
                throw new ReglaNegocioException("No se puede eliminar el socio porque tiene préstamos pendientes.");
            }
            
            return await _socioRepositorio.EliminarLogicoAsync(id);
        }
    }
}
