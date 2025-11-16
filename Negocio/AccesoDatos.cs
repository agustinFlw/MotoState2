using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient; // Driver de SQL Server
using Dominio;
using System.Data;
using DotNetEnv;

namespace Negocio
{
    public class AccesoDatos
    {
        
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;

        public SqlDataReader Lector
        {
            get { return lector; }
        }

        public AccesoDatos()
        {
            // Cargar variables actualizadas del archivo .env
            Env.Load();

            string host = Env.GetString("HOST");
            string db = Env.GetString("DATABASE"); // Ahora debe leer TallerMecanicoMotos


            // Cadena de conexión usando el formato estándar de SQL Server (SqlClient)
            conexion = new SqlConnection(
            $"Server={host}; Database={db}; Trusted_Connection=True; TrustServerCertificate=True"
            );

            comando = new SqlCommand();
        }

        public void SetearConsulta(string consulta)
        {
            // Limpio parámetros de ejecuciones anteriores
            comando.Parameters.Clear();
            comando.CommandText = consulta;
        }

        public void SetearTipoComando(CommandType tipo)
        {
            comando.CommandType = tipo;
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
            // AddWithValue funciona igual en SqlClient
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

        public int ObtenerIdUsuario(string email)
        {
            int id = 0;
            try
            {
                SetearConsulta("SELECT id_usuario FROM Usuario WHERE email = @Email");
                SetearParametro("@Email", email);
                EjecutarLectura();

                // Lector funciona igual
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