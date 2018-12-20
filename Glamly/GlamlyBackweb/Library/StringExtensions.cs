using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;


namespace GlamlyBackweb.Library
{
    public static class StringExtensions
    {
        public static string GetValueFromWebConfig(this string key)
        {
            return ConfigurationManager.AppSettings[key] ?? string.Empty;
        }
    }
}