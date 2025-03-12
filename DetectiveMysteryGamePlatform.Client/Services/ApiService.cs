using DetectiveMysteryGamePlatform.Shared.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DetectiveMysteryGamePlatform.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Authentication
        public async Task<string> Login(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Email = email, Password = password });
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result.Token;
        }

        // Quests
        public async Task<List<Quest>> GetQuests()
        {
            return await _httpClient.GetFromJsonAsync<List<Quest>>("api/quests");
        }

        public async Task<Quest> GetQuest(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<Quest>($"api/quests/{id}");
        }

        public async Task<Quest> CreateQuest(Quest quest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/quests", quest);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Quest>();
        }

        public async Task UpdateQuest(Quest quest)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/quests/{quest.Id}", quest);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteQuest(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/quests/{id}");
            response.EnsureSuccessStatusCode();
        }

        // Characters
        public async Task<List<Character>> GetCharactersByQuest(Guid questId)
        {
            return await _httpClient.GetFromJsonAsync<List<Character>>($"api/quests/{questId}/characters");
        }

        public async Task<Character> GetCharacter(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<Character>($"api/characters/{id}");
        }

        public async Task<Character> CreateCharacter(Guid questId, Character character)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/quests/{questId}/characters", character);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Character>();
        }

        public async Task UpdateCharacter(Character character)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/characters/{character.Id}", character);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCharacter(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/characters/{id}");
            response.EnsureSuccessStatusCode();
        }

        // Rounds
        public async Task<List<Round>> GetRoundsForQuest(Guid questId)
        {
            return await _httpClient.GetFromJsonAsync<List<Round>>($"api/quests/{questId}/rounds");
        }

        public async Task<Round> GetRound(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<Round>($"api/rounds/{id}");
        }

        public async Task<Round> CreateRound(Guid questId, Round round)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/quests/{questId}/rounds", round);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Round>();
        }

        public async Task UpdateRound(Round round)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/rounds/{round.Id}", round);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteRound(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/rounds/{id}");
            response.EnsureSuccessStatusCode();
        }

        // Game Sessions
        public async Task<List<GameSession>> GetGameSessions()
        {
            return await _httpClient.GetFromJsonAsync<List<GameSession>>("api/game-sessions");
        }

        public async Task<GameSession> GetGameSession(int id)
        {
            return await _httpClient.GetFromJsonAsync<GameSession>($"api/game-sessions/{id}");
        }

        public async Task<GameSession> CreateGameSession(Guid questId)
        {
            var response = await _httpClient.PostAsJsonAsync("api/game-sessions", new { QuestId = questId });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GameSession>();
        }

        public async Task StartGameSession(int sessionId)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/game-sessions/{sessionId}/status", 
                new { Status = GameSessionStatus.InProgress });
            response.EnsureSuccessStatusCode();
        }

        public async Task EndGameSession(int sessionId)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/game-sessions/{sessionId}/status", 
                new { Status = GameSessionStatus.Completed });
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> InvitePlayer(int gameSessionId, string email)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/game-sessions/{gameSessionId}/invite-player", new { Email = email });
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<InvitePlayerResponse>();
            return result.InvitationToken;
        }

        public async Task AdvanceRound(int sessionId, int roundNumber)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/game-sessions/{sessionId}/advance-round", new { RoundNumber = roundNumber });
            response.EnsureSuccessStatusCode();
        }

        public async Task AssignCharacterToPlayer(int sessionId, string playerName, int characterId)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/game-sessions/{sessionId}/assign-character", 
                new { PlayerName = playerName, CharacterId = characterId });
            response.EnsureSuccessStatusCode();
        }

        public async Task ToggleContentVisibility(int sessionId, int contentId)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/game-sessions/{sessionId}/toggle-content", 
                new { ContentId = contentId });
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<PlayerResponse>> GetPlayersInSession(int sessionId)
        {
            return await _httpClient.GetFromJsonAsync<List<PlayerResponse>>($"api/game-sessions/{sessionId}/players");
        }

        // Player
        public async Task<GameInfoResponse> GetGameInfo(string token)
        {
            return await _httpClient.GetFromJsonAsync<GameInfoResponse>($"api/player/{token}/game-info");
        }

        public async Task<CharacterResponse> GetPlayerCharacter(string token)
        {
            return await _httpClient.GetFromJsonAsync<CharacterResponse>($"api/player/{token}/character");
        }

        public async Task<List<CharacterResponse>> GetPublicCharacters(string token)
        {
            return await _httpClient.GetFromJsonAsync<List<CharacterResponse>>($"api/player/{token}/public-characters");
        }

        public async Task<RoundResponse> GetCurrentRound(string token)
        {
            return await _httpClient.GetFromJsonAsync<RoundResponse>($"api/player/{token}/current-round");
        }

        public async Task<List<ContentResponse>> GetRevealedContent(string token)
        {
            return await _httpClient.GetFromJsonAsync<List<ContentResponse>>($"api/player/{token}/revealed-content");
        }

        public async Task JoinGame(string token, string playerName)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/player/{token}/join", new { PlayerName = playerName });
            response.EnsureSuccessStatusCode();
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
    }

    public class InvitePlayerResponse
    {
        public string InvitationToken { get; set; }
    }

    public class GameInfoResponse
    {
        public string QuestTitle { get; set; }
        public string QuestDescription { get; set; }
        public GameSessionStatus GameSessionStatus { get; set; }
        public int CurrentRoundNumber { get; set; }
        public string CurrentRoundTitle { get; set; }
        public string PlayerName { get; set; }
        public bool IsConnected { get; set; }
        public bool HasCharacterAssigned { get; set; }
    }

    public class CharacterResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AvatarImagePath { get; set; }
    }

    public class RoundResponse
    {
        public int RoundNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<ContentResponse> PublicContent { get; set; }
        public List<ContentResponse> CharacterContent { get; set; }
    }

    public class ContentResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public ContentType Type { get; set; }
        public string ImagePath { get; set; }
    }

    public class PlayerResponse
    {
        public string Name { get; set; }
        public int AssignedCharacterId { get; set; }
        public bool IsConnected { get; set; }
    }
} 