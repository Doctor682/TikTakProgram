using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using TikTakProgram.Dtos;

namespace TikTakProgram;

public static class HttpRequests
{
    private static readonly HttpClient client = new()
    {
        BaseAddress = new Uri("https://tiktak-server.onrender.com/")
        //BaseAddress = new Uri("http://192.168.0.120:8080/")
    };

    public static MoveResultDto? SendMove(string playerSymbol, CellAddress cell, string sessionId)
    {
        var req = new MoveRequestDto(playerSymbol, cell.ToString());
        var json = JsonConvert.SerializeObject(req);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var resp = client.PostAsync($"game/{Uri.EscapeDataString(sessionId)}/move", content).Result;
        var respContent = resp.Content.ReadAsStringAsync().Result;

        if (!resp.IsSuccessStatusCode)
        {
            var errorObj = JsonConvert.DeserializeObject<ErrorResponseDto>(respContent);
            Console.WriteLine("Error: " + errorObj?.error);
            return null;
        }

        return JsonConvert.DeserializeObject<MoveResultDto>(respContent);
    }

    public static GameStatusResponseDto? GetGameState(string sessionId)
    {
        var resp = client.GetAsync($"game/{Uri.EscapeDataString(sessionId)}/state").Result;
        if (!resp.IsSuccessStatusCode) return null;

        var json = resp.Content.ReadAsStringAsync().Result;
        return JsonConvert.DeserializeObject<GameStatusResponseDto>(json);
    }

    public static List<SessionLobbyDto> GetAvailableLobbies()
    {
        var resp = client.GetAsync("lobby/available").Result;
        resp.EnsureSuccessStatusCode();

        var json = resp.Content.ReadAsStringAsync().Result;

        return JsonConvert.DeserializeObject<List<SessionLobbyDto>>(json)
               ?? new List<SessionLobbyDto>();
    }

    public static JoinResponseDto JoinLobby(string sessionId, string playerName)
    {
        var url = $"lobby/{Uri.EscapeDataString(sessionId)}/join";

        var body = new { name = playerName };
        var json = JsonConvert.SerializeObject(body);

        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var resp = client.PostAsync(url, content).Result;
        resp.EnsureSuccessStatusCode();

        var responseJson = resp.Content.ReadAsStringAsync().Result;
        return JsonConvert.DeserializeObject<JoinResponseDto>(responseJson)!;
    }

    public static CreateSessionDto CreateSession(string playerName)
    {
        const string url = "lobby/create";

        var json = JsonConvert.SerializeObject(new PlayerJoinRequestDto(playerName));
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var resp = client.PostAsync(url, content).Result;
        resp.EnsureSuccessStatusCode();

        var responseJson = resp.Content.ReadAsStringAsync().Result;
        return JsonConvert.DeserializeObject<CreateSessionDto>(responseJson)!;
    }

    public static List<GamesHistoryDto> GetGamesHistory()
    {
        var resp = client.GetAsync("game/history").Result;
        resp.EnsureSuccessStatusCode();

        var json = resp.Content.ReadAsStringAsync().Result;
        return JsonConvert.DeserializeObject<List<GamesHistoryDto>>(json)
               ?? new List<GamesHistoryDto>();
    }

    public static LeaveResponseDto? LeaveLobby(string sessionId, string? playerName)
    {
        var json = JsonConvert.SerializeObject(new PlayerLeaveRequestDto(playerName));
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var resp = client.PostAsync($"lobby/{Uri.EscapeDataString(sessionId)}/leave", content).Result;
        resp.EnsureSuccessStatusCode();

        var responseJson = resp.Content.ReadAsStringAsync().Result;
        return JsonConvert.DeserializeObject<LeaveResponseDto>(responseJson)!;
    }
}