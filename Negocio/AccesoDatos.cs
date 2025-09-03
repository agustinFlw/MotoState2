using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
// Driver PostgreSQL
using Dominio;
using System.Data;

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
            // Cambiá los valores según tu base en PostgreSQL
            conexion = new NpgsqlConnection("Host=localhost; Database=motoState; Username=postgres; Password=tu_password");
            comando = new NpgsqlCommand();
        }

        public void setearConsulta(string consulta)
        {
            comando.CommandType = CommandType.Text;
            comando.CommandText = consulta;
        }

        public void ejecutarLectura()
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

        public void ejecutarAccion()
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

        public void setearParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
        }

        public void cerrarConexion()
        {
            if (lector != null && !lector.IsClosed)
                lector.Close();
            conexion.Close();
        }

        // Ejemplo: obtener id de un usuario por email
        public int obtenerIdUsuario(string email)
        {
            int id = 0;
            try
            {
                setearConsulta("SELECT id_usuario FROM Usuario WHERE email = @Email");
                setearParametro("@Email", email);
                ejecutarLectura();

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
                cerrarConexion();
            }
            return id;
        }
    }
}