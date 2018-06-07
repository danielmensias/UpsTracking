using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace ShippingTrackingUtilities
{
    public enum CarrierName { USPS =0, UPS= 1,FedEx =2}

    public class TrackingUtilities
    {
        ITrackingFacility trackingFacility;        
        private ShippingResult shippingResult;
        public ShippingResult ShippingResult { get { return shippingResult; } }
        public string ShippingResultInString { get { return ConvertTrackingResultIntoString(); } }

        // Entry Point.
        public void GetTrackingResult(string trackingNo)
        {
            shippingResult = new ShippingResult();

            string strShippingResult = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(trackingNo))
                {
                    CarrierName carrier = getCarrierName(trackingNo);

                    CredentialValidation(carrier);
                    
                    trackingFacility = new UPSTracking(trackingNo);

                    shippingResult = trackingFacility.GetTrackingResult();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private CarrierName getCarrierName(string trackingNo)
        {
            CarrierName carrierName = CarrierName.USPS;

            if (IsUPSTracking(trackingNo))
                carrierName = CarrierName.UPS;

            return carrierName;
        }

        private bool IsUPSTracking(string trackingNo)
        {
            return trackingNo.Trim().ToUpper().StartsWith("1Z");
        }

        private void CredentialValidation(CarrierName carrier)
        {            
            if (!HasUPSCredentialSetup())
            throw new Exception("Please setup UPS credential first, before tracking UPS package.");
            
        }
        
        private bool HasUPSCredentialSetup()
        {
            return !string.IsNullOrEmpty(ConnectionString.UPS_ACCESS_LICENSE_NO);
        }

        private string ConvertTrackingResultIntoString()
        {
            string result = "";

            if (shippingResult != null)
            {
                result = "Delivered: " + shippingResult.Delivered;
                result += "\n\nService Type: " + shippingResult.ServiceType;
                result += "\nStatusCode: " + shippingResult.StatusCode + " " + "Status: " + shippingResult.Status;
                result += "\nSummary: " + shippingResult.StatusSummary;

                if (shippingResult.Delivered)
                    result += "\nDelivered On: " + shippingResult.DeliveredDateTime;
                else
                {
                    if (!string.IsNullOrEmpty(shippingResult.Message))
                        result += "\nMessage: " + shippingResult.Message;
                    if (!string.IsNullOrEmpty(shippingResult.ScheduledDeliveryDate))
                        result += "\nScheduled Delivery Date: " + shippingResult.ScheduledDeliveryDate;
                }

                if (ConnectionString.ToShowDetails)
                {
                    if (shippingResult.TrackingDetails.Count > 0)
                    {
                        result += "\n\nTracking Details: ";

                        foreach (var detail in shippingResult.TrackingDetails)
                        {
                            result += "\nEventDateTime: " + detail.EventDateTime;
                            result += " Event: " + detail.Event;
                            result += "\nEvent Address: " + detail.EventAddress;
                        }
                    }
                }
            }
            else
            {
                result = "No result found.";
            }           

            return result;
        }
    }
}
