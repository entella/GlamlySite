using GlamlyBackweb.Library;
using GlamlyData;
using GlamlyServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlamlyBackweb
{
    /// <summary>
    /// Summary description for dibscancel
    /// </summary>
    public class dibscancel : IHttpHandler
    {
        UserServices _userService = new UserServices();
        wp_glamly_payment payment = new wp_glamly_payment();
        string redirect = "~/";
        ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string orderid = context.Request.Params["orderid"] ?? string.Empty;

                if (!string.IsNullOrEmpty(orderid))
                {
                    payment = _userService.GetPaymentById(Convert.ToInt32(orderid.Substring(1)));

                    if (payment != null)
                    {
                        var bookingdetail = _userService.updatebookingstatusbybookingid(payment.bookingid);

                        redirect = "~/DibsTemplate.html?path=" + HttpUtility.UrlEncode("/Cancel");
                    }
                }
            }
            catch (Exception)
            {
                
            }
                                                     
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