using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using TikTakProgram.Dtos;

namespace TikTakProgram;

public class HttpRequests
{
    public readonly HttpClient client = new()
    {
        BaseAddress = new Uri("https://tiktak-server.onrender.com/")
        //BaseAddress = new Uri("http://192.168.0.122:8080/")
    };

    public async Task<MoveResultDto?> SendMove(string playerSymbol, CellAddress cell, string sessionId)
    {
        MoveRequestDto? req = new MoveRequestDto(playerSymbol, cell.ToString());
        string json = JsonConvert.SerializeObject(req);

        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage resp = await client.PostAsync($"game/{Uri.EscapeDataString(sessionId)}/move", content);
        string respContent = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
        {
            string errorJson = await resp.Content.ReadAsStringAsync();
            ErrorHandler.PrintServerError(errorJson);
            return null;
        }

        return JsonConvert.DeserializeObject<MoveResultDto>(respContent);
    }


    public async Task<GameStatusResponseDto?> GetGameState(string sessionId)
    {
        HttpResponseMessage resp = await client.GetAsync($"game/{Uri.EscapeDataString(sessionId)}/state");
        if (!resp.IsSuccessStatusCode) return null;

        string respContent = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            ErrorHandler.PrintServerError(respContent);
            return null;
        }

        string json = await resp.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<GameStatusResponseDto>(json);
    }

    public async Task<List<SessionLobbyDto>> GetAvailableLobbies()
    {
        HttpResponseMessage resp = await client.GetAsync("lobby/available");

        string respContent = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            ErrorHandler.PrintServerError(respContent);
            return null;
        }

        string json = await resp.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<List<SessionLobbyDto>>(json)
               ?? new List<SessionLobbyDto>();
    }

    public async Task<JoinResponseDto?> JoinLobby(string sessionId, string playerName)
    {
        string url = $"lobby/{Uri.EscapeDataString(sessionId)}/join";

        var body = new { name = playerName };
        string json = JsonConvert.SerializeObject(body);

        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage resp = await client.PostAsync(url, content);
        string respContent = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
        {
            ErrorHandler.PrintServerError(respContent);
            return null;
        }

        return JsonConvert.DeserializeObject<JoinResponseDto>(respContent);
    }


    public async Task<CreateSessionDto?> CreateSession(string playerName)
    {
        string url = "lobby/create";

        string json = JsonConvert.SerializeObject(new PlayerJoinRequestDto(playerName));
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage resp = await client.PostAsync(url, content);
        string respContent = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
        {
            ErrorHandler.PrintServerError(respContent);
            return null;
        }

        return JsonConvert.DeserializeObject<CreateSessionDto>(respContent);
    }


    public async Task<List<GamesHistoryDto>> GetGamesHistory()
    {
        HttpResponseMessage resp = await client.GetAsync("game/history");

        string respContent = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            ErrorHandler.PrintServerError(respContent);
            return null;
        }

        string json = await resp.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<GamesHistoryDto>>(json)
               ?? new List<GamesHistoryDto>();
    }

    public async Task<LeaveResponseDto?> LeaveLobby(string sessionId, string? playerName)
    {
        string json = JsonConvert.SerializeObject(new PlayerLeaveRequestDto(playerName));
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage resp = await client.PostAsync($"lobby/{Uri.EscapeDataString(sessionId)}/leave", content);

        if (!resp.IsSuccessStatusCode)
        {
            string errorText = await resp.Content.ReadAsStringAsync();
            Console.WriteLine($"[LeaveLobby] Server error: {resp.StatusCode} — {errorText}");
            return null;
        }

        string responseJson = await resp.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<LeaveResponseDto>(responseJson)!;
    }
}