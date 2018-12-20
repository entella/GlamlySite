
using GlamlyServices.Entities;
using GlamlyServices.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace GlamlyBackweb.Library
{
    public class EmailHelper
    {


        #region Templates

        JObject arrayLanguagefile;
        string AppLanguagePreferences = "en-US";
        private IUserServices _userService = new UserServices();

        public EmailHelper()
        {
            using (StreamReader r = new StreamReader(HttpContext.Current.Server.MapPath("~/Scripts\\Resources\\emails.json")))
            {
                string json = r.ReadToEnd();
                arrayLanguagefile = JObject.Parse(json);
            }
        }

        public void SendEmailWithTemplatedCancelBooking(string useremail,  DateTime datetime,string services,string types)
        {
            //  var userKey = GetUserKeyForPasswordByUserId(userId);

            string body = string.Empty;
            string Time = datetime.ToString("HH:mm");
            string Date = datetime.ToString("yyyy/MM/dd");


            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~//Views//Templates//CancelBooking.html")))
            {
                body = reader.ReadToEnd();
            }
            //Replace the static text
            body = body.Replace("{UserEmail}", useremail);
            body = body.Replace("{Service}", services);
            body = body.Replace("{Types}", types);
            body = body.Replace("{BookingDate}", Date);
            body = body.Replace("{Time}", Time);
            System.Text.StringBuilder returnList = new System.Text.StringBuilder();

            //replacing the body in layout html
            body = body.Replace("{body}", body);
            //Subject of email
            string from = "FromEmail".GetValueFromWebConfig();
            SendEmail(from, useremail, "Avbokningsbekräftelse  " + services + " " + Date + "!", body);
        }

       
        public void SendEmailWithTemplatedAddStylistUser(string useremail, string password,string username)
        {
            //  var userKey = GetUserKeyForPasswordByUserId(userId);

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~//Views//Templates//AddUser.html")))
            {
                body = reader.ReadToEnd();
            }
            //Replace the static text
            body = body.Replace("{UserEmail}", useremail);
            body = body.Replace("{Password}", password);
            body = body.Replace("{name}", username);
            System.Text.StringBuilder returnList = new System.Text.StringBuilder();

            //replacing the body in layout html
            body = body.Replace("{body}", body);
            //Subject of email
            string from = "FromEmail".GetValueFromWebConfig();
            SendEmail(from, useremail, "Välkommen till Glamly!", body);
        }

        public void SendEmailWithTemplateResetPasswordUser(int userId, string useremail, string subject)
        {
            var userKey = GetUserKeyForPasswordByUserId(userId);

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~//Views//Templates//ResetPassword.html")))
            {
                body = reader.ReadToEnd();
            }

            string domainUrl = Helper.BaseUrl + "#/ResetPassword?id=" + userKey;

           // subject = getTranslatedValuebyKey(subject);

            //Replace the static text
          
            body = body.Replace("{RESET_PASSWORD_LINK}", domainUrl);
            System.Text.StringBuilder returnList = new System.Text.StringBuilder();

            //replacing the body in layout html
            body = body.Replace("{body}", body);
            //Subject of email
            string from = "FromEmail".GetValueFromWebConfig();
            SendEmail(from, useremail, "Återställ lösenord", body);
        }

        public void SendEmailWithTemplatedAssignedtoStylist(string firstname, string useremail, string customerfirstname, string customersurname, DateTime datetime, string email, string address, string city, string comments, string zipcode, string service, string types, string mobile)
        {
            //  var userKey = GetUserKeyForPasswordByUserId(userId);

            string body = string.Empty;
            string Time = datetime.ToString("HH:mm");
            string Date = datetime.ToString("yyyy/MM/dd");


            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~//Views//Templates//AssigntoStylist.html")))
            {
                body = reader.ReadToEnd();
            }
            //Replace the static text

            body = body.Replace("{UserEmail}", useremail == "null" ? "" : useremail);
            body = body.Replace("{stylistname}", firstname == "null" ? "" : firstname);
            body = body.Replace("{CustomerName}", customerfirstname + "" + customersurname);
            body = body.Replace("{Service}", service == "null" ? "" : service);
            body = body.Replace("{Types}", types == "null" ? "" : types);
            body = body.Replace("{BookingDate}", Date == "null" ? "" : Date);
            body = body.Replace("{Address}", address == "null" ? "" : address);
            body = body.Replace("{zipcode}", zipcode == "null" ? "" : zipcode);
            body = body.Replace("{City}", city == "null" ? "" : city);
            body = body.Replace("{Comments}", comments == "null" ? "" : comments);
            body = body.Replace("{Time}", Time == "null" ? "" : Time);          
            body = body.Replace("{Telefonnummer}", mobile == "null" ? "" : mobile);            
           

            //replacing the body in layout html
            body = body.Replace("{body}", body);
            //Subject of email
            string from = "FromEmail".GetValueFromWebConfig();
            SendEmail(from, useremail, "Ny bokning är tilldelad  " + service + " " + Date + "!", body);
        }


        public string GetUserKeyForPasswordByUserId(int userId)
        {
            UserResetPassword userResetPassword = new UserResetPassword();
            userResetPassword.UserId = userId;
            userResetPassword.UserKey = Guid.NewGuid().ToString("N");
            userResetPassword.RequestTime = DateTime.Now;

            var isSave = _userService.SetUserResetPassword(userResetPassword);
            if (isSave)
                return userResetPassword.UserKey;
            else
                throw new Exception();
        }


        private string getTranslatedValuebyKey(string KeyName)
        {
            string value = "";
            foreach (var item in arrayLanguagefile)
            {
                if (item.Key == AppLanguagePreferences)
                {
                    JObject obj = JObject.Parse(item.Value.ToString());
                    return value = (string)obj[KeyName];
                }
            }
            return value;
        }

        #endregion

        #region private

        private static bool SendEmail(string from, string to, string subject, string body)
        {
            //string TempToEmail = "TempToEmail".GetValueFromWebConfig();
            //if (!string.IsNullOrWhiteSpace(TempToEmail))
            //    to = TempToEmail;

            try
            {
                var mailMessage = new MailMessage(from, to) { Subject = subject, Body = body };

                if (mailMessage.Body != "")
                {
                    mailMessage.IsBodyHtml = true;
                    var smtp = new SmtpClient();
                    smtp.EnableSsl = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableSsl"]) ? Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]) : true;
                    smtp.Host = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Host"]) ? ConfigurationManager.AppSettings["Host"] : "smtp.gmail.com";
                    smtp.Port = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Port"]) ? Convert.ToInt32(ConfigurationManager.AppSettings["Port"]) : 587;

                    System.Net.NetworkCredential networkCredential = new System.Net.NetworkCredential();

                    if (ConfigurationManager.AppSettings["UserName"] != null && ConfigurationManager.AppSettings["Password"] != null)
                    {
                        networkCredential.UserName = ConfigurationManager.AppSettings["UserName"]; //reading from web.config  
                        networkCredential.Password = ConfigurationManager.AppSettings["EmailPassword"]; //reading from web.config  

                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = networkCredential;
                    }

                    smtp.Send(mailMessage);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        #endregion
    }
}