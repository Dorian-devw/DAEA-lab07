using System;

namespace Biblioteca.Negocio.Excepciones
{
    public class ReglaNegocioException : Exception
    {
        public ReglaNegocioException(string mensaje) : base(mensaje)
        {
        }
    }
}
