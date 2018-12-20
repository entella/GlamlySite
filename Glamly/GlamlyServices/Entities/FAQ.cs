using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlamlyServices.Entities
{
    public class FAQ
    {
        public int id { get; set; }
        public string question { get; set; }
        public string answer { get; set; }
    }
}