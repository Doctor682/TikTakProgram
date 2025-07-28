using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTakProgram
{
    public static class ErrorMessages
    {
        public static string GetMessageByCode(int code)
        {
            return code switch
            {
                1001 => "The input data is invalid.",
                1002 => "The requested resource was not found.",
                1003 => "You are not authorized to perform this action.",
                1004 => "Access to the requested resource is forbidden.",
                1005 => "A conflict occurred. Possibly a duplicate or state mismatch.",
                1006 => "The server encountered an internal error.",
                1007 => "Some fields did not pass validation.",
                _ => "An unknown error occurred."
            };
        }
    }

}
