using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarinTalkApi.Models
{
    public static class DbContext
    {
        public static List<User> Users = new List<User>();
        public static List<UserConnection> UserConnections=new List<UserConnection>();
        public static List<Message> Messages=new List<Message>();
        public static List<ConfirmReadMessage> ConfirmReadMessages=new List<ConfirmReadMessage>();

    }
}