using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTakProgram
{
    public class GameStateValidator
    {
        private readonly string MySymbol;
        private readonly string SessionId;

        public GameStateValidator(string mySymbol, string sessionId)
        {
            MySymbol = mySymbol;
            SessionId = sessionId;
        }

        public bool CanMove(out string reason)
        {
            var state = HttpRequests.GetGameState(SessionId);
            if (state == null)
            {
                reason = "Failed to retrieve game state.";
                return false;
            }

            if (state.players.Count < 2)
            {
                reason = "Waiting for the second player.";
                return false;
            }

            if (!string.IsNullOrEmpty(state.winner))
            {
                reason = $"Game over. Winner: {state.winner}";
                return false;
            }

            if (!string.Equals(state.currentTurn, MySymbol, StringComparison.OrdinalIgnoreCase))
            {
                reason = $"It's the turn of the player with symbol: {state.currentTurn}";
                return false;
            }

            reason = "";
            return true;
        }
    }
}
