using Biblioteca.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biblioteca.Negocio.Interfaces
{
    public interface ILibroRepositorio
    {
        Task<List<Libro>> ObtenerTodosAsync();
        Task<Libro> ObtenerPorIdAsync(int id);
        Task<Libro> ObtenerPorISBNAsync(string isbn);
        Task<int> InsertarAsync(Libro libro);
        Task<bool> ActualizarAsync(Libro libro);
        Task<bool> EliminarLogicoAsync(int id);
    }
}
