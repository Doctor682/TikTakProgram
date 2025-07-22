using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTakProgram.Dtos
{
    public record SessionLobbyDto(string sessionId, Dictionary<string, string> players);
}
