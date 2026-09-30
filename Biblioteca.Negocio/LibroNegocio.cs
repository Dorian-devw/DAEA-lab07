using Biblioteca.Entidades;
using Biblioteca.Negocio.Excepciones;
using Biblioteca.Negocio.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biblioteca.Negocio
{
    public class LibroNegocio
    {
        private readonly ILibroRepositorio _libroRepositorio;

        // Inyección de dependencias como solicitado en el punto 17
        public LibroNegocio(ILibroRepositorio libroRepositorio)
        {
            _libroRepositorio = libroRepositorio;
        }

        public async Task<List<Libro>> ObtenerTodosAsync()
        {
            return await _libroRepositorio.ObtenerTodosAsync();
        }

        public async Task<Libro> ObtenerPorIdAsync(int id)
        {
            return await _libroRepositorio.ObtenerPorIdAsync(id);
        }

        public async Task<int> InsertarAsync(Libro libro)
        {
            if (string.IsNullOrWhiteSpace(libro.Titulo) || string.IsNullOrWhiteSpace(libro.ISBN))
                throw new ReglaNegocioException("El título y el ISBN son obligatorios.");

            var libroExistente = await _libroRepositorio.ObtenerPorISBNAsync(libro.ISBN);
            if (libroExistente != null)
                throw new ReglaNegocioException("Ya existe un libro con este ISBN.");

            if (libro.Ejemplares < 0)
                throw new ReglaNegocioException("La cantidad de ejemplares no puede ser negativa.");

            return await _libroRepositorio.InsertarAsync(libro);
        }

        public async Task<bool> ActualizarAsync(Libro libro)
        {
            if (string.IsNullOrWhiteSpace(libro.Titulo) || string.IsNullOrWhiteSpace(libro.ISBN))
                throw new ReglaNegocioException("El título y el ISBN son obligatorios.");

            var libroExistente = await _libroRepositorio.ObtenerPorISBNAsync(libro.ISBN);
            if (libroExistente != null && libroExistente.LibroId != libro.LibroId)
                throw new ReglaNegocioException("Ya existe otro libro con este ISBN.");

            if (libro.Ejemplares < 0)
                throw new ReglaNegocioException("La cantidad de ejemplares no puede ser negativa.");

            return await _libroRepositorio.ActualizarAsync(libro);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            // TODO: Falta validar que el libro no tenga préstamos pendientes (se necesita consulta de préstamos cruzada)
            // Esto se resolvería inyectando IPrestamoRepositorio o haciendo una consulta específica.
            return await _libroRepositorio.EliminarLogicoAsync(id);
        }
    }
}
