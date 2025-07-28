using TikTakProgram.Dtos;

namespace TikTakProgram;

public class TikTakLobbyManager
{
    private readonly string playerName;
    private HttpRequests httpRequests = new HttpRequests();
    private HistoryViewer historyViewer = new HistoryViewer();

    public TikTakLobbyManager(string? playerName)
    {
        this.playerName = playerName!;
    }

    public async Task<(JoinResponseDto join, string sessionId)?> ShowMenu()
    {
        TikTakMusicHandler.StartMainLoop();
        while (true)
        {
            Console.Clear();
            List<SessionLobbyDto> lobbies = await httpRequests.GetAvailableLobbies();
            PrintLobbyList(lobbies);

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.Q:
                    return null;            

                case ConsoleKey.N:
                    continue;

                case ConsoleKey.M:
                    {
                        TikTakMusicHandler.SwitchMusicState();
                        break;                    
                    }

                case ConsoleKey.C:
                    {
                        var created = await CreateNewLobby();          
                        if (created != null) return created;
                        break;
                    }
                case ConsoleKey.H:
                    {
                        Console.Clear();
                        await historyViewer.ShowGameHistory();
                        break;
                    }

                case >= ConsoleKey.D1 and <= ConsoleKey.D9:
                case >= ConsoleKey.NumPad1 and <= ConsoleKey.NumPad9:
                    {
                        int lobbyIndex = keyInfo.KeyChar - '1';
                        if (lobbyIndex >= 0 && lobbyIndex < lobbies.Count)
                        {
                            var joined = await TryJoinLobby(lobbies[lobbyIndex]);   
                            if (joined != null) return joined;
                        }
                        break;
                    }
            }
        }
    }

    private void PrintLobbyList(List<SessionLobbyDto> lobbies)
    {
        if (lobbies.Count == 0)
        {
            Console.WriteLine("Oh no, no lobby =(");
        }
        else
        {
            for (int lobbyIndex = 0; lobbyIndex < lobbies.Count; lobbyIndex++)
                PrintLobby(lobbyIndex, lobbies[lobbyIndex]);
        }
        Console.WriteLine("[N] – update lobby list  |  [Q] – quit  | [C] - create own lobby  | [H] - show Games History  | [M] - on/off the music");
    }

    private void PrintLobby(int idx, SessionLobbyDto lobby)
    {
        string host = lobby.players.Values.FirstOrDefault() ?? "noname";
        Console.WriteLine($"{idx + 1}. Lobby {lobby.sessionId}");
        Console.WriteLine($"Created by: {host}");
        Console.WriteLine("--------------------------------");
    }

    private async Task<(JoinResponseDto, string)?> TryJoinLobby(SessionLobbyDto lobby)
    {
        try
        {
            JoinResponseDto? joinResponse = await httpRequests.JoinLobby(lobby.sessionId, playerName);
            Console.WriteLine($"\n {joinResponse.message}");
            return (joinResponse, lobby.sessionId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n {ex.Message}\nPress any button");
            return null;
        }
    }

    private async Task<(JoinResponseDto, string)?> CreateNewLobby()
    {
        try
        {
            CreateSessionDto cs = await httpRequests.CreateSession(playerName);
            JoinResponseDto joinResponse = new(playerName, cs.symbol, cs.message);
            Console.WriteLine($"\n{cs.message}");
            return (joinResponse, cs.sessionId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n {ex.Message}\nPress any button");
            return null;
        }
    }

    
}
