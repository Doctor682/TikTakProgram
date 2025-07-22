using System;
using System.Net.Http;
using System.Threading.Tasks;
using TikTakProgram;
using static TikTakProgram.HttpRequests;

PlayerLoginHandler.LoginMessage();
string? playerName = PlayerLoginHandler.LoginInGame();


while (true)
{
    Console.Clear();
    TikTakLobbyManager lobbyPrinter = new TikTakLobbyManager(playerName);
    var result = lobbyPrinter.ShowMenu();

    if (result == null)
    {
        Console.WriteLine("You leave from game. See ya");
        break;
    }

    var (joinResponse, sessionId) = result.Value;

    //треба ось тут зробити перевірку на те чи є в лобі два гравця, не виводячи боард тільки наступний рядок.
    Console.WriteLine("Press 'Q' if you want to leave from lobby; any other will connect to lobby");
    ConsoleKeyInfo key = Console.ReadKey(true);
    if (key.Key == ConsoleKey.Q)
    {
        try
        {
            HttpRequests.LeaveLobby(sessionId, playerName!);
            Console.WriteLine("You have left the lobby.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while leaving: {ex.Message}");
        }

        Console.WriteLine("Press any key to return to the menu...");
        Console.ReadKey(true);
        continue;
    }
    
    Console.Clear();
    BoardDimensions dims = new BoardDimensions();
    TikTakBoardEngine game = new TikTakBoardEngine(dims, joinResponse.symbol, joinResponse.name, sessionId);
    game.Run();

}
