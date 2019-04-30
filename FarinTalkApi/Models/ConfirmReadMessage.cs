using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarinTalkApi.Models
{
    public class ConfirmReadMessage
    {
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public DateTime CreationDate { get; set; }
        public string MessageId { get; set; }

    }
}