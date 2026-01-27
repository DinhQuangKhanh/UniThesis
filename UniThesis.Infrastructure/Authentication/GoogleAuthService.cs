using Microsoft.Extensions.Options;

namespace UniThesis.Infrastructure.Authentication
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly GoogleAuthSettings _settings;
        private readonly HttpClient _httpClient;
    
        public GoogleAuthService(IOptions<GoogleAuthSettings> settings, HttpClient httpClient)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
        }
    
        public async Task<GoogleUserInfo?> ValidateGoogleTokenAsync(string idToken)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");
    
                if (!response.IsSuccessStatusCode) return null;
    
                var content = await response.Content.ReadAsStringAsync();
                var payload = System.Text.Json.JsonSerializer.Deserialize<GoogleTokenPayload>(content);
    
                if (payload is null || payload.Aud != _settings.ClientId) return null;
    
                return new GoogleUserInfo(payload.Email, payload.Name, payload.Picture, payload.Sub);
            }
            catch
            {
                return null;
            }
        }
    
        private class GoogleTokenPayload
        {
            public string Sub { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string? Picture { get; set; }
            public string Aud { get; set; } = string.Empty;
        }
    }
}
