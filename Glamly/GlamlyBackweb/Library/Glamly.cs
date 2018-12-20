using GlamlyBackweb.Models;
using GlamlyData;
using GlamlyServices.Services;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;


namespace GlamlyBackweb.Library
{
    public class Glamly
    {
        public string AccessToken
        {
            get
            {
                string accessToken = null;

                try
                {
                    string key = "Authorization";

                    if (HttpContext.Current.Request.Headers.AllKeys.Contains(key) && HttpContext.Current.Request.Headers.GetValues(key).Any())
                    {
                        // Get access token
                        string keyData = HttpContext.Current.Request.Headers.GetValues(key).FirstOrDefault();
                        Match match = Regex.Match(keyData, @"^(\s+)?(bearer)((\s+)?:(\s+)?|\s+)(.+)(\s+)?$", RegexOptions.IgnoreCase);
                        if (match.Success)
                            accessToken = match.Groups[6].Value;
                    }
                }
                catch (Exception ex)
                {
                }

                return accessToken;
            }
        }
        private IUserServices _userService = new UserServices();
        public bool IsValidAccessToken
        {
            get
            {
                bool valid = false;
                wp_users objvalid = new wp_users();
                try
                {
                    if (!string.IsNullOrWhiteSpace(AccessToken))
                    {
                        // Get token string
                        TokenString tokenString = new TokenString(AccessToken);

                        if (tokenString.Type == TokenString.TokenType.AccessToken)
                        {
                            // Get expire date
                            int expiresIn = (tokenString.Values.Any() ? Convert.ToInt32(tokenString.Values.FirstOrDefault()) : 0);
                            DateTime expires = tokenString.CreateDate.AddSeconds(expiresIn);

                            if (expires > DateTime.UtcNow)
                            {
                                // Get user
                                objvalid = _userService.GetUser(Convert.ToInt32(tokenString.UserId));
                               // valid = new UserBL().Exists(tokenString.UserId);

                                // Update refresh token
                                if (objvalid.ID > 0) { }
                                   // new RefreshTokenBL().UpdateDate(tokenString.UserId, tokenString.Values[1]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                return valid;
            }
        }
    }
}