using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace ClaseEntidadDatos
{
    class ConexionSqlServer
    {        
        private SqlConnection conexionSql;
        public const string ArchivoBDD = "C:\\Users\\Daniel\\Documents\\Visual Studio 2013\\Projects\\Monitoreo Ups\\configura_bdd\\configuracion.inf";
        
        //DS_005 20112017
        //Lectura de archivo que contiene credenciales de la BDD
        private string[] LecturaArchivo()
        {
            string[] datosConfiguracion = null;            
            string[] credencialesBDD=new string[4];
            try
            {
                if (File.Exists(ArchivoBDD))
                {
                    datosConfiguracion = File.ReadAllLines(ArchivoBDD);
                    for (int i=0;i< datosConfiguracion.Length;i++)
                    {
                        credencialesBDD[i] = datosConfiguracion[i].Split('=')[1];
                    }
                    
                    return credencialesBDD;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return credencialesBDD;
        }

        /// DS_002 07092017
        /// Establece la conexion

        private SqlConnection Conexion()
        {
            string strCadenaConexion;
            conexionSql = null;

            string[] credencialesBase = LecturaArchivo();
            // arma la cadena de conexion con Nombre o ip del server, nombre BDD, user y password
            strCadenaConexion = "Data Source =" + credencialesBase[0] + "; Network Library =DBMSSOCN; Initial Catalog =" + credencialesBase[1] + "; User ID =" + credencialesBase[2] + "; Password =" + credencialesBase[3] + ";";

            try
            {
                conexionSql = new SqlConnection(strCadenaConexion);
                conexionSql.Open();
            }
            catch (Exception ex)
            {
                string msgErrorConexion = ex.Message + "CapaAccesoDatos: Error al instanciar la conexion de SQL";
                Console.WriteLine(msgErrorConexion);
            }

            return conexionSql;
        }

        /// DS_002 07092017
        /// Cierra la conexion establecida

        private void CerrarConexion()
        {
            conexionSql.Close();
        }

        
        /// DS_002 07092017
        /// Devuelve un DataTable con datos de los tracking

        public DataTable ObtenerDatosTracking(string nombreSP, string courier)
        {
            DataTable datosTabla = new DataTable();
            try
            {
                conexionSql = Conexion();

                string sentenciaSql = "exec " + nombreSP + " " + courier;

                SqlDataAdapter adaptador = new SqlDataAdapter(sentenciaSql, conexionSql);
                SqlCommandBuilder cb = new SqlCommandBuilder(adaptador);

                adaptador.Fill(datosTabla);

                CerrarConexion();

                return datosTabla;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public string RegistrarDatosTracking(string nombreSP, DataTable datos)
        {
            string cadena = string.Empty;
            conexionSql = Conexion();
            try
            {
                SqlCommand comand = new SqlCommand(nombreSP);
                comand.CommandType = CommandType.StoredProcedure;
                comand.Connection = conexionSql;
                comand.Parameters.AddWithValue("@datos", datos);
                cadena = (string)comand.ExecuteScalar();
                CerrarConexion();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                CerrarConexion();
            }
            return cadena;
        }
    }
}
