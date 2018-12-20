using GlamlyBackweb.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlamlyBackweb
{
    /// <summary>
    /// Summary description for dibscallback
    /// </summary>
    public class dibscallback : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            /*List<string> messages = new List<string>();
            ReKy reKy = new ReKy();
            Exception error = null;

            try
            {
                messages.Add($"{DateTime.UtcNow.ToString("HH:mm:ss.fff")} - Entered");
                foreach (string key in context.Request.Form.Keys)
                    messages.Add($"{key} = {context.Request.Form[key]}");

                // Get post data
                string amount = context.Request.Form["amount"];
                string approvalcode = context.Request.Form["approvalcode"];
                string authkey = context.Request.Form["authkey"];
                string currency = context.Request.Form["currency"];
                long orderid = Convert.ToInt64(context.Request.Form["orderid"]);
                string paytype = context.Request.Form["paytype"];
                string transact = context.Request.Form["transact"];

                if (authkey == reKy.GetMD5HashString(ReKy.DibsMD5K2 + reKy.GetMD5HashString($"{ReKy.DibsMD5K1}transact={transact}&amount={amount}&currency={currency}")))
                {
                    // Get payment
                    PaymentBL paymentBL = new PaymentBL();
                    Payment payment = paymentBL.Get(orderid);

                    if (payment != null)
                    {
                        if (payment.TransactionDate == DateTime.MinValue)
                        {
                            // Modify payment
                            payment.TransactionApprovalCode = approvalcode;
                            payment.TransactionDate = DateTime.UtcNow;
                            payment.TransactionId = transact;
                            payment.TransactionPayType = paytype;
                            payment.ToEmptyStrings();

                            // Update payment
                            bool updated = paymentBL.Update(payment);

                            if (updated)
                            {
                                if (payment.Type == Payment.PaymentType.ColorTheme)
                                {
                                    // Restore job
                                    List<SqlParameter> sqlParams = new List<SqlParameter>();
                                    sqlParams.Add(new SqlParameter { DbType = DbType.Int64, ParameterName = "@themeid", Value = payment.ItemId });
                                    new JobBL().Restore(payment.ParentId, "themeid=@themeid", sqlParams);
                                }
                                else if (payment.Type == Payment.PaymentType.Spontaneous)
                                {
                                    // Get user
                                    UserBL userBL = new UserBL();
                                    User user = userBL.Get(payment.UserId);

                                    if (user != null)
                                    {
                                        // Modify user
                                        user.Spontaneous = true;

                                        // Update user
                                        userBL.Update(user);
                                    }
                                }
                                else
                                    messages.Add($"{DateTime.UtcNow.ToString("HH:mm:ss.fff")} - Payment type not found");
                            }
                            else
                                messages.Add($"{DateTime.UtcNow.ToString("HH:mm:ss.fff")} - Failed to update payment");
                        }
                        else
                            messages.Add($"{DateTime.UtcNow.ToString("HH:mm:ss.fff")} - Payment already processed?");
                    }
                    else
                        messages.Add($"{DateTime.UtcNow.ToString("HH:mm:ss.fff")} - Payment not found");
                }
                else
                    messages.Add($"{DateTime.UtcNow.ToString("HH:mm:ss.fff")} - Invalid authkey");
            }
            catch (Exception ex)
            {
                messages.Add($"{DateTime.UtcNow.ToString("HH:mm:ss.fff")} - Error: {ex.Message}");
                error = ex;
            }
            finally
            {
            }

            messages.Add($"{DateTime.UtcNow.ToString("HH:mm:ss.fff")} - Done");
            messages.Add("====================");

            reKy.Log($"{DateTime.UtcNow.ToString("yyyyMMdd")}_dibscallback.txt", string.Join("\r\n", messages), null);

            if (error != null)
                throw error;*/
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