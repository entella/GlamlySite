using GlamlyData;

using GlamlyServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlamlyServices.Services
{
    public interface IUserServices
    {

        #region "User Interface"

        List<wp_users> GetUser();

        wp_users GetUser(int id);

        int updateuserdata(wp_usermeta user);

        int updateuserpassword(wp_users user);

        List<wp_usermeta> GetCustomer();

        wp_usermeta GetCustomer(int id);

        List<wp_usermeta> GetStylist();

        wp_usermeta GetStylist(int id);

        List<wp_usermeta> GetStylist_New(int recordsPerPage, int pageNumber);

        List<wp_usermeta> GetUserDetail();

        wp_usermeta GetUserDetailById(int id);

        wp_users validationUser(string username, string password);

        int Saveuserdata(wp_users logindata);

        int Saveusermedadata(wp_usermeta logindata);

        wp_users GetUserByEmailId(string emailId);

        wp_users validationFacebookUser(string fbid);

        wp_usermeta GetUserMetadatakeybyId(decimal userid);

        List<wp_usermeta> GetUserMetadatabyId(decimal userid);

        bool IsFacebookLogin(int userid, string facebookid);

        wp_users validationUserEmail(string username);

        #endregion

        #region "Services Interface"

        List<wp_glamly_servicestypes> GetServiceTypes();

        wp_glamly_servicestypes GetServiceTypeById(int id);

        List<wp_glamly_servicestypes> GetTypesByServiceId(int id);

        List<wp_glamly_services> GetServices();

        List<wp_glamly_services> GetServiceList();

        List<wp_glamly_services> GetServicesTypes(int id);

        wp_glamly_services GetServicesById(int id);

        bool DeleteStylist(int userid);

        bool DeleteServiceType(int Serviceid);

        int updateservicetypes(wp_glamly_servicestypes types);

        int SaveServicetype(wp_glamly_servicestypes types);

        int SaveService(wp_glamly_services service);
        #endregion
    
        #region "User Reset Password Interface"

        bool SetUserResetPassword(UserResetPassword userResetPassword);

        bool DeleteUserResetPasswordByUserId(int userId);

        wp_glamly_userresetpassword GetUserResetPasswordByUserKey(string userKey);

        #endregion

        #region "Booking Interface"

        List<wp_glamly_servicesbookings> GetBookings();

        wp_glamly_servicesbookings GetBookingByBookingId(string bookingid);

        List<wp_glamly_servicesbookings> GetStylistUser(int recordsPerPage, int pageNumber);

        List<wp_glamly_servicesbookings> GetBookingByStatus(string status);

        wp_glamly_servicesbookings GetBookingById(int id);

        List<wp_glamly_servicesbookings> GetBookingByUserId(int id);

        string savebookingdata(wp_glamly_servicesbookings bookings);

        int updatebookingdata(wp_glamly_servicesbookings bookings);

        int updatestylistdata(wp_usermeta userdetails);

        bool DeleteBooking(string bookingid);

        wp_glamly_servicesbookings SendDeleteBookingMail(string bookingid);

        bool ApprovedBookingByAdmin(string bookingid, int stylistid);

        int GetIdByName(string serviceName);

        bool updatebookingstatusbybookingid(string bookingid);

        int GetBookingBystylistid(int stylistid);

        #endregion

        #region "Payment Interface"

        int savepaymentdata(wp_glamly_payment payment);

        List<wp_glamly_payment> GetPaymentList();

        List<wp_glamly_payment> GetPaymentByUserId(int id);

        wp_glamly_payment GetPaymentById(int id);

        #endregion

        #region "Calendar Interface"

        List<wp_glamly_servicesbookings> GetCalendarBookings();

        int savecalendardata(wp_glamly_stylistschedules bookings);

        List<wp_glamly_stylistschedules> Getstylistcalendar();

        wp_glamly_stylistschedules GetstylistcalendarByStylistId(int stylistid);

        int updateStylistSchedule(wp_glamly_stylistschedules schedule);


        #endregion

        #region "StylistPage Interface"

        int SaveStylistPage(wp_glamly_stylistTemp StylistPagedata);

        List<wp_glamly_stylistTemp> GetStylistPageList();

        wp_glamly_stylistTemp GetStylistPageById(int pageid);

        int updateStylistPagedata(wp_glamly_stylistTemp stylistpage);

        bool DeleteStylistPage(int StylistPageid);

        #endregion

        #region "FAQ Interface"

        List<wp_glamly_faq> GetFAQ();

        int SaveFAQ(wp_glamly_faq faqdata);

        wp_glamly_faq GetFAQById(int id);

        int updateFAQdata(wp_glamly_faq faqupdateddata);

        bool DeleteFAQ(int faqid);


        #endregion

































    }
}