using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;

namespace GlamlyServices.Entities
{
    public class UploadFile
    {

        public static string UploadPath
        {
            get
            {
                string path = Path.Combine(HttpRuntime.AppDomainAppPath, "Uploads");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string UserPhotoPath
        {
            get
            {
                string path = Path.Combine(UploadPath, "UserPhoto");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

    }
}