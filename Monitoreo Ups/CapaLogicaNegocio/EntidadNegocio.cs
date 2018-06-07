
using ClaseEntidadDatos;
using System.Data;

namespace CapaLogicaNegocio
{
    /// DS_002 07092017
    /// Clase hace referencia a Currier

    public class CourierLN
    {
        CourierAD oCurrierAD = new CourierAD();

        /// DS_002 07092017
        /// Devuelve la consulta en un DataTable

        public DataTable buscarNumeroTracking(string accion, string nombreCourier)
        {
            return oCurrierAD.obtenerDatosTracking(accion, nombreCourier);
        }

        /// DS_002 07092017
        /// registra numero tracking   
        public string ingresaDatosProcedimiento(string cadena, DataTable datos)
        {
           return oCurrierAD.ingresaDatosTracking(cadena, datos);
        }

    }
}
