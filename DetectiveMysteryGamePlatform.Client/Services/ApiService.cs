using DetectiveMysteryGamePlatform.Shared.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Linq;

namespace DetectiveMysteryGamePlatform.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        // Authentication
        public async Task<string> Login(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Email = email, Password = password });
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result?.Token ?? throw new InvalidOperationException("Login response token is null");
        }

        // Quests
        public async Task<List<Quest>> GetQuests()
        {
            var quests = await _httpClient.GetFromJsonAsync<List<Quest>>("api/quests");
            return quests ?? new List<Quest>();
        }

        public async Task<Quest> GetQuest(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<Quest>($"api/quests/{id}");
        }

        public async Task<Quest> CreateQuest(Quest quest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/quests", quest);
            response.EnsureSuccessStatusCode();
            var createdQuest = await response.Content.ReadFromJsonAsync<Quest>();
            return createdQuest ?? throw new InvalidOperationException("Created quest is null");
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
            var characters = await _httpClient.GetFromJsonAsync<List<Character>>($"api/quests/{questId}/characters");
            return characters ?? new List<Character>();
        }

        public async Task<List<CharacterResponse>> GetQuestCharacters(Guid questId)
        {
            var characters = await GetCharactersByQuest(questId);
            // Convert from Character to CharacterResponse
            return characters.Select(c => new CharacterResponse
            {
                Id = c.Id,
                Name = c.Name ?? string.Empty,
                Description = c.Description ?? string.Empty,
                AvatarImagePath = c.AvatarImagePath ?? string.Empty,
                IsAssigned = false // Default, will be updated elsewhere
            }).ToList();
        }

        public async Task<Character?> GetCharacter(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<Character>($"api/characters/{id}");
        }

        public async Task<Character> CreateCharacter(Guid questId, Character character)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/quests/{questId}/characters", character);
            response.EnsureSuccessStatusCode();
            var createdCharacter = await response.Content.ReadFromJsonAsync<Character>();
            return createdCharacter ?? throw new InvalidOperationException("Created character is null");
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
            var rounds = await _httpClient.GetFromJsonAsync<List<Round>>($"api/quests/{questId}/rounds");
            return rounds ?? new List<Round>();
        }

        public async Task<List<RoundResponse>> GetQuestRounds(Guid questId)
        {
            var rounds = await GetRoundsForQuest(questId);
            // Convert from Round to RoundResponse
            return rounds.Select(r => new RoundResponse
            {
                RoundNumber = r.Number,
                Title = r.Title ?? string.Empty,
                Description = r.Description ?? string.Empty,
                PublicContent = new List<ContentResponse>(),
                CharacterContent = new List<ContentResponse>()
            }).ToList();
        }

        public async Task<Round?> GetRound(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<Round>($"api/rounds/{id}");
        }

        public async Task<Round> CreateRound(Guid questId, Round round)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/quests/{questId}/rounds", round);
            response.EnsureSuccessStatusCode();
            var createdRound = await response.Content.ReadFromJsonAsync<Round>();
            return createdRound ?? throw new InvalidOperationException("Created round is null");
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
            var sessions = await _httpClient.GetFromJsonAsync<List<GameSession>>("api/game-sessions");
            return sessions ?? new List<GameSession>();
        }

        public async Task<GameSessionResponse> GetGameSession(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<GameSession>($"api/game-sessions/{id}");
            if (response == null)
                throw new InvalidOperationException("Game session not found");

            // Convert from GameSession to GameSessionResponse
            return new GameSessionResponse
            {
                Id = response.Id,
                QuestId = response.QuestId,
                QuestTitle = "Quest Title", // This would be filled from actual data
                Status = response.Status,
                CreatedAt = response.CreatedAt,
                StartedAt = response.StartedAt,
                EndedAt = response.EndedAt,
                CurrentRound = response.CurrentRound,
                MaxPlayers = 10, // This would be determined elsewhere
                Players = new List<PlayerSessionResponse>() // Would be populated elsewhere
            };
        }

        public async Task<GameSession> CreateGameSession(Guid questId)
        {
            var response = await _httpClient.PostAsJsonAsync("api/game-sessions", new { QuestId = questId });
            response.EnsureSuccessStatusCode();
            var createdSession = await response.Content.ReadFromJsonAsync<GameSession>();
            return createdSession ?? throw new InvalidOperationException("Created game session is null");
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
            return result?.InvitationToken ?? throw new InvalidOperationException("Invitation token is null");
        }

        public async Task AdvanceRound(int sessionId, int roundNumber)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/game-sessions/{sessionId}/advance-round", new { RoundNumber = roundNumber });
            response.EnsureSuccessStatusCode();
        }

        public async Task AssignCharacterToPlayer(int sessionId, string playerName, Guid characterId)
        {
            await _httpClient.PostAsJsonAsync($"api/game-sessions/{sessionId}/assign-character", new
            {
                PlayerName = playerName,
                CharacterId = characterId
            });
        }

        public async Task ToggleContentVisibility(int sessionId, Guid contentId)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/game-sessions/{sessionId}/toggle-content",
                new { ContentId = contentId });
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<PlayerResponse>> GetPlayersInSession(int sessionId)
        {
            var players = await _httpClient.GetFromJsonAsync<List<PlayerResponse>>($"api/game-sessions/{sessionId}/players");
            return players ?? new List<PlayerResponse>();
        }

        // Player
        public async Task<GameInfoResponse?> GetGameInfo(string token)
        {
            return await _httpClient.GetFromJsonAsync<GameInfoResponse>($"api/player/{token}/game-info");
        }

        public async Task<CharacterResponse?> GetPlayerCharacter(string token)
        {
            return await _httpClient.GetFromJsonAsync<CharacterResponse>($"api/player/{token}/character");
        }

        public async Task<List<CharacterResponse>?> GetPublicCharacters(string token)
        {
            return await _httpClient.GetFromJsonAsync<List<CharacterResponse>>($"api/player/{token}/public-characters");
        }

        public async Task<RoundResponse?> GetCurrentRound(string token)
        {
            return await _httpClient.GetFromJsonAsync<RoundResponse>($"api/player/{token}/current-round");
        }

        public async Task<string> GetGameSessionId(string token)
        {
            var response = await _httpClient.GetFromJsonAsync<GameSessionIdResponse>($"api/player/{token}/session-id");
            return response?.SessionId ?? throw new InvalidOperationException("Session ID not found");
        }

        public async Task JoinGame(string token, string playerName)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/player/{token}/join", new { PlayerName = playerName });
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<ContentResponse>?> GetRevealedContent(string token)
        {
            return await _httpClient.GetFromJsonAsync<List<ContentResponse>>($"api/player/{token}/revealed-content");
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }

    public class InvitePlayerResponse
    {
        public string InvitationToken { get; set; } = string.Empty;
    }

    public class GameSessionIdResponse
    {
        public string SessionId { get; set; } = string.Empty;
    }

    public class GameInfoResponse
    {
        public string QuestTitle { get; set; } = string.Empty;
        public string QuestDescription { get; set; } = string.Empty;
        public GameSessionStatus GameSessionStatus { get; set; }
        public int CurrentRoundNumber { get; set; }
        public string CurrentRoundTitle { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public bool IsConnected { get; set; }
        public bool HasCharacterAssigned { get; set; }
    }

    public class CharacterResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AvatarImagePath { get; set; } = string.Empty;
        public bool IsAssigned { get; set; }
    }

    public class RoundResponse
    {
        public int RoundNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<ContentResponse> PublicContent { get; set; } = new();
        public List<ContentResponse> CharacterContent { get; set; } = new();
    }

    public class ContentResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public ContentType Type { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public bool IsRevealed { get; set; }
        public Guid? CharacterId { get; set; }
    }

    public class PlayerResponse
    {
        public string Name { get; set; } = string.Empty;
        public Guid AssignedCharacterId { get; set; }
        public bool IsConnected { get; set; }
    }
}