using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace DetectiveMysteryGamePlatform.Api.Hubs
{
    public class GameHub : Hub
    {
        // Add player to a game session group
        public async Task JoinGameSession(string gameSessionId)
        {
            if (string.IsNullOrWhiteSpace(gameSessionId))
            {
                throw new ArgumentException("Game session ID is required", nameof(gameSessionId));
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, gameSessionId);
        }

        // Remove player from a game session group
        public async Task LeaveGameSession(string gameSessionId)
        {
            if (string.IsNullOrWhiteSpace(gameSessionId))
            {
                throw new ArgumentException("Game session ID is required", nameof(gameSessionId));
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameSessionId);
        }

        // Update player connection status
        public async Task UpdateConnectionStatus(string gameSessionId, string playerName, bool isConnected)
        {
            if (string.IsNullOrWhiteSpace(gameSessionId))
            {
                throw new ArgumentException("Game session ID is required", nameof(gameSessionId));
            }

            if (string.IsNullOrWhiteSpace(playerName))
            {
                throw new ArgumentException("Player name is required", nameof(playerName));
            }

            await Clients.Group(gameSessionId).SendAsync("PlayerConnectionUpdated", playerName, isConnected);
        }

        // Notify when a round advances
        public async Task RoundAdvanced(string gameSessionId, int newRoundNumber, string newRoundTitle)
        {
            if (string.IsNullOrWhiteSpace(gameSessionId))
            {
                throw new ArgumentException("Game session ID is required", nameof(gameSessionId));
            }

            if (newRoundNumber <= 0)
            {
                throw new ArgumentException("Round number must be greater than 0", nameof(newRoundNumber));
            }

            if (string.IsNullOrWhiteSpace(newRoundTitle))
            {
                throw new ArgumentException("Round title is required", nameof(newRoundTitle));
            }

            await Clients.Group(gameSessionId).SendAsync("RoundAdvanced", newRoundNumber, newRoundTitle);
        }

        // Notify when new content is revealed
        public async Task ContentRevealed(string gameSessionId, string contentTitle)
        {
            if (string.IsNullOrWhiteSpace(gameSessionId))
            {
                throw new ArgumentException("Game session ID is required", nameof(gameSessionId));
            }

            if (string.IsNullOrWhiteSpace(contentTitle))
            {
                throw new ArgumentException("Content title is required", nameof(contentTitle));
            }

            await Clients.Group(gameSessionId).SendAsync("ContentRevealed", contentTitle);
        }

        // Notify when game status changes
        public async Task GameStatusChanged(string gameSessionId, string newStatus)
        {
            if (string.IsNullOrWhiteSpace(gameSessionId))
            {
                throw new ArgumentException("Game session ID is required", nameof(gameSessionId));
            }

            if (string.IsNullOrWhiteSpace(newStatus))
            {
                throw new ArgumentException("New status is required", nameof(newStatus));
            }

            await Clients.Group(gameSessionId).SendAsync("GameStatusChanged", newStatus);
        }

        // Handle disconnection
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // In a real implementation, update player connection status in the database
            await base.OnDisconnectedAsync(exception);
        }
    }
} 