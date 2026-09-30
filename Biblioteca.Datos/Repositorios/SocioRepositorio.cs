using Biblioteca.Entidades;
using Biblioteca.Negocio.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biblioteca.Datos.Repositorios
{
    public class SocioRepositorio : ISocioRepositorio
    {
        public async Task<List<Socio>> ObtenerTodosAsync()
        {
            var lista = new List<Socio>();
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var cmd = new SqlCommand("SELECT SocioId, DNI, Nombre, Email, Activo FROM Socios WHERE Activo = 1", cn);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Socio
                        {
                            SocioId = dr.GetInt32(0),
                            DNI = dr.GetString(1),
                            Nombre = dr.GetString(2),
                            Email = dr.IsDBNull(3) ? null : dr.GetString(3),
                            Activo = dr.GetBoolean(4)
                        });
                    }
                }
            }
            return lista;
        }

        public async Task<Socio> ObtenerPorIdAsync(int id)
        {
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var cmd = new SqlCommand("SELECT SocioId, DNI, Nombre, Email, Activo FROM Socios WHERE SocioId = @id", cn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        return new Socio
                        {
                            SocioId = dr.GetInt32(0),
                            DNI = dr.GetString(1),
                            Nombre = dr.GetString(2),
                            Email = dr.IsDBNull(3) ? null : dr.GetString(3),
                            Activo = dr.GetBoolean(4)
                        };
                    }
                }
            }
            return null;
        }

        public async Task<Socio> ObtenerPorDniAsync(string dni)
        {
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var cmd = new SqlCommand("SELECT SocioId, DNI, Nombre, Email, Activo FROM Socios WHERE DNI = @dni", cn);
                cmd.Parameters.AddWithValue("@dni", dni);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        return new Socio
                        {
                            SocioId = dr.GetInt32(0),
                            DNI = dr.GetString(1),
                            Nombre = dr.GetString(2),
                            Email = dr.IsDBNull(3) ? null : dr.GetString(3),
                            Activo = dr.GetBoolean(4)
                        };
                    }
                }
            }
            return null;
        }

        public async Task<int> InsertarAsync(Socio socio)
        {
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var cmd = new SqlCommand(
                    "INSERT INTO Socios (DNI, Nombre, Email, Activo) OUTPUT INSERTED.SocioId VALUES (@dni, @nombre, @email, @activo)", cn);
                cmd.Parameters.AddWithValue("@dni", socio.DNI);
                cmd.Parameters.AddWithValue("@nombre", socio.Nombre);
                cmd.Parameters.AddWithValue("@email", (object)socio.Email ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@activo", socio.Activo);
                
                return (int)await cmd.ExecuteScalarAsync();
            }
        }

        public async Task<bool> ActualizarAsync(Socio socio)
        {
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var cmd = new SqlCommand(
                    "UPDATE Socios SET DNI = @dni, Nombre = @nombre, Email = @email WHERE SocioId = @id", cn);
                cmd.Parameters.AddWithValue("@dni", socio.DNI);
                cmd.Parameters.AddWithValue("@nombre", socio.Nombre);
                cmd.Parameters.AddWithValue("@email", (object)socio.Email ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@id", socio.SocioId);
                
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> EliminarLogicoAsync(int id)
        {
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var cmd = new SqlCommand("UPDATE Socios SET Activo = 0 WHERE SocioId = @id", cn);
                cmd.Parameters.AddWithValue("@id", id);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }
    }
}
