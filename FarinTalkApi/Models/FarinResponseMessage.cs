using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace FarinTalkApi.Models
{
    public class FarinResponseMessage
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public object DataItem { get; set; }
        public List<object> DataItemList { get; set; }
        public bool IsSuccess { get; set; }
    }
}