using Biblioteca.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biblioteca.Negocio.Interfaces
{
    public interface IAutorRepositorio
    {
        Task<List<Autor>> ObtenerTodosAsync();
    }
}
