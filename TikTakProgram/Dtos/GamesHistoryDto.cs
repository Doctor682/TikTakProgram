using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTakProgram.Dtos
{
    public record GamesHistoryDto(string sessionId, 
                                  Dictionary<string, string> players,
                                  Dictionary<string, string> board,
                                  string winner, string endedAt);
}
