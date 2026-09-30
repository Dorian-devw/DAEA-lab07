using Biblioteca.Entidades;
using Biblioteca.Negocio.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biblioteca.Datos.Repositorios
{
    public class LibroRepositorio : ILibroRepositorio
    {
        public async Task<List<Libro>> ObtenerTodosAsync()
        {
            var lista = new List<Libro>();
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var query = @"SELECT l.LibroId, l.Titulo, l.ISBN, l.AutorId, l.Ejemplares, l.Activo,
                                     a.Nombre as AutorNombre 
                              FROM Libros l 
                              INNER JOIN Autores a ON l.AutorId = a.AutorId 
                              WHERE l.Activo = 1";
                var cmd = new SqlCommand(query, cn);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        var libro = new Libro
                        {
                            LibroId = dr.GetInt32(0),
                            Titulo = dr.GetString(1),
                            ISBN = dr.GetString(2),
                            AutorId = dr.GetInt32(3),
                            Ejemplares = dr.GetInt32(4),
                            Activo = dr.GetBoolean(5),
                            Autor = new Autor { AutorId = dr.GetInt32(3), Nombre = dr.GetString(6) }
                        };
                        lista.Add(libro);
                    }
                }
            }
            return lista;
        }

        public async Task<Libro> ObtenerPorIdAsync(int id)
        {
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var cmd = new SqlCommand("SELECT LibroId, Titulo, ISBN, AutorId, Ejemplares, Activo FROM Libros WHERE LibroId = @id", cn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        return new Libro
                        {
                            LibroId = dr.GetInt32(0),
                            Titulo = dr.GetString(1),
                            ISBN = dr.GetString(2),
                            AutorId = dr.GetInt32(3),
                            Ejemplares = dr.GetInt32(4),
                            Activo = dr.GetBoolean(5)
                        };
                    }
                }
            }
            return null;
        }

        public async Task<Libro> ObtenerPorISBNAsync(string isbn)
        {
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var cmd = new SqlCommand("SELECT LibroId, Titulo, ISBN, AutorId, Ejemplares, Activo FROM Libros WHERE ISBN = @isbn", cn);
                cmd.Parameters.AddWithValue("@isbn", isbn);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        return new Libro
                        {
                            LibroId = dr.GetInt32(0),
                            Titulo = dr.GetString(1),
                            ISBN = dr.GetString(2),
                            AutorId = dr.GetInt32(3),
                            Ejemplares = dr.GetInt32(4),
                            Activo = dr.GetBoolean(5)
                        };
                    }
                }
            }
            return null;
        }

        public async Task<int> InsertarAsync(Libro libro)
        {
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var cmd = new SqlCommand(
                    "INSERT INTO Libros (Titulo, ISBN, AutorId, Ejemplares, Activo) OUTPUT INSERTED.LibroId VALUES (@titulo, @isbn, @autorId, @ejemplares, @activo)", cn);
                cmd.Parameters.AddWithValue("@titulo", libro.Titulo);
                cmd.Parameters.AddWithValue("@isbn", libro.ISBN);
                cmd.Parameters.AddWithValue("@autorId", libro.AutorId);
                cmd.Parameters.AddWithValue("@ejemplares", libro.Ejemplares);
                cmd.Parameters.AddWithValue("@activo", libro.Activo);
                
                return (int)await cmd.ExecuteScalarAsync();
            }
        }

        public async Task<bool> ActualizarAsync(Libro libro)
        {
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var cmd = new SqlCommand(
                    "UPDATE Libros SET Titulo = @titulo, ISBN = @isbn, AutorId = @autorId, Ejemplares = @ejemplares WHERE LibroId = @id", cn);
                cmd.Parameters.AddWithValue("@titulo", libro.Titulo);
                cmd.Parameters.AddWithValue("@isbn", libro.ISBN);
                cmd.Parameters.AddWithValue("@autorId", libro.AutorId);
                cmd.Parameters.AddWithValue("@ejemplares", libro.Ejemplares);
                cmd.Parameters.AddWithValue("@id", libro.LibroId);
                
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> EliminarLogicoAsync(int id)
        {
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var cmd = new SqlCommand("UPDATE Libros SET Activo = 0 WHERE LibroId = @id", cn);
                cmd.Parameters.AddWithValue("@id", id);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }
    }
}
