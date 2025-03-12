using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace DetectiveMysteryGamePlatform.Client.Services
{
    public class GameHubService : IAsyncDisposable
    {
        private readonly NavigationManager _navigationManager;
        private HubConnection _hubConnection;
        
        public event Action<string, bool> OnPlayerConnectionUpdated;
        public event Action<int, string> OnRoundAdvanced;
        public event Action<string> OnContentRevealed;
        public event Action<string> OnGameStatusChanged;

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public GameHubService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        }

        public async Task StartConnection(string gameSessionId)
        {
            if (string.IsNullOrEmpty(gameSessionId))
                throw new ArgumentNullException(nameof(gameSessionId));

            if (_hubConnection == null)
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_navigationManager.ToAbsoluteUri("/gamehub"))
                    .WithAutomaticReconnect()
                    .Build();

                _hubConnection.On<string, bool>("PlayerConnectionUpdated", (playerName, isConnected) =>
                {
                    OnPlayerConnectionUpdated?.Invoke(playerName, isConnected);
                });

                _hubConnection.On<int, string>("RoundAdvanced", (newRoundNumber, newRoundTitle) =>
                {
                    OnRoundAdvanced?.Invoke(newRoundNumber, newRoundTitle);
                });

                _hubConnection.On<string>("ContentRevealed", (contentTitle) =>
                {
                    OnContentRevealed?.Invoke(contentTitle);
                });

                _hubConnection.On<string>("GameStatusChanged", (newStatus) =>
                {
                    OnGameStatusChanged?.Invoke(newStatus);
                });

                await _hubConnection.StartAsync();
            }

            await _hubConnection.InvokeAsync("JoinGameSession", gameSessionId);
        }

        public async Task StopConnection(string gameSessionId)
        {
            if (string.IsNullOrEmpty(gameSessionId))
                throw new ArgumentNullException(nameof(gameSessionId));

            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("LeaveGameSession", gameSessionId);
                await _hubConnection.StopAsync();
            }
        }

        public async Task UpdateConnectionStatus(string gameSessionId, string playerName, bool isConnected)
        {
            if (string.IsNullOrEmpty(gameSessionId))
                throw new ArgumentNullException(nameof(gameSessionId));

            if (string.IsNullOrEmpty(playerName))
                throw new ArgumentNullException(nameof(playerName));

            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("UpdateConnectionStatus", gameSessionId, playerName, isConnected);
            }
        }

        public async Task NotifyRoundAdvanced(string gameSessionId, int newRoundNumber, string newRoundTitle)
        {
            if (string.IsNullOrEmpty(gameSessionId))
                throw new ArgumentNullException(nameof(gameSessionId));

            if (string.IsNullOrEmpty(newRoundTitle))
                throw new ArgumentNullException(nameof(newRoundTitle));

            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("RoundAdvanced", gameSessionId, newRoundNumber, newRoundTitle);
            }
        }

        public async Task NotifyContentRevealed(string gameSessionId, string contentTitle)
        {
            if (string.IsNullOrEmpty(gameSessionId))
                throw new ArgumentNullException(nameof(gameSessionId));

            if (string.IsNullOrEmpty(contentTitle))
                throw new ArgumentNullException(nameof(contentTitle));

            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("ContentRevealed", gameSessionId, contentTitle);
            }
        }

        public async Task NotifyGameStatusChanged(string gameSessionId, string newStatus)
        {
            if (string.IsNullOrEmpty(gameSessionId))
                throw new ArgumentNullException(nameof(gameSessionId));

            if (string.IsNullOrEmpty(newStatus))
                throw new ArgumentNullException(nameof(newStatus));

            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("GameStatusChanged", gameSessionId, newStatus);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
            }
        }
    }
} 