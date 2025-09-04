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
            // Cargar variables del archivo .env
            Env.Load();

            string password = Env.GetString("PG_PASSWORD");
            // valores según base en PostgreSQL
            conexion = new NpgsqlConnection("Host=localhost; Port=5432; Database=motoState; Username=postgres; Password={password}");
            comando = new NpgsqlCommand();
        }

        public void SetearConsulta(string consulta)
        {
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
            catch (Exception ex)
            {
                throw ex;
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetearParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
        }

        public void CerrarConexion()
        {
            if (lector != null && !lector.IsClosed)
                lector.Close();
            conexion.Close();
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