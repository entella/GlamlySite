using GlamlyData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Conversive.PHPSerializationLibrary;
using System.Web.Script.Serialization;
using System.Data.Entity.Core;
using GlamlyServices.Entities;

namespace GlamlyServices.Services
{

    public class UserServices : IUserServices
    {       
        private GlamlyEntities _context = new GlamlyEntities();
        Serializer serialize = new Serializer();

        #region "User Details"

        public List<wp_users> GetUser()
        {
            return _context.wp_users.ToList();
        }

        public wp_users GetUser(int id)
        {
            return _context.wp_users.Where(x => x.ID == id).FirstOrDefault();
        }

        public List<wp_usermeta> GetUserDetail()
        {
            return _context.wp_usermeta.ToList();
        }

        public wp_usermeta GetUserDetailById(int id)
        {
            return _context.wp_usermeta.Where(x => x.user_id == id).FirstOrDefault();
        }

        public wp_users GetUserByEmailId(string emailId)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    return context.wp_users.FirstOrDefault(x => x.user_email == emailId);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public wp_usermeta GetUserMetadatakeybyId(decimal userid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    return context.wp_usermeta.FirstOrDefault(ud => ud.user_id == userid);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public wp_users validationFacebookUser(string fbid)
        {
            try
            {
                wp_usermeta objfb = new wp_usermeta();
                wp_users obj = new wp_users();
                objfb = _context.wp_usermeta.FirstOrDefault(ud => ud.meta_key == "facebook_id" && ud.meta_value.Contains(fbid));
                if (objfb != null)
                {
                    obj.ID = objfb.user_id;
                }
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public wp_users validationUser(string username,string password)
        {
            try
            {

                using (var context = new GlamlyEntities())
                {

                    return context.wp_users.FirstOrDefault(x => x.user_email == username && x.user_pass == password);

                }
            }
            catch (Exception )
            {
                return null;
            }
        }

        public wp_users validationUserEmail(string username)
        {
            try
            {

                using (var context = new GlamlyEntities())
                {

                    return context.wp_users.FirstOrDefault(x => x.user_email == username);

                }
            }
            catch (Exception )
            {
                return null;
            }
        }

        public bool IsFacebookLogin(int userid, string facebookid)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            bool isfacebook = false;
            var usermetadata = GetUserMetadatakeybyId(userid);
            var desearlize = serialize.Deserialize(usermetadata.meta_value);
            if (desearlize != null)
            {
                UserData usercollection = serializer.Deserialize<UserData>(Convert.ToString(desearlize));
                var fbid = usercollection.user_facebookid;
                if (!string.IsNullOrEmpty(fbid) && fbid.Trim().ToLower() == facebookid.Trim().ToLower())
                    isfacebook = true;
            }
            return isfacebook;
        }

        public int Saveuserdata(wp_users userdata)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(userdata).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    return Convert.ToInt32(userdata.ID);
                }
            }
            catch (Exception )
            {
                throw new Exception();
            }
        }

