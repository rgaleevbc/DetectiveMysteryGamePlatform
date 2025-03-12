using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace DetectiveMysteryGamePlatform.Client.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public CustomAuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);

            return new AuthenticationState(new ClaimsPrincipal(
                new ClaimsIdentity(ParseClaimsFromJwt(savedToken), "jwt")));
        }

        public void MarkUserAsAuthenticated(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            var authenticatedUser = new ClaimsPrincipal(
                new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            
            _httpClient.DefaultRequestHeaders.Authorization = null;
            
            NotifyAuthenticationStateChanged(authState);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
                throw new ArgumentNullException(nameof(jwt));

            var claims = new List<Claim>();
            var parts = jwt.Split('.');
            if (parts.Length != 3)
                throw new ArgumentException("Invalid JWT token format", nameof(jwt));

            var payload = parts[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes)
                ?? throw new InvalidOperationException("Failed to deserialize JWT payload");

            if (keyValuePairs.TryGetValue("role", out object roles))
            {
                var rolesStr = roles.ToString();
                if (!string.IsNullOrEmpty(rolesStr))
                {
                    if (rolesStr.Trim().StartsWith("["))
                    {
                        var parsedRoles = JsonSerializer.Deserialize<string[]>(rolesStr);
                        if (parsedRoles != null)
                        {
                            foreach (var parsedRole in parsedRoles)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                            }
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, rolesStr));
                    }
                }

                keyValuePairs.Remove("role");
            }

            claims.AddRange(keyValuePairs.Select(kvp => 
                new Claim(kvp.Key, kvp.Value?.ToString() ?? string.Empty)));

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
                throw new ArgumentNullException(nameof(base64));

            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
} 