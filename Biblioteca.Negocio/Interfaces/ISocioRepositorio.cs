using Biblioteca.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biblioteca.Negocio.Interfaces
{
    public interface ISocioRepositorio
    {
        Task<List<Socio>> ObtenerTodosAsync();
        Task<Socio> ObtenerPorIdAsync(int id);
        Task<Socio> ObtenerPorDniAsync(string dni);
        Task<int> InsertarAsync(Socio socio);
        Task<bool> ActualizarAsync(Socio socio);
        Task<bool> EliminarLogicoAsync(int id);
    }
}
