using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarinTalkApi.Models
{
    public class Message
    {
        public string MessageId { get; set; }
        public string MessageText { get; set; }
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public DateTime CreationDate { get; set; }
        public bool Delivered { get; set; }
        public bool ConfirmGetMessageByServer { get; set; }
        public bool ConfirmGetMessageByOtherClient { get; set; }
        public DateTime GetConfirmServerDateTime { get; set; }
        public DateTime GetConfirmOtherClientDateTime { get; set; }

    }
}