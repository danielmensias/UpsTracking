namespace ShippingTrackingUtilities
{
    public static class ConnectionString
    {
        internal static string USPS_USERID = "";
        internal static string UPS_ACCESS_LICENSE_NO = "";
        internal static bool ToShowDetails = false;

        public static void SetupUPSCredential(string licenseNO)
        {
            UPS_ACCESS_LICENSE_NO = licenseNO;
        }
    }
}
