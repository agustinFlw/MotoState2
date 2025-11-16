using System;
using System.Collections.Generic;
using Dominio;
using Microsoft.Data.SqlClient; // Necesario para capturar SqlException
using System.Data; // Necesario para CommandType

namespace Negocio
{
    public class MotoNegocio
    {
        // -------------------------------------------------------------------
        // REEMPLAZO DEL MÉTODO AGREGAR
        // Este nuevo método llama al SP transaccional que creamos
        // -------------------------------------------------------------------
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
                // 1. Indicar que vamos a ejecutar un Stored Procedure (¡CORRECCIÓN AQUÍ!)
                datos.SetearTipoComando(CommandType.StoredProcedure); // <-- USAMOS EL NUEVO MÉTODO

                // 2. Setear el nombre del SP (El método SetearConsulta necesita una pequeña modificación para SPs)
                datos.SetearConsulta("SP_RegistrarOrdenCompleta"); // <-- Aquí pasamos el nombre del SP

                // El resto de tu código para SetearParámetros sigue igual:
                // 3. Setear los 6 parámetros del SP (NOMBRES EXACTOS)
                datos.SetearParametro("@Patente", patente);
                // ...

                datos.EjecutarAccion();

                return 1; // Retornamos 1 si el SP se ejecutó sin errores
            }
            catch (SqlException ex)
            {
                // ¡CLAVE! Capturar el error del Trigger (Unidad 5: Manejo de Errores)
                if (ex.Message.Contains("El mecánico asignado ya se encuentra ocupado"))
                {
                    // Relanzamos el error para que la interfaz sepa que falló por la regla de negocio.
                    throw new InvalidOperationException($"ERROR DE NEGOCIO: {ex.Message}", ex);
                }
                // Si es cualquier otro error de BD, lo relanzamos.
                throw;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        // ... El resto de tus métodos (Eliminar, Modificar, Listar) siguen igual ...
    }
}