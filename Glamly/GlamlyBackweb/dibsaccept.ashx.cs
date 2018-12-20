using GlamlyBackweb.Library;
using GlamlyData;
using GlamlyServices.Entities;
using GlamlyServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlamlyBackweb
{
    /// <summary>
    /// Summary description for dibsaccept
    /// </summary>
    public class dibsaccept : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            Helper helpers = new Helper();
            UserServices _userService = new UserServices();
            List<string> messages = new List<string>();
            DibsPayment dipsobj = new DibsPayment();
            string redirect = "~/";
            //string notificationId1 = string.Empty;
            //string notificationId2 = string.Empty;

            try
            {
                messages.Add($"{DateTime.UtcNow.ToString("HH:mm:ss.fff")} - Entered");
                foreach (string key in context.Request.Params.Keys)
                    messages.Add($"{key} = {context.Request.Params[key]}");

                // Get post data
                string amount = context.Request.Params["amount"] ?? string.Empty;
                string approvalcode = context.Request.Params["approvalcode"] ?? string.Empty;
                string authkey = context.Request.Params["authkey"] ?? string.Empty;
                string currency = context.Request.Params["currency"] ?? string.Empty;
                string orderid = context.Request.Params["orderid"] ?? string.Empty;
                string paytype = context.Request.Params["paytype"] ?? string.Empty;
                string transact = context.Request.Params["transact"] ?? string.Empty;

                if (authkey == Hashing.GetMD5HashString(DibsPayment.MD5K2 + Hashing.GetMD5HashString($"{DibsPayment.MD5K1}transact={transact}&preauth=true&currency={currency}")))
                {
                    wp_glamly_payment payment = _userService.GetPaymentById(Convert.ToInt32(orderid.Substring(1)));

                    if (payment != null)
                    {                        
                        payment.acquirer = !string.IsNullOrEmpty(context.Request.Params["acquirer"]) ? context.Request.Params["acquirer"] : string.Empty;
                        payment.amount = (Convert.ToInt32(amount) / 100).ToString();
                        payment.approvalcode = approvalcode;
                        payment.cardexpdate = !string.IsNullOrEmpty(context.Request.Params["cardexpdate"]) ? context.Request.Params["cardexpdate"] : string.Empty;
                        payment.cardnomask = !string.IsNullOrEmpty(context.Request.Params["cardnomask"]) ? context.Request.Params["cardnomask"] : string.Empty;
                        payment.cardprefix = !string.IsNullOrEmpty(context.Request.Params["cardprefix"]) ? context.Request.Params["cardprefix"] : string.Empty;
                        payment.currency = currency;
                        payment.lang = !string.IsNullOrEmpty(context.Request.Params["lang"]) ? context.Request.Params["lang"] : string.Empty;
                        payment.merchant = !string.IsNullOrEmpty(context.Request.Params["merchant"]) ? context.Request.Params["merchant"] : string.Empty;
                        payment.orderid = orderid;
                        payment.paytype = paytype;
                        payment.test = !string.IsNullOrEmpty(context.Request.Params["test"]) ? context.Request.Params["test"] : string.Empty;
                        payment.transact = transact;

                        payment.isdeleted = "false";

                        var paymentid = _userService.updatepaymentdata(payment);

                        // update the booking for notification.
                        //var bookingdetail = _userService.GetBookingByBookingId(payment.bookingid);

                        //if(helpers.ConvertToUtcDateTime(bookingdetail.datetime).Date > DateTime.UtcNow.Date)
                        //{
                        //    notificationId1 = helpers.SendPushNotificationSchedule(bookingdetail.userid.Value, "Du har en bokning imorgon", helpers.ConvertToUtcDateTime(bookingdetail.datetime).AddDays(-1));
                        //}
                        //else
                        //{
                        //    notificationId1 = "";
                        //}                
                        //notificationId2 = helpers.SendPushNotificationSchedule(bookingdetail.userid.Value, "Du har en bokning om en timma", helpers.ConvertToUtcDateTime(bookingdetail.datetime).AddHours(-1));
                        //bookingdetail.notificationbyday = notificationId1;
                        //bookingdetail.notificationbyhour = notificationId2;
                        //_userService.updatebookingdata(bookingdetail);

                        // Redirect
                        redirect = "~/DibsTemplate.html?path=" + HttpUtility.UrlEncode("/Success");
                    }
                }
                else
                    messages.Add($"{DateTime.UtcNow.ToString("HH:mm:ss.fff")} - Invalid authkey");
            }
            finally
            {
            }

            messages.Add($"{DateTime.UtcNow.ToString("HH:mm:ss.fff")} - Done");
            messages.Add("====================");

            // reKy.Log($"{DateTime.UtcNow.ToString("yyyyMMdd")}_dibsaccept.txt", string.Join("\r\n", messages), null);

            // Redirect
            context.Response.Redirect(redirect);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}