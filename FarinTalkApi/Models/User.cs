using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarinTalkApi.Models
{
    public class User
    {
        public string UserName { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastStatus { get; set; }

    }
}