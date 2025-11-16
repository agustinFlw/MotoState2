using System;
using System.Collections.Generic;
using Dominio;
using Microsoft.Data.SqlClient; // Necesario para capturar SqlException
using System.Data; // Necesario para CommandType
using System.Linq; // Necesario para Listar (si lo usas)

namespace Negocio
{
    public class MotoNegocio
    {
        // =========================================================================
        // MÉTODO DE INSERCIÓN TRANSACCIONAL (Llamada al Stored Procedure)
        // =========================================================================
        public int RegistrarOrdenInicial(
            string patente,
            string dniMecanicoPrincipal,
            string descripcionOrden,
            string servicioDesc,
            decimal costoServicio,
            string dniMecanicoDetalle
        )
        {
            var datos = new AccesoDatos();
            try
            {
                // Configuración para llamar al Stored Procedure
                datos.SetearTipoComando(CommandType.StoredProcedure);
                datos.SetearConsulta("SP_RegistrarOrdenCompleta");

                // 3. Setear los 6 parámetros del SP
                datos.SetearParametro("@Patente", patente);
                datos.SetearParametro("@MecanicoPrincipalDni", dniMecanicoPrincipal);
                datos.SetearParametro("@DescripcionOrden", (object)descripcionOrden ?? DBNull.Value);
                datos.SetearParametro("@ServicioDesc", servicioDesc);
                datos.SetearParametro("@CostoServicio", costoServicio);
                datos.SetearParametro("@MecanicoDetalleDni", dniMecanicoDetalle);

                // El SP se ejecuta. Si el commit tiene éxito, la acción se completa.
                datos.EjecutarAccion();

                // Nota: Retornamos 1 si la ejecución fue sin error, o puedes 
                // modificar el SP y esta parte para retornar el ID generado.
                return 1;
            }
            catch (SqlException ex)
            {
                // Captura el error del Trigger de Regla de Negocio
                if (ex.Message.Contains("El mecánico asignado ya se encuentra ocupado"))
                {
                    throw new InvalidOperationException($"ERROR DE NEGOCIO: {ex.Message}", ex);
                }
                throw;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        // =========================================================================
        // MÉTODOS CRUD ORIGINALES (Mantenidos para el resto de la app)
        // =========================================================================

        // Alta: inserta y devuelve el ID generado (NOTA: ASUMO QUE ESTO ES UN CÓDIGO VIEJO 
        // Y SOLO DEBERÍA USARSE PARA INSERTAR UNA MOTO SI NO HAY REPARACIÓN ASOCIADA, PERO 
        // LO MANTENEMOS POR INTEGRIDAD DE LA CLASE).
        public int Agregar(Moto moto)
        {
            var datos = new AccesoDatos();
            try
            {
                // ATENCIÓN: Si usas SQL Server, 'RETURNING' no funciona, 
                // deberías usar SCOPE_IDENTITY() dentro de un SELECT. 
                // Asumimos que esta query ha sido adaptada o usa un SP.
                datos.SetearConsulta(@"
                    INSERT INTO Moto (marca, modelo, patente, fecha_ingreso, foto_url, foto_subida, id_usuario)
                    VALUES (@Marca, @Modelo, @Patente, @FechaIngreso, @FotoUrl, @FotoSubida, @IdUsuario)
                    SELECT SCOPE_IDENTITY(); -- Usamos SCOPE_IDENTITY() para SQL Server
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
                        // Asegúrate de que los índices coincidan con la consulta SELECT
                        IdMoto = datos.Lector.GetInt32(0),
                        Marca = datos.Lector.GetString(1),
                        Modelo = datos.Lector.GetString(2),
                        Patente = datos.Lector.IsDBNull(3) ? null : datos.Lector.GetString(3),
                        FechaIngreso = datos.Lector.GetDateTime(4),
                        FotoUrl = datos.Lector.IsDBNull(5) ? null : datos.Lector.GetString(5),
                        // Asumo que FotoSubida es SMALLINT (Int16)
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

        // Obtener una moto por ID
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