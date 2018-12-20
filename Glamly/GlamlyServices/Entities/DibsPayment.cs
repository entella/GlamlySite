using System.Configuration;
using System.Web;

namespace GlamlyServices.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class DibsPayment
    {
        public static string MD5K1
        {
            get
            {
                return ConfigurationManager.AppSettings["DibsMD5K1"];
            }
        }

        public static string MD5K2
        {
            get
            {
                return ConfigurationManager.AppSettings["DibsMD5K2"];
            }
        }

        public static string MerchantId
        {
            get
            {
                return ConfigurationManager.AppSettings["DibsMerchantId"];
            }
        }

        public static string Currency
        {
            get
            {
                return ConfigurationManager.AppSettings["DibsCurrency"]; 
            }
        }
        public static string Language
        {
            get
            {
                return ConfigurationManager.AppSettings["DibsLanguage"]; 
            }
        }

        public static bool Test
        {
            get
            {
                return (ConfigurationManager.AppSettings["DibsTest"] == "1");
            }
        }
    }
}