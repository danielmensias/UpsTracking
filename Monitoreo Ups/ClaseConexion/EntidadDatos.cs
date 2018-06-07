using System;
using System.Data;

namespace ClaseEntidadDatos
{
    /// DS_002 08092017
    /// Clase hace refencia a acceso a datos de Courier

    public class CourierAD
    {
        private ConexionSqlServer conexion;

        public CourierAD()
        {
            conexion = new ConexionSqlServer();
        }        

        /// DS_002 07092017
        /// selecciona el SP correspondiente a la accion buscarTracking y llama al
        /// metodo para obtener los datos

        public DataTable obtenerDatosTracking(string accion, string courier)
        {
            DataTable datosTracking = new DataTable();
            string nombreSP = SeleccionarSP(accion);
            if (nombreSP.Length > 0)
            {
                datosTracking = conexion.ObtenerDatosTracking(nombreSP, courier);
            }
            return datosTracking;
        }

        /// DS_002 08092017
        /// Registra los datos usando un dataTable y un SP

        public string ingresaDatosTracking(string cadena, DataTable datos)
        {
            string nombreSP = SeleccionarSP(cadena);

            if (nombreSP.Length > 0)
            {
                return conexion.RegistrarDatosTracking(nombreSP, datos);
            }
            return "";
        }

        /// DS_002 07092017
        /// Busca Procedimiento a ejecutar
        public string SeleccionarSP(string accion)
        {
            string storeProcedure = "";
            switch (accion)
            {
                case "buscaTracking":
                    storeProcedure = "MOR_BuscaTracking";
                    break;
                case "registraTracking":
                    storeProcedure = "MOR_RegistroTracking";
                    break;
            }
            return storeProcedure;
        }

    }
}
