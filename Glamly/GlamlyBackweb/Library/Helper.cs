using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;

namespace GlamlyBackweb.Library
{
    /// <summary>
    /// Helper class provides the common functions for the API
    /// </summary>
    public class Helper
    {
        public static string BaseUrl
        {
            get
            {
                string url = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
                url = url.Substring(0, url.IndexOf("/glamlyapi") + 1);
                return url;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static Random random = new Random();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        //public string UserToken
        //{
        //    get
        //    {
        //        string userAutorization = Convert.ToString(HttpContext.Current.Request.Headers["X-User-Authorization"]);
        //        if (!string.IsNullOrWhiteSpace(userAutorization))
        //            return userAutorization;
        //        else
        //            return string.Empty;
        //    }
        //}

        //public string AppConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["AppConnectionString"]?.ToString());
        //public bool IsApiAuthorized
        //{
        //    get
        //    {
        //        bool success = false;
        //        try
        //        {
        //            //Set success
        //            success = (HttpContext.Current.Request.Headers["X-API-Authorization"] == WebConfigurationManager.AppSettings["APIKey"]);
        //            if (!success)
        //                Logs.Add(string.Format("IsApiAuthorized :: APIKey is not authorized."), "Cab");
        //        }
        //        catch (Exception Ex)
        //        {
        //            Logs.Add(string.Format("{0} Method: {1} Error: {2}", DateTime.Now, MethodBase.GetCurrentMethod().Name, Ex.ToString()), "Cab");
        //        }
        //        return success;
        //    }
        //}


        /// <summary>
        /// Method to get the password with salt
        /// </summary>
        /// <returns></returns>
        public static string getPasswordSalt()
        {
            var random = new RNGCryptoServiceProvider();

            // Maximum length of salt
            int maxLength = 8;

            // Empty salt array
            byte[] byteSalt = new byte[maxLength];

            // Build the random bytes
            random.GetNonZeroBytes(byteSalt);

            // Return the string encoded salt
            return Convert.ToBase64String(byteSalt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringToValidate"></param>
        /// <returns></returns>
        public static bool IsEmail(string stringToValidate)
        {
            try
            {
                string pattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

                return Regex.IsMatch(stringToValidate, pattern);
            }
            catch (Exception)
            {

            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="message"></param>
        /// <param name="utcDate"></param>
        /// <returns></returns>
        public string SendPushNotificationSchedule(int userid, string message, DateTime utcDate)
        {
            string id = string.Empty;

            try
            {
                var sendPushNotificationServiceUrl = ConfigurationManager.AppSettings["PushNotificationServiceUrl"].ToString() + "/send.ashx";
                var pushNotificationServiceApiKey = ConfigurationManager.AppSettings["PushNotificationServiceApiKey"].ToString();

                var url = string.Format("{0}?key={1}&userid={2}&message={3}&scheduleutc={4}&return=id",
                                        sendPushNotificationServiceUrl,
                                        pushNotificationServiceApiKey,
                                        userid,
                                        message,
                                        utcDate.ToString("yyyy-MM-ddTHH:mm"));

                var webClient = new WebClient();
                string response = webClient.DownloadString(url);

                if (response.StartsWith("success."))
                    id = response.Replace("success.", "");
            }
            catch (Exception ex)
            {

            }

            return id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="message"></param>
        /// <param name="badge"></param>
        /// <returns></returns>
        public bool SendPushNotificationwithbadge(int userid, string message, int badge)
        {
           // string id = string.Empty;
            bool success = false;
            try
            {
                var sendPushNotificationServiceUrl = ConfigurationManager.AppSettings["PushNotificationServiceUrl"].ToString() + "/send.ashx";
                var pushNotificationServiceApiKey = ConfigurationManager.AppSettings["PushNotificationServiceApiKey"].ToString();

                var url = string.Format("{0}?key={1}&userid={2}&message={3}&badge={4}",
                                        sendPushNotificationServiceUrl,
                                        pushNotificationServiceApiKey,
                                        userid,
                                        message,
                                        badge);

                var webClient = new WebClient();
                string response = webClient.DownloadString(url);

                if (response.StartsWith("success"))
                    success = true;
                 //   id = response.Replace("success.", "");
            }
            catch (Exception ex)
            {

            }

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="message"></param>
        /// <param name="badge"></param>
        /// <returns></returns>
        public bool SendPushNotification(int userid, string message)
        {
            // string id = string.Empty;
            bool success = false;
            try
            {
                var sendPushNotificationServiceUrl = ConfigurationManager.AppSettings["PushNotificationServiceUrl"].ToString() + "/send.ashx";
                var pushNotificationServiceApiKey = ConfigurationManager.AppSettings["PushNotificationServiceApiKey"].ToString();

                var url = string.Format("{0}?key={1}&userid={2}&message={3}",
                                        sendPushNotificationServiceUrl,
                                        pushNotificationServiceApiKey,
                                        userid,
                                        message);

                var webClient = new WebClient();
                string response = webClient.DownloadString(url);

                if (response.StartsWith("success"))
                    success = true;
                //   id = response.Replace("success.", "");
            }
            catch (Exception ex)
            {

            }

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="utcDate"></param>
        /// <returns></returns>
        public string UpdatePushNotificationSchedule(string notificationId, DateTime utcDate)
        {
            string id = string.Empty;

            try
            {
                var sendPushNotificationServiceUrl = ConfigurationManager.AppSettings["PushNotificationServiceUrl"].ToString() + "/send.ashx";
                var pushNotificationServiceApiKey = ConfigurationManager.AppSettings["PushNotificationServiceApiKey"].ToString();

                var url = string.Format("{0}?key={1}&id={2}&scheduleutc={3}",
                                        sendPushNotificationServiceUrl,
                                        pushNotificationServiceApiKey,
                                        notificationId,
                                        utcDate.ToString("yyyy-MM-ddTHH:mm"));

                var webClient = new WebClient();
                webClient.DownloadStringAsync(new Uri(url));
            }
            catch (Exception ex)
            {

            }

            return id;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public string CancelPushNotificationSchedule(string notificationId)
        {
            string id = string.Empty;

            try
            {
                var sendPushNotificationServiceUrl = ConfigurationManager.AppSettings["PushNotificationServiceUrl"].ToString() + "/unsend.ashx";
                var pushNotificationServiceApiKey = ConfigurationManager.AppSettings["PushNotificationServiceApiKey"].ToString();

                var url = string.Format("{0}?key={1}&id={2}",
                                        sendPushNotificationServiceUrl,
                                        pushNotificationServiceApiKey,
                                        notificationId);

                var webClient = new WebClient();
                webClient.DownloadStringAsync(new Uri(url));
            }
            catch (Exception ex)
            {

            }

            return id;
        }

        public DateTime ConvertToUtcDateTime(string Date, string TimeZoneId = "Central Europe Standard Time")
        {
            // Convert unspecified time zone to UTC
            DateTime date = Convert.ToDateTime(Date);
            DateTime utcDate = date.AddTicks(TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId).GetUtcOffset(date).Ticks * -1);
            return utcDate;
        }
    }
}