using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTakProgram.Dtos
{
    public record ErrorResponse(string error,
                                int code,
                                Dictionary<string, string?> details);
}
