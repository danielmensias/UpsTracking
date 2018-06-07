using CapaLogicaNegocio;
using ShippingTrackingUtilities;
using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;


namespace ConsoleApp
{
    public partial class RastreoEnvio : Form
    {
        private Timer tiempoEspera;
        private CourierLN oCourierLN;

        public RastreoEnvio()
        {
            InitializeComponent();
            oCourierLN = new CourierLN();
            tiempoEspera = new Timer();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {

            try
            {
                tiempoEspera.Tick += new EventHandler(Tiempo);
                int intervaloTiempo = Convert.ToInt32(txtIntervalo.Text.Trim());
                tiempoEspera.Interval = intervaloTiempo * 1000;
                Hide();
                //inicia el monitoreo
                BuscarNumeroRastreo(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Notificaciones(string mensaje)
        {
            notificacion.BalloonTipText = mensaje;
            Console.WriteLine(notificacion.BalloonTipText);
            notificacion.BalloonTipIcon = ToolTipIcon.Info;
            notificacion.BalloonTipTitle = Text;
            notificacion.ShowBalloonTip(1000);
        }

        /// DS_002 07092017 
        /// Tiempo de espera ejecutar la tarea

        private void Tiempo(object sender, EventArgs e)
        {            
            Notificaciones("Wait: " + DateTime.Now.ToString("HH:mm:ss"));
            tiempoEspera.Stop();
            BuscarNumeroRastreo();
        }

        /// DS_002 07092017
        /// Verifica y actualiza el estado de los trackings

        private void BuscarNumeroRastreo()
        {
            string nombreCourier = "ups";
            string informacion = string.Empty;

            Notificaciones("Start: " + DateTime.Now.ToString("HH:mm:ss"));

            DataTable datosTracking = oCourierLN.buscarNumeroTracking("buscaTracking", nombreCourier);

            if (datosTracking.Rows.Count > 0)
            {
                //Establece las Credenciales de Ups
                try
                {
                    ConnectionString.SetupUPSCredential("1D31883556A1614C");

                    TrackingUtilities utilities = new TrackingUtilities();
                
                    datosTracking.Columns.Add(new DataColumn("Estado"));
                    datosTracking.Columns.Add(new DataColumn("Firma"));
                    datosTracking.Columns.Add(new DataColumn("Fecha"));
                    datosTracking.Columns.Add(new DataColumn("Observacion"));

                    foreach (DataRow fila in datosTracking.Rows)
                    {
                        string numeroTrack = fila[2].ToString().Trim(); //"1Z66753ED365962288";
                        string[] fechaHora;

                        utilities.GetTrackingResult(numeroTrack);

                        var result = utilities.ShippingResult;

                        if (result.Message.Length == 0)
                        {
                            if (result.Delivered) //Delivery
                            {
                                fechaHora = result.DeliveredDateTime.Split(' ');

                                fila[4] = result.Status;                                     //Estado de la carga
                                fila[5] = result.SignatureName;                              //Firma de Recibido
                                fila[6] = conversionCadenaFecha(result.DeliveredDateTime);       //Fecha Recepción     
                                fila[7] = result.StatusSummary;                           
                            }
                            else //En Transito
                            {
                                if (result.Status.Length < 20)
                                {
                                    fila[4] = result.Status;                                    //Estado de la carga
                                    fila[6] = conversionCadenaFecha(result.PickupDate);         //Fecha Recepción
                                    fila[7] = result.StatusSummary;
                                }
                                else
                                {                                    
                                    fila[6] = conversionCadenaFecha(result.PickupDate);         //Fecha Recepción
                                    fila[7] = result.Status;
                                }

                            }
                        }                        
                        else
                        {
                            Console.WriteLine("Error: " + result.Message);
                        }                        
                    }
                    //Ingresar la información en la base
                    informacion = oCourierLN.ingresaDatosProcedimiento("registraTracking", datosTracking);

                }
                catch (Exception ex)
                {
                    informacion = oCourierLN.ingresaDatosProcedimiento("registraTracking", datosTracking);                   
                    MessageBox.Show(informacion + "\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            //Inicia nuevamente el intervalo
            if (string.IsNullOrEmpty(informacion))
            {                                
                Console.WriteLine("Inicia: " + DateTime.Now.ToString("HH:mm:ss"));
                tiempoEspera.Start();
            }
            else
            {
                MessageBox.Show(informacion, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Show();
            }

        }

        private string conversionCadenaFecha(string fechaObtiene)
        {

            string fecha = string.Empty;
            string[] contieneFecha = fechaObtiene.Split(' ');
            string[] hora = new string[2];

            fecha = contieneFecha[0];

            //Obtiene la hora
            if (contieneFecha.Length > 1)
            {

                hora[0] = contieneFecha[1].Substring(0, 2);
                hora[1] = contieneFecha[1].Substring(2, 2);
                DateTime horaConvertida = DateTime.Parse(hora[0] + ":" + hora[1], CultureInfo.CurrentCulture);
                fecha += " " + horaConvertida.ToString("HH:mm:ss");
            }

            return fecha;
        }

        private void mostrarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
        }

        private void RastreoEnvio_Move(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                Notificaciones("Stop: " + DateTime.Now.ToString("HH:mm:ss"));                
            }
        }
    }
}
