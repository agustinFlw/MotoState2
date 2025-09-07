using System;
using System.Collections.Generic;
using Dominio;
using Npgsql;

namespace Negocio
{
    public class MotoNegocio
    {
        // Alta: inserta y devuelve el ID generado
        public int Agregar(Moto moto)
        {
            var datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(@"
                    INSERT INTO Moto (marca, modelo, patente, fecha_ingreso, foto_url, foto_subida, id_usuario)
                    VALUES (@Marca, @Modelo, @Patente, @FechaIngreso, @FotoUrl, @FotoSubida, @IdUsuario)
                    RETURNING id_moto;
                ");

                datos.SetearParametro("@Marca", moto.Marca);
                datos.SetearParametro("@Modelo", moto.Modelo);
                datos.SetearParametro("@Patente", (object?)moto.Patente ?? DBNull.Value);
                datos.SetearParametro("@FechaIngreso", moto.FechaIngreso.Date);
                datos.SetearParametro("@FotoUrl", (object?)moto.FotoUrl ?? DBNull.Value);
                datos.SetearParametro("@FotoSubida", moto.FotoSubida);
                datos.SetearParametro("@IdUsuario", moto.IdUsuario);

                var result = datos.EjecutarEscalar();
                return Convert.ToInt32(result);
            }
            catch
            {
                throw;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        // Baja: elimina por ID (devuelve true si no explota)
        public bool Eliminar(int idMoto)
        {
            var datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta("DELETE FROM Moto WHERE id_moto = @IdMoto;");
                datos.SetearParametro("@IdMoto", idMoto);
                datos.EjecutarAccion();
                return true;
            }
            catch
            {
                throw;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        // Modificar: actualiza todos los campos por ID
        public bool Modificar(Moto moto)
        {
            var datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(@"
                    UPDATE Moto
                    SET  marca = @Marca,
                         modelo = @Modelo,
                         patente = @Patente,
                         fecha_ingreso = @FechaIngreso,
                         foto_url = @FotoUrl,
                         foto_subida = @FotoSubida,
                         id_usuario = @IdUsuario
                    WHERE id_moto = @IdMoto;
                ");

                datos.SetearParametro("@Marca", moto.Marca);
                datos.SetearParametro("@Modelo", moto.Modelo);
                datos.SetearParametro("@Patente", (object?)moto.Patente ?? DBNull.Value);
                datos.SetearParametro("@FechaIngreso", moto.FechaIngreso.Date);
                datos.SetearParametro("@FotoUrl", (object?)moto.FotoUrl ?? DBNull.Value);
                datos.SetearParametro("@FotoSubida", moto.FotoSubida);
                datos.SetearParametro("@IdUsuario", moto.IdUsuario);
                datos.SetearParametro("@IdMoto", moto.IdMoto);

                datos.EjecutarAccion();
                return true;
            }
            catch
            {
                throw;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        // Listar: devuelve todas las motos
        public List<Moto> Listar()
        {
            var lista = new List<Moto>();
            var datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(@"
                    SELECT id_moto, marca, modelo, patente, fecha_ingreso, foto_url, foto_subida, id_usuario
                    FROM Moto
                    ORDER BY id_moto DESC;
                ");
                datos.EjecutarLectura();

                while (datos.Lector.Read())
                {
                    // OJO con nulls en DB → IsDBNull
                    var moto = new Moto
                    {
                        IdMoto = datos.Lector.GetInt32(0),
                        Marca = datos.Lector.GetString(1),
                        Modelo = datos.Lector.GetString(2),
                        Patente = datos.Lector.IsDBNull(3) ? null : datos.Lector.GetString(3),
                        FechaIngreso = datos.Lector.GetDateTime(4),
                        FotoUrl = datos.Lector.IsDBNull(5) ? null : datos.Lector.GetString(5),
                        FotoSubida = datos.Lector.GetInt16(6),
                        IdUsuario = datos.Lector.GetInt32(7)
                    };
                    lista.Add(moto);
                }
                return lista;
            }
            catch
            {
                throw;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        // (Opcional pero útil) Obtener una moto por ID
        public Moto ObtenerPorId(int idMoto)
        {
            var datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(@"
                    SELECT id_moto, marca, modelo, patente, fecha_ingreso, foto_url, foto_subida, id_usuario
                    FROM Moto
                    WHERE id_moto = @IdMoto;
                ");
                datos.SetearParametro("@IdMoto", idMoto);
                datos.EjecutarLectura();

                if (datos.Lector.Read())
                {
                    return new Moto
                    {
                        IdMoto = datos.Lector.GetInt32(0),
                        Marca = datos.Lector.GetString(1),
                        Modelo = datos.Lector.GetString(2),
                        Patente = datos.Lector.IsDBNull(3) ? null : datos.Lector.GetString(3),
                        FechaIngreso = datos.Lector.GetDateTime(4),
                        FotoUrl = datos.Lector.IsDBNull(5) ? null : datos.Lector.GetString(5),
                        FotoSubida = datos.Lector.GetInt16(6),
                        IdUsuario = datos.Lector.GetInt32(7)
                    };
                }
                return null;
            }
            catch
            {
                throw;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }
    }
}
