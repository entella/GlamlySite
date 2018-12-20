using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlamlyServices.Entities
{
    public class UserService
    {

        public int id { get; set; }
        public string servicename { get; set; }
        public int status { get; set; }
        public string service_image { get; set; }
        public List<Dictionary<string, string>> service_type { get; set; }
    }
}