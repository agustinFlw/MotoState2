using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
// Driver PostgreSQL
using Dominio;
using System.Data;
using DotNetEnv;
namespace Negocio
{
    public class AccesoDatos
    {
        private NpgsqlConnection conexion;
        private NpgsqlCommand comando;
        private NpgsqlDataReader lector;

        public NpgsqlDataReader Lector
        {
            get { return lector; }
        }

        public AccesoDatos()
        {
            // Cargar variables del archivo .env (si no existen, ponemos defaults)
            Env.Load();

            string host = Env.GetString("PG_HOST") ?? "localhost";
            string port = Env.GetString("PG_PORT") ?? "5432";
            string db = Env.GetString("PG_DATABASE") ?? "motoState";
            string user = Env.GetString("PG_USER") ?? "postgres";
            string password = Env.GetString("PG_PASSWORD") ?? "";

            // IMPORTANTE: string interpolado con $ para usar variables
            conexion = new NpgsqlConnection(
                $"Host={host}; Port={port}; Database={db}; Username={user}; Password={password}"
            );

            comando = new NpgsqlCommand();
        }

        public void SetearConsulta(string consulta)
        {
            // Limpio parámetros de ejecuciones anteriores
            comando.Parameters.Clear();
            comando.CommandType = CommandType.Text;
            comando.CommandText = consulta;
        }

        public void EjecutarLectura()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                lector = comando.ExecuteReader();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void EjecutarAccion()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                comando.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //cuando necesites un único valor (ej. RETURNING id)
        public object EjecutarEscalar()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                return comando.ExecuteScalar();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetearParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
        }

        public void CerrarConexion()
        {
            try
            {
                if (lector != null && !lector.IsClosed)
                    lector.Close();
                if (conexion.State != ConnectionState.Closed)
                    conexion.Close();
            }
            catch { /* noop */ }
        }

        // Ejemplo: obtener id de un usuario por email
        public int ObtenerIdUsuario(string email)
        {
            int id = 0;
            try
            {
                SetearConsulta("SELECT id_usuario FROM Usuario WHERE email = @Email");
                SetearParametro("@Email", email);
                EjecutarLectura();

                if (Lector.Read())
                {
                    id = (int)Lector["id_usuario"];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CerrarConexion();
            }
            return id;
        }
    }
}