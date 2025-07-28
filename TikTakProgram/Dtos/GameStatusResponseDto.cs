using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTakProgram.Dtos
{
    public record GameStatusResponseDto(Dictionary<string, string?> board,
                                        string currentTurn,
                                        Dictionary<string, string?> players,
                                        string? winner,
                                        string message,
                                        string messageCode,
                                        Dictionary<string, string?> messageParams,
                                        bool isGameOver);
}
