using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTakProgram
{
    public enum ErrorCode
    {
        Unknown = 0,
        InvalidInput = 1001,
        NotFound = 1002,
        Unauthorized = 1003,
        Forbidden = 1004,
        Conflict = 1005,
        InternalError = 1006,
        ValidationError = 1007
    }
}
