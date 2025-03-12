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
            await Groups.AddToGroupAsync(Context.ConnectionId, gameSessionId);
        }

        // Remove player from a game session group
        public async Task LeaveGameSession(string gameSessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameSessionId);
        }

        // Update player connection status
        public async Task UpdateConnectionStatus(string gameSessionId, string playerName, bool isConnected)
        {
            await Clients.Group(gameSessionId).SendAsync("PlayerConnectionUpdated", playerName, isConnected);
        }

        // Notify when a round advances
        public async Task RoundAdvanced(string gameSessionId, int newRoundNumber, string newRoundTitle)
        {
            await Clients.Group(gameSessionId).SendAsync("RoundAdvanced", newRoundNumber, newRoundTitle);
        }

        // Notify when new content is revealed
        public async Task ContentRevealed(string gameSessionId, string contentTitle)
        {
            await Clients.Group(gameSessionId).SendAsync("ContentRevealed", contentTitle);
        }

        // Notify when game status changes
        public async Task GameStatusChanged(string gameSessionId, string newStatus)
        {
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