using Biblioteca.Entidades;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biblioteca.Datos.Repositorios
{
    public class AutorRepositorio
    {
        public async Task<List<Autor>> ObtenerTodosAsync()
        {
            var lista = new List<Autor>();
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var cmd = new SqlCommand("SELECT AutorId, Nombre, Nacionalidad, Activo FROM Autores WHERE Activo = 1", cn);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Autor
                        {
                            AutorId = dr.GetInt32(0),
                            Nombre = dr.GetString(1),
                            Nacionalidad = dr.IsDBNull(2) ? null : dr.GetString(2),
                            Activo = dr.GetBoolean(3)
                        });
                    }
                }
            }
            return lista;
        }
    }
}