        public int Saveusermedadata(wp_usermeta usermetadata)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(usermetadata).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    return Convert.ToInt32(usermetadata.user_id);
                }
            }
            catch (Exception )
            {
                throw new Exception();
            }
        }

        public List<wp_usermeta> GetStylist_New(int recordsPerPage, int pageNumber)
        {

            try
            {
                using (var context = new GlamlyEntities())
                {
                    var skipRecords = (pageNumber - 1) * recordsPerPage;
                    return _context.wp_usermeta.Where(m => m.meta_key == "pro_logindata").Skip(skipRecords).Take(recordsPerPage).ToList();
                }
            }
            catch (Exception )
            {
                return null;
            }          
        }

        public List<wp_usermeta> GetStylist()
        {
            return _context.wp_usermeta.Where(m => m.meta_key == "pro_logindata").ToList();
        }

        public List<wp_usermeta> GetCustomer()
        {
            return _context.wp_usermeta.Where(m => m.meta_key == "customer_logindata" || m.meta_key == "customerfb_logindata").ToList();
        }

        public wp_usermeta GetStylist(int userid)
        {

            return _context.wp_usermeta.Where(x => x.user_id == userid).FirstOrDefault();           
        }

        public wp_usermeta GetCustomer(int userid)
        {

            return _context.wp_usermeta.Where(x => x.user_id == userid && x.meta_key == "customer_logindata" || x.meta_key == "customerfb_logindata").FirstOrDefault();
        }

        public List<wp_usermeta> GetUserMetadatabyId(decimal userid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    return context.wp_usermeta.Where(ud => ud.user_id == userid).ToList();
                }
            }
            catch (Exception )
            {
                return null;
            }
        }

        #endregion

        #region "Service Details"

        public List<wp_glamly_services> GetServices()
        {
            return _context.wp_glamly_services.Where(b => b.status == 1).ToList();
        }

        public wp_glamly_services GetServicesById(int id)
        {
            return _context.wp_glamly_services.SingleOrDefault(x => x.id == id && x.status == 1);
        }

        public wp_glamly_servicestypes GetServiceTypeById(int id)
        {
            return _context.wp_glamly_servicestypes.SingleOrDefault(x => x.id == id);
        }

        public List<wp_glamly_servicestypes> GetTypesByServiceId(int id)
        {
            return _context.wp_glamly_servicestypes.Where(x => x.serviceid == id && x.status == 1).ToList();
        }

        public List<wp_glamly_services> GetServicesTypes(int id)
        {
            return _context.wp_glamly_services.Include("wp_glamly_servicestypes").Where(s => s.id == id).ToList();
        }

        public List<wp_glamly_services> GetServiceList()
        {
            return _context.wp_glamly_services.ToList();
        }

        public List<wp_glamly_servicestypes> GetServiceTypes()
        {
            return _context.wp_glamly_servicestypes.ToList();
        }

        public int updateservicetypes(wp_glamly_servicestypes types)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(types).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return Convert.ToInt32(types.id);
                }
            }
            catch (Exception )
            {
                throw new Exception("User data not updated");
            }
        }

        public int SaveServicetype(wp_glamly_servicestypes types)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(types).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    return types.id;
                }
            }
            catch (Exception )
            {
                throw new Exception();
            }
        }

        public int SaveService(wp_glamly_services service)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(service).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    return service.id;
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public bool DeleteServiceType(int Serviceid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToDelete = context.wp_glamly_servicestypes.FirstOrDefault(x => x.id == Serviceid);
                    userToDelete.status = 0;
                    context.Entry(userToDelete).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region "User Reset Password Details"

        public bool SetUserResetPassword(UserResetPassword userResetPassword)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userResetPasswordObjectDB = context.wp_glamly_userresetpassword.FirstOrDefault(x => x.userid == userResetPassword.UserId);
                    if (userResetPasswordObjectDB != null)
                        context.Entry(userResetPasswordObjectDB).State = System.Data.Entity.EntityState.Deleted;

                    context.Entry(userResetPassword).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();

                    return true;
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public bool DeleteUserResetPasswordByUserId(int userId)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userResetPasswordToDelete = context.wp_glamly_userresetpassword.FirstOrDefault(x => x.userid == userId);
                    if (userResetPasswordToDelete != null)
                        context.Entry(userResetPasswordToDelete).State = System.Data.Entity.EntityState.Deleted;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public wp_glamly_userresetpassword GetUserResetPasswordByUserKey(string userKey)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    return context.wp_glamly_userresetpassword.FirstOrDefault(x => x.userkey == userKey);
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public int updateuserpassword(wp_users user)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToUpdatePwd = context.wp_users.FirstOrDefault(x => x.ID == user.ID);
                    userToUpdatePwd.user_pass = user.user_pass;
                    userToUpdatePwd.user_activation_key = user.user_activation_key;
                    context.Entry(userToUpdatePwd).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return Convert.ToInt32(user.ID);
                }
            }
            catch (Exception)
            {
                throw new Exception("User data not updated");
            }
        }

        #endregion

        #region "Booking Details"

        public List<wp_glamly_servicesbookings> GetBookings()
        {
            return _context.wp_glamly_servicesbookings.Where(b => b.isdeleted == "false" && !string.IsNullOrEmpty(b.servicewithtypes)).ToList();
        }

        public wp_glamly_servicesbookings GetBookingById(int id)
        {
            return _context.wp_glamly_servicesbookings.SingleOrDefault(x => x.id == id);
        }

        public List<wp_glamly_servicesbookings> GetBookingByStatus(string status)
        {
            try
            {
                return _context.wp_glamly_servicesbookings.Where(x => x.status == status).ToList();

            }
            catch (Exception)
            {
                return null;
            }
        }
           
        public wp_glamly_servicesbookings GetBookingByBookingId(string bookingid)
        {
            return _context.wp_glamly_servicesbookings.SingleOrDefault(x => x.bookingid == bookingid && x.isdeleted == "false");
        }
          
        public string savebookingdata(wp_glamly_servicesbookings bookings)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(bookings).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    return bookings.bookingid;
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }
      
        public int updatebookingdata(wp_glamly_servicesbookings bookings)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(bookings).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return bookings.id;
                }
            }
            catch (Exception)
            {
                throw new Exception("booking not updated");
            }
        }       
       
        public List<wp_glamly_servicesbookings> GetBookingByUserId(int id)
        {
            return _context.wp_glamly_servicesbookings.Where(x => x.userid == id).ToList();
        }

        public int updateuserdata(wp_usermeta user)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return Convert.ToInt32(user.user_id);
                }
            }
            catch (Exception)
            {
                throw new Exception("User data not updated");
            }
        }

        public bool DeleteBooking(string bookingid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToDelete = context.wp_glamly_servicesbookings.FirstOrDefault(x => x.bookingid == bookingid);
                    userToDelete.isdeleted = "true";
                    userToDelete.workflowstatus = (int)BookingStatus.CancelByAmin;
                    context.Entry(userToDelete).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
      
        public List<wp_glamly_servicesbookings> GetStylistUser(int recordsPerPage, int pageNumber)
        {        
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var skipRecords = (pageNumber - 1) * recordsPerPage;
                    return context.wp_glamly_servicesbookings.Where(x => x.isdeleted == "false" ).OrderBy(y => y.id).Skip(skipRecords).Take(recordsPerPage).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
       
        public int updatestylistdata(wp_usermeta userdetails)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(userdetails).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return Convert.ToInt32(userdetails.user_id);
                }
            }
            catch (OptimisticConcurrencyException)
            {
                throw new Exception();
            }
        }
             
        public bool ApprovedBookingByAdmin(string bookingid, int stylistid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToUpdate = context.wp_glamly_servicesbookings.FirstOrDefault(x => x.bookingid == bookingid);
                    userToUpdate.workflowstatus = (int)BookingStatus.ApprovedByAdmin;
                    userToUpdate.stylistid = stylistid;
                    context.Entry(userToUpdate).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int GetIdByName(string serviceName)
        {
            return _context.wp_glamly_services.Where(x => x.servicename ==  serviceName.Trim()).FirstOrDefault().id;
        }

        public bool DeleteStylist(int userid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToDelete = context.wp_users.FirstOrDefault(x => x.ID == userid);
                    userToDelete.user_status = 0;                    
                    context.Entry(userToDelete).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
       
        public wp_glamly_servicesbookings SendDeleteBookingMail(string bookingid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userResetPasswordToDelete = context.wp_glamly_servicesbookings.FirstOrDefault(x => x.bookingid == bookingid);
                    if (userResetPasswordToDelete != null)
                        context.Entry(userResetPasswordToDelete).State = System.Data.Entity.EntityState.Deleted;                 
                    context.SaveChanges();

                    return userResetPasswordToDelete;
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public bool updatebookingstatusbybookingid(string bookingid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToUpdate = context.wp_glamly_servicesbookings.FirstOrDefault(x => x.bookingid == bookingid);
                    userToUpdate.workflowstatus = (int)BookingStatus.CancelByCustomer;                   
                    context.Entry(userToUpdate).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
       
        public int GetBookingBystylistid(int stylistid)
        {
            return _context.wp_glamly_servicesbookings.Count(x => x.stylistid == stylistid && x.isdeleted == "false" && x.workflowstatus == 11);
        }
          
      
      
        public wp_glamly_stylistTemp GetStylistPageById(int pageid)
        {
            return _context.wp_glamly_stylistTemp.FirstOrDefault(x => x.id == pageid);
        }
      
        #endregion
       
        #region "Payment Details"

        public int savepaymentdata(wp_glamly_payment payment)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(payment).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    return payment.id;
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public int updatepaymentdata(wp_glamly_payment payment)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(payment).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return payment.id;
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public List<wp_glamly_payment> GetPaymentList()
        {
            return _context.wp_glamly_payment.Where(b => b.isdeleted == "false").ToList();
        }

        public List<wp_glamly_payment> GetPaymentByUserId(int id)
        {
            return _context.wp_glamly_payment.Where(x => x.userid == id).ToList();
        }

        public wp_glamly_payment GetPaymentById(int id)
        {
            return _context.wp_glamly_payment.SingleOrDefault(x => x.id == id);
        }

        #endregion

        #region "Calendar Details"

        public List<wp_glamly_servicesbookings> GetCalendarBookings()
        {
            return _context.wp_glamly_servicesbookings.Where(b => b.isdeleted == "false" && !string.IsNullOrEmpty(b.servicewithtypes)).ToList();


        }

        public int savecalendardata(wp_glamly_stylistschedules calender)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(calender).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    return calender.stylistId;
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public List<wp_glamly_stylistschedules> Getstylistcalendar()
        {
            return _context.wp_glamly_stylistschedules.Where(b => b.isdeleted == "false").ToList();
        }

        public wp_glamly_stylistschedules GetstylistcalendarByStylistId(int stylistid)
        {
            return _context.wp_glamly_stylistschedules.FirstOrDefault(x => x.stylistId == stylistid);
        }

        public int updateStylistSchedule(wp_glamly_stylistschedules schedule)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {

                    context.Entry(schedule).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return Convert.ToInt32(schedule.stylistId);
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion

        #region "StylistPage Details"

        public int SaveStylistPage(wp_glamly_stylistTemp StylistPagedata)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(StylistPagedata).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    return Convert.ToInt32(StylistPagedata.id);
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }
        
        public List<wp_glamly_stylistTemp> GetStylistPageList()
        {         
            return _context.wp_glamly_stylistTemp.Where(b => b.isdeleted == "false").ToList();
        }

        public int updateStylistPagedata(wp_glamly_stylistTemp stylistpage)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(stylistpage).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return Convert.ToInt32(stylistpage.id);
                }
            }
            catch (Exception)
            {
                throw new Exception("Stylist data not updated");
            }
        }

        public bool DeleteStylistPage(int StylistPageid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToDelete = context.wp_glamly_stylistTemp.FirstOrDefault(x => x.id == StylistPageid);
                    userToDelete.isdeleted  = "true";
                    context.Entry(userToDelete).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
      
        #endregion

        #region "FAQ Details"

        public List<wp_glamly_faq> GetFAQ()
        {          
            return _context.wp_glamly_faq.Where(f => f.isdeleted == "false").ToList();
        }

        public wp_glamly_faq GetFAQById(int id)
        {
            return _context.wp_glamly_faq.FirstOrDefault(x => x.id == id);
        }

        public int SaveFAQ(wp_glamly_faq faqdata)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(faqdata).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    return Convert.ToInt32(faqdata.id);
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public int updateFAQdata(wp_glamly_faq faqupdateddata)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(faqupdateddata).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return Convert.ToInt32(faqupdateddata.id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("FAQ data not updated");
            }
        }
        public bool DeleteFAQ(int faqid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToDelete = context.wp_glamly_faq.FirstOrDefault(x => x.id == faqid);
                    userToDelete.isdeleted = "true";
                    context.Entry(userToDelete).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

    }

}
