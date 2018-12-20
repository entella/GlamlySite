using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Formatting;
using GlamlyServices.Services;
using GlamlyData;
using System.Web.Script.Serialization;
using Conversive.PHPSerializationLibrary;
using System.Collections;
using System.Configuration;
using Newtonsoft.Json;
using System.Reflection;
using GlamlyBackweb.Library;
using System.IO;
using System.Web;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using GlamlyServices.Entities;

namespace GlamlyBackweb.Controllers
{

    /// <summary>
    /// User controller contains the user information
    /// </summary>
    /// 
    //   [RoutePrefix("api/v1/user")]

    public class UserController : ApiController
    {
        #region Variables     
        private IUserServices _userService = new UserServices();
        EmailHelper emailHelper = new EmailHelper();
        Helper helpers = new Helper();
        Serializer serialize = new Serializer();
        JavaScriptSerializer javaserializer = new JavaScriptSerializer();
        #endregion

        #region "User"

        /// <summary>
        /// Get the user list.
        /// </summary>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpGet]
        public ResponseExtended<List<wp_users>> GetUser()
        {
            var resp = new ResponseExtended<List<wp_users>>();
            var user = new List<wp_users>();
            try
            {
                user = _userService.GetUser();
                if (user != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = user;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.UserNotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.Add(string.Format("{0} Method: {1} Error: {2}", DateTime.Now, MethodBase.GetCurrentMethod().Name, ex.ToString()), "Users");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        /// <summary>
        /// Get the user details by id.
        /// </summary>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpGet]
        public ResponseExtended<UserData> GetUserdetails(int id)
        {
            var resp = new ResponseExtended<UserData>();
            try
            {
                var usermetadata = _userService.GetUserMetadatakeybyId(id);
                if (usermetadata != null)
                {
                    if (string.IsNullOrEmpty(usermetadata.meta_value))
                    {
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                        return resp;
                    }

                    else
                    {
                        var desearlize = serialize.Deserialize(usermetadata.meta_value);
                        UserData usercollection = javaserializer.Deserialize<UserData>(!string.IsNullOrWhiteSpace(desearlize?.ToString()) ? Convert.ToString(desearlize).Replace("\";", "").Replace("];", "]") : usermetadata.meta_value);

                        if (usercollection != null)
                        {
                            resp.ResponseCode = Response.Codes.OK;
                            resp.ResponseData = usercollection;
                            return resp;
                        }
                        else
                        {
                            resp.ResponseCode = Response.Codes.InvalidRequest;
                            resp.ResponseData = usercollection;
                            return resp;
                        }
                    }
                }
                else
                {
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                    return resp;
                }
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "userdetail/id");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        /// <summary>
        /// Update the user data.
        /// </summary>
        /// <param name="userdata"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpPut]
        public ResponseExtended<System.Web.Mvc.JsonResult> UpdateUserDetails(UserData userdata)
        {
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var usermetadata = _userService.GetUserMetadatakeybyId(userdata.ID);
                if (usermetadata != null)
                {
                    //   var desearlize = serialize.Deserialize(usermetadata.meta_value);
                    //  UserData usercollection = javaserializer.Deserialize<UserData>(Convert.ToString(desearlize));

                    var desearlize = serialize.Deserialize(usermetadata.meta_value);
                    UserData usercollection = javaserializer.Deserialize<UserData>(!string.IsNullOrWhiteSpace(desearlize?.ToString()) ? Convert.ToString(desearlize).Replace("\";", "").Replace("];", "]") : usermetadata.meta_value);


                    if (usercollection != null)
                    {
                        usercollection.first_name = userdata.first_name == "null" ? string.Empty : userdata.first_name;
                        usercollection.last_name = userdata.last_name == "null" ? string.Empty : userdata.last_name;
                        usercollection.mobile = userdata.mobile == "null" ? string.Empty : userdata.mobile;
                        usercollection.user_email = !string.IsNullOrEmpty(userdata.user_email) ? userdata.user_email : string.Empty;
                        usercollection.offer = userdata.offer;
                        usercollection.upcomingbookings = userdata.upcomingbookings;
                        usercollection.notificationall = userdata.notificationall;

                        var jsonupdate = JsonConvert.SerializeObject(usercollection);
                        usermetadata.meta_value = jsonupdate;
                        int userid = _userService.updateuserdata(usermetadata);

                        if (userid > 0)
                        {
                            resp.ResponseCode = Response.Codes.OK;
                            resp.ResponseMessage = "User has been updated successfully";
                        }
                        else
                        {
                            resp.ResponseCode = Response.Codes.InvalidRequest;
                        }

                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }

                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updateUser");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        /// <summary>
        /// Get the stylist pro user list.
        /// </summary>
        /// <returns></returns>
        /// 
        [Authorize]
        [Route("prousers"), HttpGet]
        public List<wp_glamly_servicesbookings> GetStylistUser()
        {
            try
            {
                Serializer serialize = new Serializer();
                var stylistlist = new List<wp_glamly_servicesbookings>();
                var servicelist = new List<wp_glamly_services>();
                string serlist = string.Empty;
                string servicetypelist = string.Empty;
                stylistlist = _userService.GetBookings();
                foreach (var item in stylistlist)
                {
                    var stylishservice = serialize.Deserialize(item.service);
                    var stylishtype = serialize.Deserialize(item.type);
                    IEnumerable enumerableservice = stylishservice as IEnumerable;
                    IEnumerable enumerabletype = stylishtype as IEnumerable;
                    if (enumerableservice != null)
                    {
                        foreach (object element in enumerableservice)
                        {
                            wp_glamly_services slist = _userService.GetServicesById(Convert.ToInt32(element));
                            var obj = stylistlist.FirstOrDefault(x => x.id == item.id);
                            if (obj != null)
                                serlist += slist.servicename + ",";
                        }
                        item.service = Convert.ToString(serlist.TrimEnd(','));
                        serlist = string.Empty;
                    }

                    if (enumerabletype != null)
                    {
                        foreach (object element in enumerabletype)
                        {
                            wp_glamly_servicestypes slist1 = _userService.GetServiceTypeById(Convert.ToInt32(element));
                            var obj = stylistlist.FirstOrDefault(x => x.id == item.id);
                            if (obj != null)
                                servicetypelist += slist1.typename + ",";
                        }
                        item.type = Convert.ToString(servicetypelist.TrimEnd(','));
                        servicetypelist = string.Empty;
                    }
                }
                return stylistlist;
            }
            catch (Exception exception)
            {
                //  resp.ResponseCode = Response.Codes.ApiUnauthorized;
                // resp.ResponseMessage = "User is not authenticated by API.";
                //  _logger.Info(string.Format("exception {0}", exception.Message));
                // _logger.Info(string.Format("exception {0}", exception.Source));
                return null;
            }

        }



        /// <summary>
        /// Login the user with valid credentials
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        [Route("login"), HttpPost]
        public ResponseExtended<Dictionary<string, string>> UserLogin(UserData user)
        {
            ResponseExtended<Dictionary<string, string>> resp = new ResponseExtended<Dictionary<string, string>>();

            Token token = new Token();
            try
            {
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.user_email))
                    {
                        if (string.IsNullOrEmpty(user.user_email) || !Helper.IsEmail(user.user_email.Replace("'", "''")))
                        {
                            resp.ResponseCode = Response.Codes.InvalidEmailAddress;
                            return resp;
                        }
                    }
                    if (string.IsNullOrEmpty(user.user_pass) && string.IsNullOrEmpty(user.user_facebookid))
                    {
                        resp.ResponseCode = Response.Codes.InvalidPassword;
                        return resp;
                    }

                    using (var client = new HttpClient())
                    {
                        var form = new Dictionary<string, string>
                       {
                           {"grant_type", "password"},
                           {"username", user.user_email},
                           {"password", user.user_pass},
                           {"usertype", user.user_type},
                           {"facebookid", user.user_facebookid},
                       };
                        var tokenResponse = client.PostAsync(Helper.BaseUrl + "AuthenticationToken", new FormUrlEncodedContent(form)).Result;


                        token = tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;
                        if (string.IsNullOrEmpty(token.Error))
                        {
                            Dictionary<string, string> typeObject = new Dictionary<string, string>();
                            typeObject.Add("Token", Convert.ToString(token.AccessToken));
                            typeObject.Add("Userid", Convert.ToString(token.Id));
                            typeObject.Add("FirstName", Convert.ToString(token.FirstName));
                            typeObject.Add("UserEmail", Convert.ToString(user.user_email));
                            typeObject.Add("Mobile", Convert.ToString(token.Mobile));
                            resp.ResponseData = typeObject;
                            resp.ResponseCode = Response.Codes.OK;
                            resp.ResponseMessage = "Token has been created successfully.";
                        }
                        else
                        {
                            resp.ResponseCode = Response.Codes.ApiUnauthorized;
                        }
                    }
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "userdetail/id");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }


        /// <summary>
        /// Register the user 
        /// </summary>
        /// <param name="LoginModel"></param>
        /// <returns></returns>        
        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> RegisterProUser(UserData LoginModel)
        {
            EmailHelper emailHelper = new EmailHelper();
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            string userSalt = string.Empty;
            string userPassword = string.Empty;
            string hashedPassword = string.Empty;
            string name = string.Empty;
            string customernumber = string.Empty;
            string userJsonObject = string.Empty;
            try
            {
                userSalt = Helper.getPasswordSalt();

                if (!string.IsNullOrEmpty(LoginModel.user_pass))
                {
                    userPassword = Hashing.MD5Hash(LoginModel.user_pass.Trim(), userSalt);
                }

                if (LoginModel != null)
                {

                    bool IsUserEmailExist = false;

                    IsUserEmailExist = _userService.GetUser().Exists(users => users.user_email == LoginModel.user_email);

                    if (IsUserEmailExist)
                    {
                        resp.ResponseCode = Response.Codes.Exists;
                        return resp;
                    }
                    else
                    {
                        wp_users obj = new wp_users();
                        obj.user_email = LoginModel.user_email;
                        obj.user_login = "";
                        obj.user_nicename = "";
                        obj.user_pass = userPassword;
                        obj.user_registered = DateTime.Now;
                        obj.user_status = 0;
                        obj.user_url = "";
                        obj.user_activation_key = userSalt;
                        obj.display_name = "";
                        int user_id = _userService.Saveuserdata(obj);
                        LoginModel.ID = Convert.ToInt32(user_id);
                        LoginModel.offer = true;
                        LoginModel.upcomingbookings = true;
                        LoginModel.notificationall = true;
                        LoginModel.user_type = "Pro";
                        //  LoginModel.user_registered = DateTime.ToString("yyyy-MM-dd HH:mm:ss");
                        LoginModel.mysqldate = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                        userJsonObject = JsonConvert.SerializeObject(LoginModel);
                        var obj1 = new wp_usermeta();
                        obj1.user_id = LoginModel.ID;
                        obj1.meta_key = "pro_logindata";
                        obj1.meta_value = userJsonObject;
                        int metauser_id = _userService.Saveusermedadata(obj1);

                        if (metauser_id > 0)
                        {
                            emailHelper.SendEmailWithTemplatedAddStylistUser(LoginModel.user_email, LoginModel.user_pass, LoginModel.first_name);
                            resp.ResponseCode = Response.Codes.OK;
                        }
                        else
                        {
                            resp.ResponseCode = Response.Codes.InvalidRequest;
                        }
                        return resp;


                    }
                }
                else
                {
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                    return resp;
                }

            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "register");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// Register the user 
        /// </summary>
        /// <param name="LoginModel"></param>
        /// <returns></returns>        
        [Route("register"), HttpPost, AllowAnonymous]
        public ResponseExtended<System.Web.Mvc.JsonResult> RegisterUser(UserData LoginModel)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            string userSalt = string.Empty;
            string userPassword = string.Empty;
            string hashedPassword = string.Empty;
            string name = string.Empty;
            string customernumber = string.Empty;
            string userJsonObject = string.Empty;
            try
            {
                userSalt = Helper.getPasswordSalt();

                if (!string.IsNullOrEmpty(LoginModel.user_pass))
                {
                    userPassword = Hashing.MD5Hash(LoginModel.user_pass.Trim(), userSalt);
                }

                //MySqlConnection connectionString = new MySqlConnection("server=52.209.186.41;user id=usrglamly;password=G@Lm!Y;database=wp_glamly;");
                //connectionString.Open();
                //MySqlTransaction myTrans = connectionString.BeginTransaction();

                if (LoginModel != null)
                {

                    int userid = 0; string usertype = string.Empty; string FacebookIdExist = string.Empty; string UserEmailExist = string.Empty; bool IsFacebookIdExist = false; bool IsUserEmailExist = false;

                    var udata = _userService.GetUser().Where(users => users.user_email == LoginModel.user_email).FirstOrDefault();

                    if (udata != null)
                    {
                        userid = Convert.ToInt32(udata.ID);
                        var usermetadata = _userService.GetUserMetadatakeybyId(userid);
                        if (usermetadata != null)
                        {
                            var desearlize = serialize.Deserialize(usermetadata.meta_value);
                            UserData usercollection = javaserializer.Deserialize<UserData>(!string.IsNullOrWhiteSpace(desearlize?.ToString()) ? Convert.ToString(desearlize).Replace("\";", "").Replace("];", "]") : usermetadata.meta_value);

                            if (usercollection != null)
                            {

                                if (!string.IsNullOrEmpty(LoginModel.user_facebookid) && !string.IsNullOrWhiteSpace(LoginModel.user_facebookid))
                                {
                                    if (LoginModel.user_facebookid == usercollection.user_facebookid)
                                    {
                                        IsFacebookIdExist = true;
                                    }
                                }

                                if (!string.IsNullOrEmpty(LoginModel.user_type) && !string.IsNullOrWhiteSpace(LoginModel.user_type) && udata != null)
                                {
                                    IsUserEmailExist = _userService.GetUser().Exists(users => users.user_email == LoginModel.user_email && usercollection.user_type == LoginModel.user_type);

                                }
                            }
                        }
                    }

                    if (IsFacebookIdExist || IsUserEmailExist)
                    {
                        resp.ResponseCode = Response.Codes.Exists;
                        return resp;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(LoginModel.user_facebookid) && !string.IsNullOrWhiteSpace(LoginModel.user_facebookid))
                        {
                            wp_users obj = new wp_users();
                            obj.user_email = LoginModel.user_email;
                            obj.user_login = "";
                            obj.user_nicename = "";
                            obj.user_pass = "";
                            obj.user_registered = DateTime.MinValue;
                            obj.user_status = 1;
                            obj.user_url = "";
                            obj.user_activation_key = "";
                            obj.display_name = "";

                            int user_id = _userService.Saveuserdata(obj);
                            LoginModel.ID = Convert.ToInt32(user_id);
                            LoginModel.offer = true;
                            LoginModel.upcomingbookings = true;
                            LoginModel.notificationall = true;

                            userJsonObject = JsonConvert.SerializeObject(LoginModel);

                            var obj1 = new wp_usermeta();
                            obj1.user_id = LoginModel.ID;
                            obj1.meta_key = LoginModel.user_type == "customer" ? "customerfb_logindata" : "profb_logindata";
                            obj1.meta_value = userJsonObject;
                            int metauser_id = _userService.Saveusermedadata(obj1);

                            //  myTrans.Commit();
                            resp.ResponseData = new System.Web.Mvc.JsonResult { Data = LoginModel.ID };
                            resp.ResponseCode = Response.Codes.OK;
                            resp.ResponseMessage = "User has been register successfully";
                            resp.ResponseCode = Response.Codes.OK;
                            return resp;
                        }
                        else
                        {
                            wp_users obj = new wp_users();
                            obj.user_email = LoginModel.user_email;
                            obj.user_login = LoginModel.user_login;
                            obj.user_nicename = LoginModel.user_nicename;
                            obj.user_pass = userPassword;
                            obj.user_registered = DateTime.Now;
                            obj.user_status = 0;
                            obj.user_url = "";
                            obj.user_activation_key = userSalt;
                            obj.display_name = "";

                            int user_id = _userService.Saveuserdata(obj);
                            LoginModel.ID = Convert.ToInt32(user_id);
                            LoginModel.offer = true;
                            LoginModel.upcomingbookings = true;
                            LoginModel.notificationall = true;

                            userJsonObject = JsonConvert.SerializeObject(LoginModel);

                            var obj1 = new wp_usermeta();
                            obj1.user_id = LoginModel.ID;
                            obj1.meta_key = LoginModel.user_type == "customer" ? "customer_logindata" : "pro_logindata";
                            obj1.meta_value = userJsonObject;
                            int metauser_id = _userService.Saveusermedadata(obj1);

                            // myTrans.Commit();
                            // resp.ResponseData = new System.Web.Mvc.JsonResult { Data = LoginModel.ID };
                            resp.ResponseMessage = "User has been register successfully";
                            resp.ResponseCode = Response.Codes.OK;
                            return resp;

                        }
                    }
                }
                else
                {
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                    return resp;
                }

            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "register");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        [Authorize]
        [HttpGet]
        public ResponseExtended<List<UserData>> GetAllStylists()
        {
            ResponseExtended<List<UserData>> resp = new ResponseExtended<List<UserData>>();
            var servicesbookingsList = new List<UserData>();
            var user = new List<wp_users>();
            try
            {
                var usermetadata = _userService.GetStylist();
                if (usermetadata != null)
                {
                    foreach (var item in usermetadata)
                    {
                        var stylishservice = serialize.Deserialize(item.meta_value);
                        UserData usercollection = javaserializer.Deserialize<UserData>(!string.IsNullOrWhiteSpace(stylishservice?.ToString()) ? Convert.ToString(stylishservice).Replace("\";", "").Replace("];", "]") : item.meta_value);

                        if (usercollection.user_status == 0)
                        {
                            if (!string.IsNullOrEmpty(usercollection.guidimg))
                            {
                                string photo = Directory.GetFiles(UploadFile.UserPhotoPath, usercollection.guidimg.ToString() + "*").FirstOrDefault();
                                if (!string.IsNullOrWhiteSpace(photo))
                                    usercollection.photo = Helper.BaseUrl + $"Uploads/UserPhoto/" + Path.GetFileName(photo);
                            }
                            servicesbookingsList.Add(usercollection);
                        }

                    }
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = servicesbookingsList.OrderBy(y => y.first_name).ToList();
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "GetAllStylists");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        [Authorize]
        [HttpGet]
        public ResponseExtended<List<UserData>> GetAllStylists_WithPaging(int recordsPerPage, int pageNumber)
        {
            ResponseExtended<List<UserData>> resp = new ResponseExtended<List<UserData>>();
            var servicesbookingsList = new List<UserData>();
            var user = new List<wp_users>();
            try
            {
                var usermetadata = _userService.GetStylist();
                if (usermetadata != null)
                {
                    foreach (var item in usermetadata)
                    {
                        // var stylishservice = serialize.Deserialize(item.meta_value);
                        // UserData usercollection = javaserializer.Deserialize<UserData>(Convert.ToString(stylishservice));

                        var stylishservice = serialize.Deserialize(item.meta_value);
                        UserData usercollection = javaserializer.Deserialize<UserData>(!string.IsNullOrWhiteSpace(stylishservice?.ToString()) ? Convert.ToString(stylishservice).Replace("\";", "").Replace("];", "]") : item.meta_value);




                        if (usercollection.user_status == 0)
                        {
                            servicesbookingsList.Add(usercollection);
                        }

                    }
                    resp.ResponseCode = Response.Codes.OK;
                    var skipRecords = (pageNumber - 1) * recordsPerPage;
                    resp.ResponseData = servicesbookingsList.OrderBy(y => y.first_name).Skip(skipRecords).Take(recordsPerPage).ToList();
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "GetAllStylists");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        [Authorize]
        [HttpPost]
        public ResponseExtended<UserData> GetStylistsById(int userId)
        {
            ResponseExtended<UserData> resp = new ResponseExtended<UserData>();
            var servicesbookingsList = new List<UserData>();
            try
            {
                var usermetadata = _userService.GetStylist(userId);
                if (usermetadata != null)
                {

                    var stylishservice = serialize.Deserialize(usermetadata.meta_value);
                    UserData usercollection = javaserializer.Deserialize<UserData>(!string.IsNullOrWhiteSpace(stylishservice?.ToString()) ? Convert.ToString(stylishservice).Replace("\";", "").Replace("];", "]") : usermetadata.meta_value);


                    if (!string.IsNullOrEmpty(usercollection.guidimg))
                    {
                        string photo = Directory.GetFiles(UploadFile.UserPhotoPath, usercollection.guidimg.ToString() + "*").FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(photo))
                            usercollection.photo = Helper.BaseUrl + $"uploads/userphoto/" + Path.GetFileName(photo);
                        usercollection.isshowinApp = Convert.ToBoolean(usercollection.isshowinApp);
                    }
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = usercollection;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updatebookings");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        [Authorize]
        [HttpPost]
        public ResponseExtended<wp_glamly_servicesbookings> GetCustomerById(int userId)
        {
            ResponseExtended<wp_glamly_servicesbookings> resp = new ResponseExtended<wp_glamly_servicesbookings>();
            var servicesbookingsList = new List<wp_glamly_servicesbookings>();
            try
            {
                servicesbookingsList = _userService.GetBookingByUserId(userId);
                if (servicesbookingsList != null && servicesbookingsList.Count > 0)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = servicesbookingsList.OrderByDescending(y => y.id).FirstOrDefault();
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "GetCustomerById");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        #endregion

        #region "Services"


        //  [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, ExcludeQueryStringFromCacheKey = true)]
        [Authorize]
        [HttpGet]
        public ResponseExtended<List<wp_glamly_services>> GetServicesList()
        {
            var resp = new ResponseExtended<List<wp_glamly_services>>();
            var service = new List<wp_glamly_services>();
            try
            {
                service = _userService.GetServiceList();

                if (service != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = service;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "services/{id}");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }
        /// <summary>
        /// Get all services of stylists
        /// </summary>
        /// <returns></returns>
        // [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, ExcludeQueryStringFromCacheKey = true)]
        [Authorize]
        [HttpGet]
        public ResponseExtended<List<UserService>> GetServices()
        {

            var resp = new ResponseExtended<List<UserService>>();
            List<wp_glamly_services> servicesList = new List<wp_glamly_services>();
            List<wp_glamly_servicestypes> serviceTypeList = new List<wp_glamly_servicestypes>();
            List<UserService> serviceModelList = new List<UserService>();
            try
            {
                servicesList = _userService.GetServices();
                if (servicesList != null)
                {
                    foreach (var item in servicesList)
                    {
                        UserService serviceModel = new UserService();
                        serviceTypeList = _userService.GetTypesByServiceId(item.id);
                        List<Dictionary<string, string>> typeList = new List<Dictionary<string, string>>();
                        foreach (var type in serviceTypeList)
                        {
                            Dictionary<string, string> typeObject = new Dictionary<string, string>();
                            typeObject.Add("id", Convert.ToString(type.id));
                            typeObject.Add("typeName", type.typename);
                            typeObject.Add("price", Convert.ToString(type.price));
                            typeList.Add(typeObject);
                        }
                        serviceModel.id = item.id;
                        serviceModel.servicename = item.servicename;
                        serviceModel.status = item.status;
                        serviceModel.service_image = item.service_image;
                        serviceModel.service_type = typeList;
                        serviceModelList.Add(serviceModel);
                    }
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = serviceModelList;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                }

                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "services");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// Get service by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpGet]
        public ResponseExtended<wp_glamly_services> GetServiceById(int id)
        {
            var resp = new ResponseExtended<wp_glamly_services>();
            var service = new wp_glamly_services();
            try
            {
                service = _userService.GetServicesById(id);

                if (service != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = service;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "services/{id}");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }


        /// <summary>
        /// Get servives by type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ResponseExtended<wp_glamly_servicestypes> GetServiceTypeById(int id)
        {
            var resp = new ResponseExtended<wp_glamly_servicestypes>();
            var serviceType = new wp_glamly_servicestypes();
            try
            {
                serviceType = _userService.GetServiceTypeById(id);
                if (serviceType != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = serviceType;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "services/{id}/types");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// Get types by service id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ResponseExtended<List<wp_glamly_servicestypes>> GetTypesByServiceId(int id)
        {
            var resp = new ResponseExtended<List<wp_glamly_servicestypes>>();
            var serviceType = new List<wp_glamly_servicestypes>();
            try
            {
                serviceType = _userService.GetTypesByServiceId(id);
                if (serviceType != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = serviceType;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "services/{id}/types");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        [Authorize]
        [HttpGet]
        public ResponseExtended<List<StylistService>> GetstylistByService()
        {
            ResponseExtended<List<StylistService>> resp = new ResponseExtended<List<StylistService>>();
            Services objservice = new Services();
            List<StylistService> stylistList = new List<StylistService>();
            try
            {
                var service = new List<wp_glamly_services>();
                service = _userService.GetServiceList();
                var usermetadata = _userService.GetStylist();

                if (usermetadata != null)
                {
                    foreach (var item in service)
                    {
                        StylistService keyvalue = new StylistService();
                        List<Stylist> objstylist = new List<Stylist>();
                        foreach (var item1 in usermetadata)
                        {
                            //  var stylishservice = serialize.Deserialize(item1.meta_value);
                            //  UserData usercollection = javaserializer.Deserialize<UserData>(Convert.ToString(stylishservice));

                            var stylishservice = serialize.Deserialize(item1.meta_value);
                            UserData usercollection = javaserializer.Deserialize<UserData>(!string.IsNullOrWhiteSpace(stylishservice?.ToString()) ? Convert.ToString(stylishservice).Replace("\";", "").Replace("];", "]") : item1.meta_value);


                            if (usercollection.services != null)
                            {
                                if (usercollection.services.Contains(item.servicename))
                                {

                                    Stylist obj = new Stylist();
                                    obj.StylistID = usercollection.ID;
                                    obj.StylistName = usercollection.first_name + " " + usercollection.last_name;
                                    objstylist.Add(obj);
                                }
                            }
                        }
                        Dictionary<string, string> objname = new Dictionary<string, string>();
                        keyvalue.Id = item.id.ToString();
                        keyvalue.Name = item.servicename;
                        keyvalue.stylists = objstylist;
                        stylistList.Add(keyvalue);
                    }
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = stylistList;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updatebookings");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }
        //[Authorize]
        //[HttpGet]
        //public ResponseExtended<List<StylistService>> GetstylistByService()
        //{
        //    ResponseExtended<List<StylistService>> resp = new ResponseExtended<List<StylistService>>();
        //    Services objservice = new Services();
        //    List<StylistService> stylistList = new List<StylistService>();
        //    try
        //    {
        //        ///////////////////start new logic
        //        var stylistcalendarList = new List<wp_glamly_stylistschedules>();
        //        string stylistsIds = string.Empty;

        //        var service = new List<wp_glamly_services>();
        //        service = _userService.GetServiceList();
        //        var usermetadata = _userService.GetStylist();

        //        if (usermetadata != null)
        //        {
        //            foreach (var item in service)
        //            {
        //                StylistService keyvalue = new StylistService();
        //                List<Stylist> objstylist = new List<Stylist>();
        //                foreach (var item1 in usermetadata)
        //                {
        //                    var stylishservice = serialize.Deserialize(item1.meta_value);
        //                    UserData usercollection = javaserializer.Deserialize<UserData>(Convert.ToString(stylishservice));
        //                    if (usercollection.services != null)
        //                    {
        //                        if (usercollection.services.Contains(item.servicename))
        //                        {
        //                            stylistcalendarList = _userService.Getstylistcalendar();

        //                            foreach (var itemstylistid in stylistcalendarList)
        //                            {
        //                                stylistsIds += itemstylistid.stylistId + ",";
        //                            }
        //                            stylistsIds = stylistsIds.TrimEnd(',');

        //                            if (stylistsIds.Contains(Convert.ToString(usercollection.ID)))
        //                            {
        //                                Stylist obj = new Stylist();
        //                                obj.StylistID = usercollection.ID;
        //                                obj.StylistName = usercollection.first_name + " " + usercollection.last_name;
        //                                objstylist.Add(obj);
        //                            }
        //                        }
        //                    }
        //                }
        //                Dictionary<string, string> objname = new Dictionary<string, string>();
        //                keyvalue.Id = item.id.ToString();
        //                keyvalue.Name = item.servicename;
        //                keyvalue.stylists = objstylist;
        //                stylistList.Add(keyvalue);
        //            }
        //            resp.ResponseCode = Response.Codes.OK;
        //            resp.ResponseData = stylistList;
        //        }
        //        else
        //        {
        //            resp.ResponseCode = Response.Codes.NotFound;
        //        }
        //        return resp;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.MailError(ex, "Glamly API Ver 1.0");
        //        Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updatebookings");
        //        resp.ResponseCode = Response.Codes.InternalServerError;
        //        return resp;
        //    }
        //}

        [Authorize]
        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> UpdateService(Services service)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var typename = _userService.GetServiceTypes().Where(s => s.id == service.Id);
                bool checkservicetype = _userService.GetServiceTypes().Exists(x => x.typename == typename.FirstOrDefault().typename);
                int result = 0;
                if (checkservicetype == true)
                {
                    wp_glamly_servicestypes obj = new wp_glamly_servicestypes();
                    obj.typename = service.typeName;
                    obj.price = service.Price;
                    obj.id = service.Id;
                    obj.serviceid = typename.FirstOrDefault().serviceid;
                    obj.status = typename.FirstOrDefault().status;
                    result = _userService.updateservicetypes(obj);
                    if (result > 0)
                        resp.ResponseCode = Response.Codes.OK;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "UpdateService");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }
        [Authorize]
        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> AddServiceType(string ServiceType, int Price, string ServiceName)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                bool checkservicetype = _userService.GetServiceTypes().Exists(x => x.typename == ServiceType && x.status == 1);
                int result = 0;
                if (checkservicetype == false)
                {
                    wp_glamly_servicestypes obj = new wp_glamly_servicestypes();
                    obj.typename = ServiceType;
                    obj.price = Price;
                    obj.status = 1;
                    obj.serviceid = _userService.GetIdByName(ServiceName);
                    result = _userService.SaveServicetype(obj);
                    if (result > 0)
                        resp.ResponseCode = Response.Codes.OK;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.Exists;
                }
                return resp;
            }
            catch (Exception ex)
            {

                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "AddServiceType");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }
        [Authorize]
        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> AddService(string ServiceName)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                bool checkservicetype = _userService.GetServiceList().Exists(x => x.servicename == ServiceName);
                int result = 0;
                if (checkservicetype == false)
                {
                    wp_glamly_services obj = new wp_glamly_services();
                    obj.servicename = ServiceName;
                    obj.status = 1;
                    result = _userService.SaveService(obj);
                    if (result > 0)
                        resp.ResponseCode = Response.Codes.OK;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.Exists;
                }
                return resp;
            }
            catch (Exception ex)
            {

                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "AddServiceType");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }
        [Authorize]
        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> DeleteService(int id)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var isdelete = _userService.DeleteServiceType(id);
                if (isdelete)
                {
                    resp.ResponseCode = Response.Codes.OK;
                }

                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "DeleteServiceType");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        #endregion

        #region "Bookings"
        /// <summary>
        /// Get all bookings of users
        /// </summary>
        /// <returns></returns>
        // [CacheOutput(ClientTimeSpan = 10, ServerTimeSpan = 10, ExcludeQueryStringFromCacheKey = true)]
        [Authorize]
        [HttpGet]
        public ResponseExtended<List<Booking>> GetBookings()
        {

            //Unable to cast object of type 'System.String' to type 'System.Collections.IList
            var resp = new ResponseExtended<List<Booking>>();
            var servicesbookingsList = new List<wp_glamly_servicesbookings>();

            string userJsonObject = string.Empty;
            ServiceWithType obj = new ServiceWithType();
            List<ServiceWithType> serlist = new List<ServiceWithType>();
            var BookingList = new List<Booking>();
            try
            {
                servicesbookingsList = _userService.GetBookings();
                if (servicesbookingsList != null)
                {

                    foreach (var item in servicesbookingsList)
                    {
                        if (!string.IsNullOrEmpty(item.servicewithtypes))
                        {
                            Booking book = new Booking();
                            string servicetypes = item.servicewithtypes;
                            string[] values = servicetypes.Split(':');

                            var desearlize = serialize.Deserialize(item.servicewithtypes);
                            //List<ServiceWithType> jsonservicelist = javaserializer.Deserialize<List<ServiceWithType>>(Convert.ToString(item.servicewithtypes));
                            List<ServiceWithType> jsonservicelist = javaserializer.Deserialize<List<ServiceWithType>>(!string.IsNullOrWhiteSpace(desearlize?.ToString()) ? Convert.ToString(desearlize).Replace("\";", "").Replace("];", "]") : item.servicewithtypes);

                            book.Id = item.id;
                            book.userid = (int)item.userid;
                            book.address = item.address == "null" ? string.Empty : item.address;
                            book.altdatetime = !string.IsNullOrEmpty(item.altdatetime) ? item.altdatetime : string.Empty;
                            book.service = item.service == "null" ? string.Empty : item.service;
                            book.type = !string.IsNullOrEmpty(item.type) ? item.type : string.Empty;
                            book.billingaddress = !string.IsNullOrEmpty(item.billingaddress) ? item.billingaddress : string.Empty;
                            book.bookingid = item.bookingid == "null" ? string.Empty : item.bookingid;
                            book.stylistid = (int)item.stylistid;
                            book.servicewithtypes = jsonservicelist;
                            book.city = item.city == "null" ? string.Empty : item.city;
                            //datetime.ToString("yyyy/MM/dd")

                            book.datetime = !string.IsNullOrEmpty(Convert.ToDateTime(item.datetime).ToString("yyyy/MM/dd HH:mm")) ? Convert.ToDateTime(item.datetime).ToString("yyyy/MM/dd HH:mm") : string.Empty;
                            book.email = item.email == "null" ? string.Empty : item.email;
                            book.firstname = item.firstname == "null" ? string.Empty : item.firstname;
                            book.surname = item.surname == "null" ? string.Empty : item.surname;
                            book.isedit = item.isedit == "null" ? string.Empty : item.isedit;
                            book.zipcode = item.zipcode == "null" ? string.Empty : item.zipcode;
                            book.phone = item.phone == "null" ? string.Empty : item.phone;
                            book.newsletter = item.newsletter == "null" ? string.Empty : item.newsletter;
                            book.message = item.message == "null" ? string.Empty : item.message;
                            book.personal = item.personal == "null" ? string.Empty : item.personal;
                            book.billingaddress = item.billingaddress == "null" ? string.Empty : item.billingaddress;
                            book.status = item.status == "null" ? string.Empty : item.status;
                            book.isdeleted = item.isdeleted == "null" ? string.Empty : item.isdeleted;
                            book.comments = item.comments == "null" ? string.Empty : item.comments;  //!string.IsNullOrEmpty(item.comments) ? item.comments : string.Empty;
                            book.otherservices = item.otherservices == "null" ? string.Empty : item.otherservices;
                            book.workflowstatus = (int)item.workflowstatus;
                            BookingList.Add(book);
                        }

                    }
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = BookingList.ToList();
                    //  resp.ResponseData = BookingList.OrderBy(y => y.datetime).ToList(); 
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "services/{id}/types");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// Save the bookings 
        /// </summary>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> SaveBookings(Booking bookings)
        {
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            string jsonservicelist = string.Empty;
            string jsonservices = string.Empty;
            string jsontypes = string.Empty;
            string jsongpaymentlist = string.Empty;

            try
            {
                if (bookings != null)
                {
                    jsonservices = serialize.Serialize(bookings.service);
                    jsontypes = serialize.Serialize(bookings.type);
                    jsonservicelist = JsonConvert.SerializeObject(bookings.servicewithtypes);
                    wp_glamly_servicesbookings booking = new wp_glamly_servicesbookings();
                    booking.address = !string.IsNullOrEmpty(bookings.address) ? bookings.address : string.Empty;
                    booking.altdatetime = !string.IsNullOrEmpty(bookings.altdatetime) ? bookings.altdatetime : string.Empty;
                    booking.service = !string.IsNullOrEmpty(jsonservices) ? jsonservices : string.Empty;
                    booking.type = !string.IsNullOrEmpty(jsontypes) ? jsontypes : string.Empty;
                    booking.billingaddress = !string.IsNullOrEmpty(bookings.billingaddress) ? bookings.billingaddress : string.Empty;
                    booking.bookingid = Helper.RandomString(9);
                    booking.servicewithtypes = !string.IsNullOrEmpty(jsonservicelist) ? jsonservicelist : string.Empty;
                    booking.city = !string.IsNullOrEmpty(bookings.city) ? bookings.city : string.Empty;
                    booking.datetime = !string.IsNullOrEmpty(bookings.datetime) ? bookings.datetime : string.Empty;
                    booking.email = !string.IsNullOrEmpty(bookings.email) ? bookings.email : string.Empty;
                    booking.firstname = !string.IsNullOrEmpty(bookings.firstname) ? bookings.firstname : string.Empty;
                    booking.surname = !string.IsNullOrEmpty(bookings.surname) ? bookings.surname : string.Empty;
                    booking.isedit = !string.IsNullOrEmpty(bookings.isedit) ? bookings.isedit : string.Empty;
                    booking.zipcode = !string.IsNullOrEmpty(bookings.zipcode) ? bookings.zipcode : string.Empty;
                    booking.phone = !string.IsNullOrEmpty(bookings.phone) ? bookings.datetime : string.Empty;
                    booking.newsletter = !string.IsNullOrEmpty(bookings.newsletter) ? bookings.datetime : string.Empty;
                    booking.message = !string.IsNullOrEmpty(bookings.message) ? bookings.datetime : string.Empty;
                    booking.message = !string.IsNullOrEmpty(bookings.status) ? bookings.datetime : string.Empty;
                    booking.personal = !string.IsNullOrEmpty(bookings.personal) ? bookings.datetime : string.Empty;
                    //  booking.billingaddress = !string.IsNullOrEmpty(bookings.billingaddress) ? bookings.datetime : string.Empty;
                    booking.status = !string.IsNullOrEmpty(bookings.status) ? bookings.datetime : string.Empty;
                    booking.userid = bookings.userid > 0 ? bookings.userid : 0;
                    string bookingid = _userService.savebookingdata(booking);
                    wp_glamly_payment payment = new wp_glamly_payment();
                    payment.acquirer = !string.IsNullOrEmpty(bookings.payment.acquirer) ? bookings.payment.acquirer : string.Empty;
                    payment.amount = !string.IsNullOrEmpty(bookings.payment.amount) ? bookings.payment.amount : string.Empty;
                    payment.approvalcode = !string.IsNullOrEmpty(bookings.payment.approvalcode) ? bookings.payment.approvalcode : string.Empty;
                    payment.bookingid = !string.IsNullOrEmpty(bookingid) ? bookingid : string.Empty;
                    payment.calcfee = !string.IsNullOrEmpty(bookings.payment.calcfee) ? bookings.payment.calcfee : string.Empty;
                    payment.cardexpdate = !string.IsNullOrEmpty(bookings.payment.cardexpdate) ? bookings.payment.cardexpdate : string.Empty;
                    payment.cardnomask = !string.IsNullOrEmpty(bookings.payment.cardnomask) ? bookings.payment.cardnomask : string.Empty;
                    payment.cardprefix = !string.IsNullOrEmpty(bookings.payment.cardprefix) ? bookings.payment.cardprefix : string.Empty;
                    payment.cardtype = !string.IsNullOrEmpty(bookings.payment.cardtype) ? bookings.payment.cardtype : string.Empty;
                    payment.currency = !string.IsNullOrEmpty(bookings.payment.currency) ? bookings.payment.currency : string.Empty;
                    payment.dibsInternalIdentifier = !string.IsNullOrEmpty(bookings.payment.dibsInternalIdentifier) ? bookings.payment.dibsInternalIdentifier : string.Empty;
                    payment.fee = Convert.ToString(bookings.payment.fee);
                    payment.fullreply = !string.IsNullOrEmpty(bookings.payment.fullreply) ? bookings.payment.fullreply : string.Empty;
                    payment.lang = !string.IsNullOrEmpty(bookings.payment.lang) ? bookings.payment.lang : string.Empty;
                    payment.merchant = !string.IsNullOrEmpty(bookings.payment.merchant) ? bookings.payment.merchant : string.Empty;
                    payment.merchantid = Convert.ToString(bookings.payment.merchantid);
                    payment.method = !string.IsNullOrEmpty(bookings.payment.method) ? bookings.payment.method : string.Empty;
                    payment.mobilelib = !string.IsNullOrEmpty(bookings.payment.mobilelib) ? bookings.payment.mobilelib : string.Empty;
                    payment.orderid = !string.IsNullOrEmpty(bookings.payment.orderid) ? bookings.payment.orderid : string.Empty;
                    payment.paytype = !string.IsNullOrEmpty(bookings.payment.paytype) ? bookings.payment.paytype : string.Empty;
                    payment.platform = !string.IsNullOrEmpty(bookings.payment.platform) ? bookings.payment.platform : string.Empty;
                    payment.status = !string.IsNullOrEmpty(bookings.payment.status) ? bookings.payment.status : string.Empty;
                    payment.test = !string.IsNullOrEmpty(bookings.payment.test) ? bookings.payment.test : string.Empty;
                    payment.textreply = !string.IsNullOrEmpty(bookings.payment.textreply) ? bookings.payment.textreply : string.Empty;
                    payment.theme = !string.IsNullOrEmpty(bookings.payment.theme) ? bookings.payment.theme : string.Empty;
                    payment.timeout = !string.IsNullOrEmpty(bookings.payment.timeout) ? bookings.payment.timeout : string.Empty;
                    payment.transact = !string.IsNullOrEmpty(bookings.payment.transact) ? bookings.payment.transact : string.Empty;
                    payment.userid = bookings.payment.userid > 0 ? bookings.payment.userid : 0;
                    payment.version = !string.IsNullOrEmpty(bookings.payment.version) ? bookings.payment.version : string.Empty;
                    payment.servicewithtypes = !string.IsNullOrEmpty(jsonservicelist) ? jsonservicelist : string.Empty;
                    payment.paymentdate = !string.IsNullOrEmpty(bookings.datetime) ? bookings.datetime : string.Empty;
                    var paymentid = _userService.savepaymentdata(payment);

                    if (paymentid > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseMessage = "Booking has been successfully saved";
                    }
                    else
                    {
                        resp.ResponseMessage = "Provided parameters are incorrect";
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }

                else
                {
                    resp.ResponseMessage = "Provided parameters are incorrect";
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "services/{id}/types");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        /// <summary>
        /// Update the booking by bookingid
        /// </summary>
        /// <param name="bookings"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        public ResponseExtended<System.Web.Mvc.JsonResult> UpdateBookings(Booking bookings)
        {
            string jsonservicelist = string.Empty;
            string jsonservices = string.Empty;
            string jsontypes = string.Empty;
            string jsongpaymentlist = string.Empty;
            jsonservices = serialize.Serialize(bookings.service);
            jsontypes = serialize.Serialize(bookings.type);
            jsonservicelist = JsonConvert.SerializeObject(bookings.servicewithtypes);
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            wp_glamly_servicesbookings bookingdetail = _userService.GetBookingById(Convert.ToInt32(bookings.bookingid));

            try
            {

                if (bookingdetail != null)
                {
                    bookingdetail.address = bookings.address;
                    bookingdetail.altdatetime = bookings.altdatetime;
                    bookingdetail.service = jsonservices;
                    bookingdetail.type = jsontypes;
                    bookingdetail.billingaddress = bookings.billingaddress;
                    bookingdetail.bookingid = Helper.RandomString(9); ;
                    bookingdetail.servicewithtypes = jsonservicelist;
                    bookingdetail.city = bookings.city;
                    bookingdetail.datetime = bookings.datetime;
                    bookingdetail.email = bookings.email;
                    bookingdetail.firstname = bookings.firstname;
                    bookingdetail.surname = bookings.surname;
                    bookingdetail.isedit = "true";
                    bookingdetail.zipcode = bookings.zipcode;
                    bookingdetail.phone = bookings.phone;
                    bookingdetail.newsletter = bookings.newsletter;
                    bookingdetail.message = bookings.message;
                    bookingdetail.message = bookings.status;
                    bookingdetail.personal = bookings.personal;
                    bookingdetail.billingaddress = bookings.billingaddress;
                    bookingdetail.status = bookings.status;
                    bookingdetail.otherservices = bookingdetail.otherservices;
                    var bookingid = _userService.updatebookingdata(bookingdetail);
                    if (bookingid > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseMessage = "Booking has been updated successfully";
                    }
                    else
                    {
                        resp.ResponseMessage = "Provided parameters are incorrect";
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }
                else
                {
                    resp.ResponseMessage = "Provided parameters are incorrect";
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updatebookings");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// Get all approved bookings of users
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ResponseExtended<List<wp_glamly_servicesbookings>> GetApprovedBookings(string status)
        {

            var resp = new ResponseExtended<List<wp_glamly_servicesbookings>>();
            var servicesbookingsList = new List<wp_glamly_servicesbookings>();
            try
            {
                servicesbookingsList = _userService.GetBookingByStatus(status);

                if (servicesbookingsList != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = servicesbookingsList;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updatebookings");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// Get booking by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpGet]
        public ResponseExtended<wp_glamly_servicesbookings> GetBookingById(int id)
        {
            var resp = new ResponseExtended<wp_glamly_servicesbookings>();
            var bookingsList = new wp_glamly_servicesbookings();
            try
            {
                bookingsList = _userService.GetBookingById(id);
                if (bookingsList != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = bookingsList;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "bookings");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// Get booking by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpGet]
        public ResponseExtended<List<wp_glamly_servicesbookings>> GetBookingByUserId(int id)
        {
            var resp = new ResponseExtended<List<wp_glamly_servicesbookings>>();
            var bookingsList = new List<wp_glamly_servicesbookings>();
            try
            {
                bookingsList = _userService.GetBookingByUserId(id);
                if (bookingsList != null)
                {

                    foreach (var item in bookingsList)
                    {
                        if (!string.IsNullOrEmpty(item.servicewithtypes))
                        {
                            var desearlize = item.servicewithtypes;
                            item.servicewithtypes = (string)desearlize;
                        }

                    }

                    if (bookingsList.Count > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseData = bookingsList;
                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.NotFound;
                    }
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updatebookings");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// delete booking by Id
        /// </summary>
        /// <param name="bookingid"></param>
        /// <returns></returns>
        /// 
        //  [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, ExcludeQueryStringFromCacheKey = true)]
        [Authorize]
        [HttpDelete]
        public ResponseExtended<System.Web.Mvc.JsonResult> DeleteBooking(string id)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var isdelete = _userService.DeleteBooking(id);
                if (isdelete)
                {
                    resp.ResponseCode = Response.Codes.OK;
                }

                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "deletebooking");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        [Authorize]
        [HttpGet]
        public ResponseExtended<List<CalendarService>> GetAvailDatesbyStylist()
        {
            //Unable to cast object of type 'System.String' to type 'System.Collections.IList
            var resp = new ResponseExtended<List<CalendarService>>();
            var stylistcalendarList = new List<wp_glamly_stylistschedules>();


            //   CalendarService obj = new CalendarService();
            List<CalendarService> serlist = new List<CalendarService>();
            var BookingList = new List<Booking>();
            try
            {
                stylistcalendarList = _userService.Getstylistcalendar();


                foreach (var item in stylistcalendarList)
                {

                    CalendarService obj = new CalendarService();
                    if (!string.IsNullOrEmpty(item.date))
                    {

                      //  var desearlize = serialize.Deserialize(item.date);
                      //  List<AvailableDates> jsonservicelist = javaserializer.Deserialize<List<AvailableDates>>(Convert.ToString(desearlize));

                        var desearlize = serialize.Deserialize(item.date);
                        List<AvailableDates> jsonservicelist = javaserializer.Deserialize<List<AvailableDates>>(!string.IsNullOrWhiteSpace(desearlize?.ToString()) ? Convert.ToString(desearlize).Replace("\";", "").Replace("];", "]") : item.date);


                        obj.StylistId = item.stylistId;
                        obj.UserName = item.name;
                        obj.date = jsonservicelist;
                        serlist.Add(obj);
                    }

                }

                if (stylistcalendarList != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = serlist.ToList();
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;

            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "GetAvailDatesbyStylist");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        public ResponseExtended<int> GetStylistAssignedBookingCount(int stylistid)
        {
            var resp = new ResponseExtended<int>();
            var Declinebookings = new List<wp_glamly_servicesbookings>();
            int stylistbookingcount = 0;
            try
            {
                stylistbookingcount = _userService.GetBookingBystylistid(stylistid);

                if (stylistbookingcount > 0)
                {
                    resp.ResponseData = stylistbookingcount;
                    resp.ResponseCode = Response.Codes.OK;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "declineBookingByProUser/{BookingId}/{stylistid}");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        [Authorize]
        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> ApprovedBooking(string bookingid, int stylistid)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            var respobj = new ResponseExtended<int>();
            string service = string.Empty;
            string types = string.Empty;
            wp_usermeta stylistdetails = new wp_usermeta();
            try
            {
                var isdelete = _userService.ApprovedBookingByAdmin(bookingid, stylistid);
                respobj = GetStylistAssignedBookingCount(stylistid);
                int bookingcount = respobj.ResponseData;
                if (isdelete)
                {
                    var bookings = _userService.GetBookingByBookingId(bookingid);
                    if (bookings != null)
                    {
                        stylistdetails = _userService.GetUserDetailById(stylistid);
                        if (stylistdetails != null)
                        {
                            if (!string.IsNullOrEmpty(bookings.servicewithtypes))
                            {
                                var desearlize = serialize.Deserialize(bookings.servicewithtypes);
                                List<ServiceWithType> jsonservicelist = javaserializer.Deserialize<List<ServiceWithType>>(!string.IsNullOrWhiteSpace(desearlize?.ToString()) ? Convert.ToString(desearlize).Replace("\";", "").Replace("];", "]") : bookings.servicewithtypes);


                                foreach (var item in jsonservicelist)
                                {
                                    service += item.servicename + ",";
                                    types += item.typename + ",";
                                }
                            }
                            var desearlize1 = serialize.Deserialize(stylistdetails.meta_value);
                            UserData usercollection = javaserializer.Deserialize<UserData>(!string.IsNullOrWhiteSpace(desearlize1?.ToString()) ? Convert.ToString(desearlize1).Replace("\";", "").Replace("];", "]") : stylistdetails.meta_value);

                            emailHelper.SendEmailWithTemplatedAssignedtoStylist(usercollection.first_name, usercollection.user_email, bookings.firstname, bookings.surname, Convert.ToDateTime(bookings.datetime), bookings.email, bookings.address, bookings.city, bookings.comments, bookings.zipcode, service.TrimEnd(','), types.TrimEnd(','), bookings.phone);
                        }
                    }

                    resp.ResponseCode = Response.Codes.OK;
                    //   notificationId1 = helpers.SendPushNotificationSchedule(bookingdetail.userid.Value, "Du har en bokning imorgon", helpers.ConvertToUtcDateTime(bookingdetail.datetime).AddDays(-1));
                    //  helpers.SendPushNotificationSchedule(stylistid, "Du har fått en ny förfrågan", DateTime.UtcNow);
                    helpers.SendPushNotificationwithbadge(stylistid, "Du har fått en ny förfrågan", bookingcount);
                }

                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "ApprovedBooking");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }
        [Authorize]
        [HttpGet]
        public ResponseExtended<string> GetstylistById(int Stylistid)
        {
            var resp = new ResponseExtended<string>();
            var stylistdetail = new wp_usermeta();
            stylistdetail = _userService.GetStylist(Stylistid);
            string stylistname = string.Empty;
            var desearlize = serialize.Deserialize(stylistdetail.meta_value);
            UserData usercollection = javaserializer.Deserialize<UserData>(!string.IsNullOrWhiteSpace(desearlize?.ToString()) ? Convert.ToString(desearlize).Replace("\";", "").Replace("];", "]") : stylistdetail.meta_value);

            if (usercollection != null)
            {
                stylistname = usercollection.first_name;
                resp.ResponseCode = Response.Codes.OK;
                resp.ResponseData = stylistname;
            }
            else
            {
                resp.ResponseCode = Response.Codes.NotFound;
            }
            return resp;

        }

        #endregion

        #region "Payment"
        /// <summary>
        /// Get payment recipt by userid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpGet]
        public ResponseExtended<List<wp_glamly_payment>> GetPaymentById(int id)
        {

            var resp = new ResponseExtended<List<wp_glamly_payment>>();
            var paymentList = new List<wp_glamly_payment>();
            try
            {
                paymentList = _userService.GetPaymentByUserId(id);
                if (paymentList != null)
                {

                    foreach (var item in paymentList)
                    {
                        if (!string.IsNullOrEmpty(item.servicewithtypes))
                        {
                            var desearlize = item.servicewithtypes;
                            item.servicewithtypes = (string)desearlize;
                        }
                    }

                    if (paymentList.Count > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseData = paymentList;
                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.NotFound;
                    }
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "GetPaymentById");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// Get all services of stylists
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ResponseExtended<List<wp_glamly_payment>> GetPaymentList()
        {
            var resp = new ResponseExtended<List<wp_glamly_payment>>();
            var paymentList = new List<wp_glamly_payment>();
            try
            {
                paymentList = _userService.GetPaymentList();
                if (paymentList != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = paymentList;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "GetPaymentList");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }





        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> AddStylistPage(StylistPageData stylistdata)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            wp_glamly_stylistTemp tempdata = new wp_glamly_stylistTemp();
            tempdata.name = stylistdata.name;
            tempdata.skill1 = stylistdata.skillone;
            tempdata.skill2 = stylistdata.skillsecond;
            tempdata.skill3 = stylistdata.skillthird;
            tempdata.profileimageguid = stylistdata.profileimageguid;
            tempdata.isdeleted = "false";

            int stylistid = _userService.SaveStylistPage(tempdata);

            if (stylistid > 0)
            {
                resp.ResponseCode = Response.Codes.OK;
            }
            else
            {
                resp.ResponseCode = Response.Codes.InvalidRequest;
            }
            return resp;

        }

        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> RemoveStylistPage(StylistPageData stylistdata)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            var Isdeleted = _userService.DeleteStylistPage(stylistdata.id);

            if (Isdeleted)
            {
                resp.ResponseCode = Response.Codes.OK;
            }
            else
            {
                resp.ResponseCode = Response.Codes.InvalidRequest;
            }
            return resp;

        }


        /// <summary>
        /// Update the booking by bookingid
        /// </summary>
        /// <param name="bookings"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> UpdateStylist(UserData LoginModel)
        {
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var usermetadata = _userService.GetUserMetadatakeybyId(LoginModel.ID);
                if (usermetadata != null)
                {
                    var desearlize = serialize.Deserialize(usermetadata.meta_value);
                    UserData usercollection = javaserializer.Deserialize<UserData>(!string.IsNullOrWhiteSpace(desearlize?.ToString()) ? Convert.ToString(desearlize).Replace("\";", "").Replace("];", "]") : usermetadata.meta_value);


                    if (usercollection != null)
                    {
                        usercollection.first_name = LoginModel.first_name == "null" ? string.Empty : LoginModel.first_name;
                        usercollection.last_name = LoginModel.last_name == "null" ? string.Empty : LoginModel.last_name;
                        usercollection.mobile = LoginModel.mobile == "null" ? string.Empty : LoginModel.mobile;
                        usercollection.user_email = !string.IsNullOrEmpty(LoginModel.user_email) ? LoginModel.user_email : string.Empty;
                        usercollection.services = !string.IsNullOrEmpty(LoginModel.services) ? LoginModel.services : string.Empty;
                        usercollection.address = !string.IsNullOrEmpty(LoginModel.address) ? LoginModel.address : string.Empty;
                        usercollection.city = !string.IsNullOrEmpty(LoginModel.city) ? LoginModel.city : string.Empty;
                        usercollection.zipcode = !string.IsNullOrEmpty(LoginModel.zipcode) ? LoginModel.zipcode : string.Empty;
                        usercollection.offer = LoginModel.offer;
                        usercollection.comments = !string.IsNullOrEmpty(LoginModel.comments) ? LoginModel.comments : string.Empty;
                        usercollection.upcomingbookings = LoginModel.upcomingbookings;
                        usercollection.notificationall = LoginModel.notificationall;
                        usercollection.guidimg = LoginModel.guidimg;
                        usercollection.isshowinApp = LoginModel.isshowinApp;
                        var jsonupdate = JsonConvert.SerializeObject(usercollection);
                        usermetadata.meta_value = jsonupdate;
                        int userid = _userService.updateuserdata(usermetadata);

                        if (userid > 0)
                        {
                            resp.ResponseCode = Response.Codes.OK;
                            resp.ResponseMessage = "User has been updated successfully";
                        }
                        else
                        {
                            resp.ResponseCode = Response.Codes.InvalidRequest;
                        }

                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }

                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "UpdateStylist");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }




        [Authorize]
        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> DeleteStylist(int stylistid)
        {
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var bookinglist = _userService.GetBookings();
                bool isexist = bookinglist.Exists(x => x.stylistid == stylistid);
                if (isexist)
                {
                    resp.ResponseCode = Response.Codes.FailedDeleteStylist;
                    return resp;
                }
                else
                {


                    wp_glamly_stylistschedules schudule = new wp_glamly_stylistschedules();

                    schudule = _userService.GetstylistcalendarByStylistId(stylistid);

                    if(schudule != null)
                    {
                        schudule.id = schudule.id;                        
                        schudule.stylistId = stylistid;
                        schudule.name = schudule.name;
                        schudule.isdeleted = "true";
                        schudule.date = schudule.date;
                        _userService.updateStylistSchedule(schudule);
                    }                  
                    var usermetadata = _userService.GetUserMetadatakeybyId(stylistid);
                    if (usermetadata != null)
                    {
                        var desearlize = serialize.Deserialize(usermetadata.meta_value);
                        UserData usercollection = javaserializer.Deserialize<UserData>(!string.IsNullOrWhiteSpace(desearlize?.ToString()) ? Convert.ToString(desearlize).Replace("\";", "").Replace("];", "]") : usermetadata.meta_value);


                        if (usercollection != null)
                        {
                            usercollection.user_status = 1;
                            var jsonupdate = JsonConvert.SerializeObject(usercollection);
                            usermetadata.meta_value = jsonupdate;
                            int userid = _userService.updateuserdata(usermetadata);
                        
                            if (userid > 0)
                            {
                                resp.ResponseCode = Response.Codes.OK;
                            }
                            else
                            {
                                resp.ResponseCode = Response.Codes.InvalidRequest;
                            }
                        }
                        else
                        {
                            resp.ResponseCode = Response.Codes.InvalidRequest;
                        }
                    }

                    return resp;
                }




            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "DeleteStylist");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        #endregion

        #region "Forget Password"

        [AllowAnonymous]
        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> ResetPassword(ChangePassword userData)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            //  var usercollection = new UserData();
            string usertype = string.Empty;
            string password = string.Empty;
            UserData userToValidate = new UserData { ID = userData.Id, user_email = userData.UserEmail };
            var userdetail = _userService.GetUserByEmailId(userToValidate.user_email);
            if (userdetail == null)
            {
                resp.ResponseCode = Response.Codes.InvalidRequest;
                return resp;
            }

            else
            {
                string userSalt = Helper.getPasswordSalt();
                string newPassword = Hashing.MD5Hash(userData.NewPassword.Trim(), userSalt);
                userdetail.user_pass = newPassword;
                userdetail.user_activation_key = userSalt;
                wp_users obj = new wp_users();
                obj.ID = userdetail.ID;
                obj.user_email = userdetail.user_email;
                obj.user_login = "";
                obj.user_nicename = "";
                obj.user_pass = newPassword;
                obj.user_registered = DateTime.MinValue;
                obj.user_status = 1;
                obj.user_url = "";
                obj.user_activation_key = userSalt;
                obj.display_name = "";

                int user_id = _userService.updateuserpassword(obj);
                var usermetadata = _userService.GetUserMetadatakeybyId(user_id);

                if (usermetadata != null)
                {

                    var desearlize = serialize.Deserialize(usermetadata.meta_value);
                    UserData usercollection = javaserializer.Deserialize<UserData>(!string.IsNullOrWhiteSpace(desearlize?.ToString()) ? Convert.ToString(desearlize).Replace("\";", "").Replace("];", "]") : usermetadata.meta_value);


                    if (usercollection != null)
                    {
                        usercollection.first_name = usercollection.first_name == "null" ? string.Empty : usercollection.first_name;
                        usercollection.last_name = usercollection.last_name == "null" ? string.Empty : usercollection.last_name;
                        usercollection.mobile = usercollection.mobile == "null" ? string.Empty : usercollection.mobile;
                        usercollection.user_email = !string.IsNullOrEmpty(usercollection.user_email) ? usercollection.user_email : string.Empty;
                        usercollection.offer = usercollection.offer;
                        usercollection.upcomingbookings = usercollection.upcomingbookings;
                        usercollection.notificationall = usercollection.notificationall;
                        usercollection.user_pass = userData.NewPassword.Trim();

                        var jsonupdate = JsonConvert.SerializeObject(usercollection);
                        usermetadata.meta_value = jsonupdate;
                        int userid = _userService.updateuserdata(usermetadata);

                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }



                if (user_id > 0)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    _userService.DeleteUserResetPasswordByUserId(user_id);
                }
                else
                {
                    resp.ResponseCode = Response.Codes.InternalServerError;
                }

            }
            return resp;
        }

        [AllowAnonymous]
        [HttpGet]
        public ResponseExtended<UserData> ValidateUserByKey(string userKey)
        {
            ResponseExtended<UserData> resp = new ResponseExtended<UserData>();

            var userResetPassswordObject = _userService.GetUserResetPasswordByUserKey(userKey);

            if (userResetPassswordObject != null)
            {
                var user = _userService.GetUser((int)userResetPassswordObject.userid);
                UserData userData = new UserData();
                userData.ID = (int)user.ID;
                userData.user_email = user.user_email;
                resp.ResponseData = userData;
                resp.ResponseCode = Response.Codes.OK;
            }
            else
            {
                resp.ResponseCode = Response.Codes.InvalidRequest;
            }

            return resp;
        }

        [AllowAnonymous]
        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> SendForgetPasswordEmail(string emailId)
        {
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            if (emailId == null)
            {
                resp.ResponseMessage = "provided parameters are incorrect";
                resp.ResponseCode = Response.Codes.InvalidRequest;
            }
            else
            {
                var userdetail = _userService.GetUserByEmailId(emailId);

                if (userdetail != null)
                {
                    emailHelper.SendEmailWithTemplateResetPasswordUser(Convert.ToInt32(userdetail.ID), emailId, "Glamly");
                    resp.ResponseCode = Response.Codes.OK;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.InvalidUser;
                    resp.ResponseMessage = "Invalid email id";
                }
            }
            return resp;
        }


        public ResponseExtended<List<int>> GetDeviceTokenList()
        {
            var resp = new ResponseExtended<List<int>>();
            var DeviceTokenList = new List<wp_usermeta>();
            List<int> device = new List<int>();
            try
            {
                DeviceTokenList = _userService.GetStylist();
                if (DeviceTokenList != null)
                {
                    foreach (var item in DeviceTokenList)
                    {
                        var desearlize = serialize.Deserialize(item.meta_value);
                        UserData usercollection = javaserializer.Deserialize<UserData>(!string.IsNullOrWhiteSpace(desearlize?.ToString()) ? Convert.ToString(desearlize).Replace("\";", "").Replace("];", "]") : item.meta_value);

                        device.Add(usercollection.user_id);
                    }
                }
                if (device != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = device;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }

                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updatebookings");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }


        public void SendPushNotificationToAssignedStylist(int userid, string message)
        {
            try
            {
                var sendPushNotificationServiceUrl = ConfigurationManager.AppSettings["PushNotificationServiceUrl"].ToString() + "/send.ashx";
                var pushNotificationServiceApiKey = ConfigurationManager.AppSettings["PushNotificationServiceApiKey"].ToString();

                var pushNotificationTitle = string.Format(@"{0}", message);

                var url = string.Format("{0}?key={1}&message={2}",
                                        sendPushNotificationServiceUrl,
                                        pushNotificationServiceApiKey,
                                        pushNotificationTitle);


                var webClient = new WebClient();
                webClient.DownloadDataAsync(new Uri(url + "&userid=" + userid));

                //messageNotification.PushNotificationSent = true;
                //messageNotificationService.Edit(messageNotification);
            }
            catch (Exception ex)
            {

            }
        }

        [Authorize]
        [HttpDelete]
        public ResponseExtended<System.Web.Mvc.JsonResult> SendDeleteBookingMail(string id)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            var Deletebookings = new wp_glamly_servicesbookings();
            string service = string.Empty;
            string types = string.Empty;
            try
            {
                Deletebookings = _userService.SendDeleteBookingMail(id);
                var desearlize = serialize.Deserialize(Deletebookings.servicewithtypes);
                List<ServiceWithType> jsonservicelist = javaserializer.Deserialize<List<ServiceWithType>>(!string.IsNullOrWhiteSpace(desearlize?.ToString()) ? Convert.ToString(desearlize).Replace("\";", "").Replace("];", "]") : Deletebookings.servicewithtypes);

                foreach (var item in jsonservicelist)
                {
                    service += item.servicename + ",";
                    types += item.typename + ",";
                }

                if (Deletebookings != null)
                {
                    emailHelper.SendEmailWithTemplatedCancelBooking(Deletebookings.firstname, Convert.ToDateTime(Deletebookings.datetime), service.TrimEnd(','), types.TrimEnd(','));
                    resp.ResponseCode = Response.Codes.OK;
                }

                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "deletebooking");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        #endregion

        #region "Payment"
        [AllowAnonymous]
        [HttpPost]
        public ResponseExtended<Dictionary<string, string>> GetPaymentData(int PaymentId)
        {
            ResponseExtended<Dictionary<string, string>> resp = new ResponseExtended<Dictionary<string, string>>();
            Dictionary<string, string> obj = new Dictionary<string, string>();

            try
            {
                wp_glamly_payment payment = _userService.GetPaymentById(PaymentId);

                if (payment != null)
                {
                    // Set order ID                   
                    string orderId = (DibsPayment.Test ? "t" : "p") + payment.id;

                    // Genereate MD5
                    string md5key = Hashing.GetMD5HashString(DibsPayment.MD5K2 + Hashing.GetMD5HashString($"{DibsPayment.MD5K1}merchant={DibsPayment.MerchantId}&orderid={orderId}&currency={DibsPayment.Currency}&amount={Convert.ToInt32(payment.amount) * 100}"));

                    if (!string.IsNullOrEmpty(md5key))
                    {
                        obj.Add("accepturl", Helper.BaseUrl + "dibsaccept.ashx");
                        obj.Add("amount", (Convert.ToInt32(payment.amount) * 100).ToString());
                        //obj.Add("callbackurl", DibsPayment.BaseUrl + "dibscallback.ashx");
                        obj.Add("cancelurl", Helper.BaseUrl + "dibscancel.ashx");
                        obj.Add("currency", DibsPayment.Currency);
                        obj.Add("lang", DibsPayment.Language);
                        obj.Add("md5key", md5key);
                        obj.Add("merchant", DibsPayment.MerchantId);
                        obj.Add("orderid", orderId);
                        obj.Add("preauth", "true");
                        if (DibsPayment.Test)
                            obj.Add("test", "1");

                        resp.ResponseData = obj;
                        resp.ResponseCode = Response.Codes.OK;
                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }
                else
                {
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                }
            }
            catch (Exception ex)
            {
                Logs.Add(ex.Message);
            }

            return resp;
        }


        [Authorize]
        [HttpGet]
        public ResponseExtended<List<UserData>> GetCustomers()
        {
            ResponseExtended<List<UserData>> resp = new ResponseExtended<List<UserData>>();
            var servicesbookingsList = new List<UserData>();
            var user = new List<wp_users>();
            try
            {
                var usermetadata = _userService.GetCustomer();
                if (usermetadata != null)
                {
                    foreach (var item in usermetadata)
                    {
                        var stylishservice = serialize.Deserialize(item.meta_value);
                        UserData usercollection = javaserializer.Deserialize<UserData>(!string.IsNullOrWhiteSpace(stylishservice?.ToString()) ? Convert.ToString(stylishservice).Replace("\";", "").Replace("];", "]") : item.meta_value);

                        if (usercollection.user_status == 0)
                        {
                            if (usercollection.first_name == "null")
                            {
                                usercollection.first_name = "";
                            }
                            if (usercollection.mobile == "null")
                            {
                                usercollection.mobile = "";
                            }
                            servicesbookingsList.Add(usercollection);
                        }

                    }
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = servicesbookingsList.OrderBy(y => y.first_name).ToList();
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "GetCustomers");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }


        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> SaveAvailDatesbyStylist(CalendarService calendarService)
        {
            string jsondatelist = string.Empty;
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            // List<CalendarService> Calendarlist = new List<CalendarService>();
            // List<string> availabledates = new List<string>();
            wp_glamly_stylistschedules schedule = new wp_glamly_stylistschedules();
            //List<AvailableDates> saveaddeddates = new List<AvailableDates>();
            schedule = _userService.GetstylistcalendarByStylistId(calendarService.StylistId);
            string databasedates = string.Empty;
            try
            {
                if (schedule != null)
                {
                    var desearlize = serialize.Deserialize(schedule.date);
                   // List<AvailableDates> addeddates = javaserializer.Deserialize<List<AvailableDates>>(Convert.ToString(desearlize));

                   // var desearlize = serialize.Deserialize(stylistcalendar.date);
                    List<AvailableDates> addeddates = javaserializer.Deserialize<List<AvailableDates>>(!string.IsNullOrWhiteSpace(desearlize?.ToString()) ? Convert.ToString(desearlize).Replace("\";", "").Replace("];", "]") : schedule.date);

                    foreach (var item in addeddates)
                    {
                        databasedates += item.date + ",";
                    }
                    databasedates = databasedates.TrimEnd(',');

                    if (calendarService.date != null)
                    {
                        foreach (var item in calendarService.date)
                        {
                            if (item.status.ToLower() == "add")
                            {
                                string dates = string.Empty;
                                if (!databasedates.Contains(Convert.ToDateTime(item.date).ToString("yyyy/MM/dd")))
                                {
                                    AvailableDates obj = new AvailableDates();
                                    obj.date = (Convert.ToDateTime(item.date).ToString("yyyy/M/d"));
                                    obj.status = "add";
                                    addeddates.Add(obj);
                                }
                            }
                            if (item.status.ToLower() == "delete")
                            {
                                string dates = string.Empty;
                                if (databasedates.Contains(Convert.ToDateTime(item.date).ToString("yyyy/MM/dd")) || databasedates.Contains(Convert.ToDateTime(item.date).ToString("yyyy/M/d")))
                                {
                                    int index = addeddates.Where<AvailableDates>(x => x.date == Convert.ToDateTime(item.date).ToString("yyyy/M/d")).Select<AvailableDates, int>(x => addeddates.IndexOf(x)).FirstOrDefault<int>();
                                    addeddates.RemoveAt(index);
                                    string[] values = databasedates.Split(',');
                                    string newdates = "";
                                    if (values.Length > 0)
                                    {
                                        for (int i = 0; i < values.Length; i++)
                                        {
                                            databasedates = "";
                                            if (values[i] != Convert.ToDateTime(item.date).ToString("yyyy/MM/dd"))
                                            {
                                                newdates += values[i] + ",";
                                            }
                                        }
                                    }
                                    databasedates = newdates.TrimEnd(',');
                                }
                            }
                        }
                    }

                    jsondatelist = JsonConvert.SerializeObject(addeddates);
                    schedule.name = calendarService.UserName;
                    schedule.stylistId = calendarService.StylistId;
                    schedule.date = jsondatelist;
                    schedule.isdeleted = "false";
                    // schedule.date = serialize.Serialize(jsondatelist);
                    int stylistId = _userService.updateStylistSchedule(schedule);

                    if (stylistId > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseMessage = "Calendar data has been successfully saved";
                    }
                    else
                    {
                        resp.ResponseMessage = "Provided parameters are incorrect";
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }
                else
                {
                    if (calendarService != null)
                    {
                        string jsondateslist = string.Empty;
                        wp_glamly_stylistschedules calobj = new wp_glamly_stylistschedules();
                        jsondateslist = JsonConvert.SerializeObject(calendarService.date);
                        calobj.name = calendarService.UserName;
                        calobj.stylistId = calendarService.StylistId;
                        calobj.date = jsondateslist;
                        calobj.isdeleted = "false";
                        // calobj.date = serialize.Serialize(jsondateslist);
                        int stylistId = _userService.savecalendardata(calobj);

                        if (stylistId > 0)
                        {
                            resp.ResponseCode = Response.Codes.OK;
                            resp.ResponseMessage = "Calendar data has been successfully saved";
                        }
                        else
                        {
                            resp.ResponseMessage = "Provided parameters are incorrect";
                            resp.ResponseCode = Response.Codes.InvalidRequest;
                        }
                    }
                    else
                    {
                        resp.ResponseMessage = "Provided parameters are incorrect";
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "saveAvailDatesbyStylist");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }


        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> SaveAvailDatesbyStylist1(CalendarService calendarService)
        {
            string jsondatelist = string.Empty;
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            //List<CalendarService> Calendarlist = new List<CalendarService>();
            //List<string> availabledates = new List<string>();
            wp_glamly_stylistschedules schedule = new wp_glamly_stylistschedules();

            string selecteddate = Convert.ToDateTime(calendarService.caldate).ToString("yyyy/MM/dd");

            schedule = _userService.GetstylistcalendarByStylistId(calendarService.StylistId);
            string databasedates = string.Empty;
            try
            {
                if (schedule != null)
                {
                    var desearlize = serialize.Deserialize(schedule.date);
                    List<AvailableDates> addeddates = javaserializer.Deserialize<List<AvailableDates>>(Convert.ToString(desearlize));
                    foreach (var item in addeddates)
                    {
                        databasedates += item.date + ",";
                    }
                    databasedates = databasedates.TrimEnd(',');

                    if (selecteddate != null)
                    {
                        if (calendarService.status.ToLower() == "add")
                        {
                            string dates = string.Empty;
                            if (!databasedates.Contains(selecteddate))
                            {
                                AvailableDates obj = new AvailableDates();
                                obj.date = selecteddate;
                                obj.status = "add";
                                addeddates.Add(obj);
                            }
                        }
                        if (calendarService.status.ToLower() == "delete")
                        {
                            string dates = string.Empty;
                            if (databasedates.Contains(selecteddate))
                            {
                                int index = addeddates.Where<AvailableDates>(x => x.date == selecteddate).Select<AvailableDates, int>(x => addeddates.IndexOf(x)).Single<int>();
                                addeddates.RemoveAt(index);
                            }
                        }


                    }


                    //if (calendarService.date != null)
                    //{
                    //    foreach (var item in calendarService.date)
                    //    {
                    //        if (item.status.ToLower() == "add")
                    //        {
                    //            string dates = string.Empty;
                    //            if (!databasedates.Contains(item.date))
                    //            {
                    //                AvailableDates obj = new AvailableDates();
                    //                obj.date = item.date;
                    //                obj.status = "add";
                    //                addeddates.Add(obj);
                    //            }
                    //        }
                    //        if (item.status.ToLower() == "delete")
                    //        {
                    //            string dates = string.Empty;
                    //            if (databasedates.Contains(item.date))
                    //            {
                    //                int index = addeddates.Where<AvailableDates>(x => x.date == item.date.ToString()).Select<AvailableDates, int>(x => addeddates.IndexOf(x)).Single<int>();
                    //                addeddates.RemoveAt(index);
                    //            }
                    //        }
                    //    }
                    //}

                    jsondatelist = JsonConvert.SerializeObject(addeddates);
                    schedule.name = calendarService.UserName;
                    schedule.stylistId = calendarService.StylistId;
                    schedule.date = serialize.Serialize(jsondatelist);
                    schedule.isadmin = 1;
                    int stylistId = _userService.updateStylistSchedule(schedule);

                    if (stylistId > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseMessage = "Calendar data has been successfully saved";
                    }
                    else
                    {
                        resp.ResponseMessage = "Provided parameters are incorrect";
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }
                else
                {
                    if (calendarService != null)
                    {
                        string jsondateslist = string.Empty;
                        wp_glamly_stylistschedules calobj = new wp_glamly_stylistschedules();
                        jsondateslist = JsonConvert.SerializeObject(calendarService.date);
                        calobj.name = calendarService.UserName;
                        calobj.stylistId = calendarService.StylistId;
                        calobj.date = serialize.Serialize(jsondateslist);
                        calobj.isadmin = 1;
                        int stylistId = _userService.savecalendardata(calobj);

                        if (stylistId > 0)
                        {
                            resp.ResponseCode = Response.Codes.OK;
                            resp.ResponseMessage = "Calendar data has been successfully saved";
                        }
                        else
                        {
                            resp.ResponseMessage = "Provided parameters are incorrect";
                            resp.ResponseCode = Response.Codes.InvalidRequest;
                        }
                    }
                    else
                    {
                        resp.ResponseMessage = "Provided parameters are incorrect";
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "saveAvailDatesbyStylist");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }



        #endregion

        #region "FAQ"
        [Authorize]
        [HttpGet]
        public ResponseExtended<List<wp_glamly_faq>> GetFAQList()
        {
            var resp = new ResponseExtended<List<wp_glamly_faq>>();
            var FAQlist = new List<wp_glamly_faq>();
            try
            {
                FAQlist = _userService.GetFAQ();

                if (FAQlist != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = FAQlist;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "GetFAQList");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> AddFAQ(FAQ faqdata)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            wp_glamly_faq tempdata = new wp_glamly_faq();
            tempdata.question = faqdata.question;
            tempdata.answer = faqdata.answer;
            tempdata.isdeleted = "false";

            int faqid = _userService.SaveFAQ(tempdata);

            if (faqid > 0)
            {
                resp.ResponseCode = Response.Codes.OK;
            }
            else
            {
                resp.ResponseCode = Response.Codes.InvalidRequest;
            }
            return resp;

        }

        [Authorize]
        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> UpdateFAQData(FAQ faqdata)
        {
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var faqdetails = _userService.GetFAQById(faqdata.id);
                if (faqdetails != null)
                {

                    faqdetails.question = faqdata.question == "null" ? string.Empty : faqdata.question;
                    faqdetails.answer = faqdata.answer == "null" ? string.Empty : faqdata.answer;

                    int faqid = _userService.updateFAQdata(faqdetails);

                    if (faqid > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseMessage = "FAQ data has been updated successfully";
                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }

                }
                else
                {
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                }


                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "UpdateStylistPage");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> RemoveFAQ(FAQ faqdata)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            var Isdeleted = _userService.DeleteFAQ(faqdata.id);

            if (Isdeleted)
            {
                resp.ResponseCode = Response.Codes.OK;
            }
            else
            {
                resp.ResponseCode = Response.Codes.InvalidRequest;
            }
            return resp;

        }

        #endregion

        #region Upload

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> SaveProfileFiles(string filename)
        {
            //var guidimg = Guid.NewGuid();
            var guidimg = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string ext = string.Empty;
            try
            {
                // Check request
                string userid = HttpContext.Current.Request.Headers["UserId"];
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                    return BadRequest();
                else
                {
                    // Get current user
                    string current = string.Empty;
                    if (current == null)
                        return BadRequest();
                    else
                    {
                        // Set directory
                        string dir = UploadFile.UserPhotoPath;

                        if (!Request.Content.IsMimeMultipartContent())
                        {
                            throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                        }


                        if (!string.IsNullOrEmpty(filename))
                        {
                            string imagepath = string.Empty;
                            string photo = UploadFile.UserPhotoPath + "/" + filename.ToString();
                            if (!string.IsNullOrWhiteSpace(photo))
                                imagepath = string.Format("{0}/Uploads/UserPhoto/{1}", ConfigurationManager.AppSettings["ProfileImageUrl"] != null ? ConfigurationManager.AppSettings["ProfileImageUrl"]?.ToString().TrimEnd(new char[] { '/' }) : "", Path.GetFileName(photo));
                            if (string.IsNullOrEmpty(imagepath))
                            {
                                guidimg = guidimg.ToString();
                            }
                            else
                            {
                                guidimg = filename.ToString();
                            }
                        }
                   


                        // Save file
                        var provider = new CustomMultipartFormDataStreamProvider(dir, guidimg.ToString());
                        await Task.Run(async () => await Request.Content.ReadAsMultipartAsync(provider));


                        foreach (var file in provider.FileData)
                        {
                            ext = System.IO.Path.GetExtension(file.LocalFileName);                            
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }

            return Ok(guidimg + ext);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> SaveFiles()
        {
            //var guidimg = Guid.NewGuid();
            var guidimg = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string ext = string.Empty;
            try
            {
                // Check request
                string userid = HttpContext.Current.Request.Headers["UserId"];
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                    return BadRequest();
                else
                {
                    // Get current user
                    string current = string.Empty;
                    if (current == null)
                        return BadRequest();
                    else
                    {
                        // Set directory
                        string dir = UploadFile.UserPhotoPath;

                        if (!Request.Content.IsMimeMultipartContent())
                        {
                            throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                        }                    



                        // Save file
                        var provider = new CustomMultipartFormDataStreamProvider(dir, guidimg.ToString());
                        await Task.Run(async () => await Request.Content.ReadAsMultipartAsync(provider));


                        foreach (var file in provider.FileData)
                        {

                            ext = System.IO.Path.GetExtension(file.LocalFileName);

                            //***** Commeted the resize image code due check  to fix in app side ********//

                            //try
                            //{
                            //    Image img = null;

                            //    using (FileStream fs = new FileStream(file.LocalFileName, FileMode.Open, FileAccess.Read))
                            //    {
                            //        // Load image
                            //        img = Image.FromStream(fs);

                            //        if (new string[] { ".jpeg", ".jpg",".png" }.Contains(Path.GetExtension(file.LocalFileName).ToLower()))
                            //        {
                            //            try
                            //            {
                            //                // Reset position
                            //                fs.Position = 0;

                            //                // Get orientation
                            //                int orientation = 1;
                            //                ExifLib.ExifReader exif = new ExifLib.ExifReader(fs);
                            //                object temp;
                            //                if (exif.GetTagValue<object>(ExifLib.ExifTags.Orientation, out temp) && temp != null)
                            //                    orientation = Convert.ToInt32(temp);

                            //                // Rotate image
                            //                switch (orientation)
                            //                {
                            //                    case 1:
                            //                        // No rotation required.
                            //                        break;
                            //                    case 2:
                            //                        img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            //                        break;
                            //                    case 3:
                            //                        img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            //                        break;
                            //                    case 4:
                            //                        img.RotateFlip(RotateFlipType.Rotate180FlipX);
                            //                        break;
                            //                    case 5:
                            //                        img.RotateFlip(RotateFlipType.Rotate90FlipX);
                            //                        break;
                            //                    case 6:
                            //                        img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            //                        break;
                            //                    case 7:
                            //                        img.RotateFlip(RotateFlipType.Rotate270FlipX);
                            //                        break;
                            //                    case 8:
                            //                        img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            //                        break;
                            //                }
                            //            }
                            //            catch (Exception ex)
                            //            {
                            //            }
                            //        }
                            //    }
                            //    double destHeight = 200;
                            //    double destWidth = 200;
                            //    double rectHeight = img.Height;
                            //    double rectWidth = img.Width;
                            //    double maxHeight = 200;
                            //    double maxWidth = 200;
                            //    double posX = 0;
                            //    double posY = 0;
                            //    double percHeight = 1;
                            //    double percWidth = 1;
                            //    double percentage = 1;

                            //    if (img.Height > maxHeight)
                            //        percHeight = maxHeight / img.Height;
                            //    if (img.Width > maxWidth)
                            //        percWidth = maxWidth / img.Width;
                            //    percentage = Math.Min(percHeight, percWidth);
                            //    rectHeight = img.Height * percentage;
                            //    rectWidth = img.Width * percentage;
                            //    if (rectHeight > rectWidth)
                            //        posX = (destWidth - rectWidth) / 2;
                            //    else if (rectHeight < rectWidth)
                            //        posY = (destHeight - rectHeight) / 2;                               

                            //    // Resize image
                            //    Rectangle destRect = new Rectangle((int)posX, (int)posY, (int)rectWidth, (int)rectHeight);
                            //    Bitmap destImage = new Bitmap((int)destWidth, (int)destHeight);
                            //    destImage.MakeTransparent(Color.Red);
                            //    destImage.SetResolution(img.HorizontalResolution, img.VerticalResolution);

                            //    using (Graphics graphics = Graphics.FromImage(destImage))
                            //    {
                            //        if (rectHeight != destHeight || rectWidth != destWidth)
                            //            graphics.Clear(Color.White);
                            //        graphics.CompositingMode = CompositingMode.SourceCopy;
                            //        graphics.CompositingQuality = CompositingQuality.HighQuality;
                            //        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            //        graphics.SmoothingMode = SmoothingMode.HighQuality;
                            //        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                            //        using (ImageAttributes wrapMode = new ImageAttributes())
                            //        {
                            //            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                            //            graphics.DrawImage(img, destRect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, wrapMode);
                            //        }
                            //    }

                            //    // Save image
                            //    string filePath = Path.ChangeExtension(file.LocalFileName, "png");
                            //    destImage.Save(filePath, ImageFormat.Png);
                            //  ext = System.IO.Path.GetExtension(file.LocalFileName);
                            //    // Delete original file
                            //    if (file.LocalFileName.ToLower() != filePath.ToLower())
                            //        File.Delete(file.LocalFileName);
                            //}
                            //catch (Exception ex)
                            //{
                            //    // Delete original file
                            //    File.Delete(file.LocalFileName);
                            //}
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }

            return Ok(guidimg + ext);
        }



        [Authorize]
        [HttpGet]
        public ResponseExtended<List<wp_glamly_stylistTemp>> GetStylistPage()
        {

            //Unable to cast object of type 'System.String' to type 'System.Collections.IList
            var resp = new ResponseExtended<List<wp_glamly_stylistTemp>>();
            var StylistPageList = new List<wp_glamly_stylistTemp>();
            try
            {
                StylistPageList = _userService.GetStylistPageList();
                if (StylistPageList != null)
                {
                    foreach (var item in StylistPageList)
                    {
                        if (!string.IsNullOrEmpty(item.profileimageguid))
                        {
                            string photo = Directory.GetFiles(UploadFile.UserPhotoPath, item.profileimageguid.ToString() + "*").FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(photo))
                                item.profileimageguid = Helper.BaseUrl + $"uploads/userphoto/" + Path.GetFileName(photo);

                        }
                    }

                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = StylistPageList;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "GetStylistPage");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        [Authorize]
        [HttpGet]
        public ResponseExtended<wp_glamly_stylistTemp> GetStylistsPageById(int PageId)
        {
            ResponseExtended<wp_glamly_stylistTemp> resp = new ResponseExtended<wp_glamly_stylistTemp>();

            try
            {
                var StylistPagedata = _userService.GetStylistPageById(PageId);
                if (StylistPagedata != null)
                {
                    if (!string.IsNullOrEmpty(StylistPagedata.profileimageguid))
                    {
                        string photo = Directory.GetFiles(UploadFile.UserPhotoPath, StylistPagedata.profileimageguid.ToString() + "*").FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(photo))
                            StylistPagedata.profileimageguid = Helper.BaseUrl + $"uploads/userphoto/" + Path.GetFileName(photo);
                    }
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = StylistPagedata;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updatebookings");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        [Authorize]
        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> UpdateStylistPage(StylistPageData stylistPageModel)
        {
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var pagedata = _userService.GetStylistPageById(stylistPageModel.id);
                if (pagedata != null)
                {

                    pagedata.name = stylistPageModel.name == "null" ? string.Empty : stylistPageModel.name;
                    pagedata.skill1 = stylistPageModel.skillone == "null" ? string.Empty : stylistPageModel.skillone;
                    pagedata.skill2 = stylistPageModel.skillsecond == "null" ? string.Empty : stylistPageModel.skillsecond;
                    pagedata.skill3 = !string.IsNullOrEmpty(stylistPageModel.skillthird) ? stylistPageModel.skillthird : string.Empty;
                    if (stylistPageModel.profileimageguid != "")
                    {
                        pagedata.profileimageguid = !string.IsNullOrEmpty(stylistPageModel.profileimageguid) ? stylistPageModel.profileimageguid : string.Empty;
                    }
                    int userid = _userService.updateStylistPagedata(pagedata);

                    if (userid > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseMessage = "Stylist page has been updated successfully";
                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }

                }
                else
                {
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                }


                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updateUser");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }



        #endregion
    }
}
