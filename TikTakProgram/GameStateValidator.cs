using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TikTakProgram.Dtos;

namespace TikTakProgram
{
    public class GameStateValidator
    {
        private readonly string _symbol;
        private readonly string _sessionId;
        private string reason;
        private HttpRequests httpRequests = new HttpRequests();

        public GameStateValidator(string symbol, string sessionId)
        {
            _symbol = symbol;
            _sessionId = sessionId;
            reason = string.Empty;
        }

        public async Task<(bool,string)> CanMove()
        {
            GameStatusResponseDto? state = await httpRequests.GetGameState(_sessionId);
            if (state == null)
            {
                reason = "Failed to retrieve game state.";
                return (false, reason);
            }

            if (state.players.Count < 2)
            {
                reason = "Waiting for the second player.";
                return (false, reason);
            }

            if (!string.IsNullOrEmpty(state.winner))
            {
                reason = $"Game over. Winner: {state.winner}";
                return (false, reason);
            }

            if (!string.Equals(state.currentTurn, _symbol, StringComparison.OrdinalIgnoreCase))
            {
                reason = $"It's the turn of the player with symbol: {state.currentTurn}";
                return (false, reason);
            }

            reason = "";
            return (true, reason);
        }
    }
}
