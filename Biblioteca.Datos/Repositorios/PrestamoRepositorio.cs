using Biblioteca.Entidades;
using Biblioteca.Negocio.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biblioteca.Datos.Repositorios
{
    public class PrestamoRepositorio : IPrestamoRepositorio
    {
        public async Task<bool> RegistrarPrestamoAsync(Prestamo prestamo, List<int> librosIds)
        {
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                // REGLA: La cabecera, los detalles y el descuento de ejemplares se guardan en una sola transacción
                using (var tr = cn.BeginTransaction())
                {
                    try
                    {
                        var cmdCabecera = new SqlCommand(
                            "INSERT INTO Prestamos (SocioId, FechaPrestamo, FechaLimite, Estado) OUTPUT INSERTED.PrestamoId VALUES (@socioId, @fechaPrestamo, @fechaLimite, @estado)", cn, tr);
                        cmdCabecera.Parameters.AddWithValue("@socioId", prestamo.SocioId);
                        cmdCabecera.Parameters.AddWithValue("@fechaPrestamo", prestamo.FechaPrestamo);
                        cmdCabecera.Parameters.AddWithValue("@fechaLimite", prestamo.FechaLimite);
                        cmdCabecera.Parameters.AddWithValue("@estado", prestamo.Estado);
                        
                        int prestamoId = (int)await cmdCabecera.ExecuteScalarAsync();

                        foreach (var libroId in librosIds)
                        {
                            var cmdDetalle = new SqlCommand(
                                "INSERT INTO DetallePrestamo (PrestamoId, LibroId) VALUES (@prestamoId, @libroId)", cn, tr);
                            cmdDetalle.Parameters.AddWithValue("@prestamoId", prestamoId);
                            cmdDetalle.Parameters.AddWithValue("@libroId", libroId);
                            await cmdDetalle.ExecuteNonQueryAsync();

                            var cmdDescuento = new SqlCommand(
                                "UPDATE Libros SET Ejemplares = Ejemplares - 1 WHERE LibroId = @libroId", cn, tr);
                            cmdDescuento.Parameters.AddWithValue("@libroId", libroId);
                            await cmdDescuento.ExecuteNonQueryAsync();
                        }

                        await tr.CommitAsync();
                        return true;
                    }
                    catch
                    {
                        await tr.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<bool> RegistrarDevolucionAsync(int prestamoId, int libroId, DateTime fechaDevolucion)
        {
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                using (var tr = cn.BeginTransaction())
                {
                    try
                    {
                        var cmdDetalle = new SqlCommand(
                            "UPDATE DetallePrestamo SET FechaDevolucion = @fecha WHERE PrestamoId = @pId AND LibroId = @lId", cn, tr);
                        cmdDetalle.Parameters.AddWithValue("@fecha", fechaDevolucion);
                        cmdDetalle.Parameters.AddWithValue("@pId", prestamoId);
                        cmdDetalle.Parameters.AddWithValue("@lId", libroId);
                        await cmdDetalle.ExecuteNonQueryAsync();

                        var cmdSuma = new SqlCommand(
                            "UPDATE Libros SET Ejemplares = Ejemplares + 1 WHERE LibroId = @lId", cn, tr);
                        cmdSuma.Parameters.AddWithValue("@lId", libroId);
                        await cmdSuma.ExecuteNonQueryAsync();

                        // Verificar si quedan pendientes en este préstamo
                        var cmdVerificar = new SqlCommand(
                            "SELECT COUNT(*) FROM DetallePrestamo WHERE PrestamoId = @pId AND FechaDevolucion IS NULL", cn, tr);
                        cmdVerificar.Parameters.AddWithValue("@pId", prestamoId);
                        int pendientes = (int)await cmdVerificar.ExecuteScalarAsync();

                        if (pendientes == 0)
                        {
                            var cmdEstado = new SqlCommand(
                                "UPDATE Prestamos SET Estado = 'Devuelto' WHERE PrestamoId = @pId", cn, tr);
                            cmdEstado.Parameters.AddWithValue("@pId", prestamoId);
                            await cmdEstado.ExecuteNonQueryAsync();
                        }

                        await tr.CommitAsync();
                        return true;
                    }
                    catch
                    {
                        await tr.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<int> ObtenerLibrosPendientesPorSocioAsync(int socioId)
        {
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var cmd = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM Prestamos p
                    INNER JOIN DetallePrestamo dp ON p.PrestamoId = dp.PrestamoId
                    WHERE p.SocioId = @socioId AND dp.FechaDevolucion IS NULL", cn);
                cmd.Parameters.AddWithValue("@socioId", socioId);
                return (int)await cmd.ExecuteScalarAsync();
            }
        }

        public async Task<bool> ExistePrestamoPendienteParaLibroAsync(int libroId)
        {
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var cmd = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM DetallePrestamo dp
                    INNER JOIN Prestamos p ON dp.PrestamoId = p.PrestamoId
                    WHERE dp.LibroId = @libroId AND dp.FechaDevolucion IS NULL", cn);
                cmd.Parameters.AddWithValue("@libroId", libroId);
                return (int)await cmd.ExecuteScalarAsync() > 0;
            }
        }

        public async Task<bool> ExistePrestamoPendienteParaSocioAsync(int socioId)
        {
            return await ObtenerLibrosPendientesPorSocioAsync(socioId) > 0;
        }

        public async Task<List<Prestamo>> ObtenerPrestamosConDetalleAsync(int socioId)
        {
            // Retorna prestamos pendientes de un socio con sus detalles
            var prestamos = new List<Prestamo>();
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var query = @"
                    SELECT p.PrestamoId, p.SocioId, p.FechaPrestamo, p.FechaLimite, p.Estado,
                           dp.LibroId, l.Titulo, dp.FechaDevolucion
                    FROM Prestamos p
                    INNER JOIN DetallePrestamo dp ON p.PrestamoId = dp.PrestamoId
                    INNER JOIN Libros l ON dp.LibroId = l.LibroId
                    WHERE p.SocioId = @socioId AND p.Estado = 'Pendiente' AND dp.FechaDevolucion IS NULL";

                var cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@socioId", socioId);
                
                Prestamo prestamoActual = null;
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        int pId = dr.GetInt32(0);
                        if (prestamoActual == null || prestamoActual.PrestamoId != pId)
                        {
                            prestamoActual = new Prestamo
                            {
                                PrestamoId = pId,
                                SocioId = dr.GetInt32(1),
                                FechaPrestamo = dr.GetDateTime(2),
                                FechaLimite = dr.GetDateTime(3),
                                Estado = dr.GetString(4)
                            };
                            prestamos.Add(prestamoActual);
                        }

                        prestamoActual.Detalles.Add(new DetallePrestamo
                        {
                            PrestamoId = pId,
                            LibroId = dr.GetInt32(5),
                            FechaDevolucion = dr.IsDBNull(7) ? (DateTime?)null : dr.GetDateTime(7),
                            Libro = new Libro { Titulo = dr.GetString(6) }
                        });
                    }
                }
            }
            return prestamos;
        }

        public async Task<List<ReportePrestamoDto>> ObtenerReportePrestamosAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var resultado = new List<ReportePrestamoDto>();
            using (var cn = DbConnectionFactory.CreateConnection())
            {
                await cn.OpenAsync();
                var query = @"
                    SELECT s.Nombre AS Socio, l.Titulo AS Libro, p.FechaPrestamo, p.FechaLimite, dp.FechaDevolucion, p.Estado
                    FROM Prestamos p
                    INNER JOIN DetallePrestamo dp ON p.PrestamoId = dp.PrestamoId
                    INNER JOIN Libros l ON dp.LibroId = l.LibroId
                    INNER JOIN Socios s ON p.SocioId = s.SocioId
                    WHERE p.FechaPrestamo >= @inicio AND p.FechaPrestamo <= @fin
                    ORDER BY p.FechaPrestamo DESC";
                var cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@inicio", fechaInicio);
                cmd.Parameters.AddWithValue("@fin", fechaFin.AddDays(1).AddSeconds(-1)); // Hasta el final del día
                
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        resultado.Add(new ReportePrestamoDto
                        {
                            Socio = dr.GetString(0),
                            TituloLibro = dr.GetString(1),
                            FechaPrestamo = dr.GetDateTime(2),
                            FechaLimite = dr.GetDateTime(3),
                            FechaDevolucion = dr.IsDBNull(4) ? (DateTime?)null : dr.GetDateTime(4),
                            Estado = dr.GetString(5)
                        });
                    }
                }
            }
            return resultado;
        }
    }
}
