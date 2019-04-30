using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarinTalkApi.Models
{
    public static class Utility
    {
        public static string GetClearPhoneNumber(string number)
        {
            if (string.IsNullOrEmpty(number))
                return string.Empty;
            number = number.Trim().ToLower();
            if (number.StartsWith("98"))
                number = "0" + number.Substring(2);
            if (number.StartsWith("+98"))
                number = "0" + number.Substring(3);
            return number;
        }
    }
}