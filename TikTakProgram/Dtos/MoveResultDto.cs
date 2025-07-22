using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTakProgram.Dtos
{
    public record MoveResultDto(Dictionary<string, string?> board,
                                string? currentTurn,
                                string? winner,
                                string message,
                                bool isGameOver);
}
