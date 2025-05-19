using LikeMovies.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace LikeMovies.Services
{
    public class GoogleOAuthService : IGoogleOAuthService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private static readonly HttpClient _httpClient = new HttpClient();

        public GoogleOAuthService()
        {
            _clientId = ConfigurationManager.AppSettings["GoogleClientId"];
            _clientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"];
            if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret))
            {
                throw new InvalidOperationException("Google ClientId hoặc ClientSecret chưa được cấu hình trong Web.config.");
            }
        }

        public string GenerateAuthorizationUrl(string redirectUri)
        {
            string googleOAuthUrl = "https://accounts.google.com/o/oauth2/v2/auth";
            var queryParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("response_type", "code"),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("scope", "openid email profile"),
                new KeyValuePair<string, string>("access_type", "online")
            };
            string queryString = string.Join("&", queryParams.Select(q => $"{q.Key}={Uri.EscapeDataString(q.Value)}"));
            return $"{googleOAuthUrl}?{queryString}";
        }

        public async Task<string> ExchangeCodeForAccessTokenAsync(string code, string redirectUri)
        {
            string tokenEndpoint = "https://oauth2.googleapis.com/token";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);
            if (jsonResponse.error != null)
            {
                throw new Exception($"Lỗi khi đổi code lấy token Google: {jsonResponse.error_description}");
            }
            return jsonResponse.access_token;
        }

        public async Task<dynamic> GetUserInfoAsync(string accessToken)
        {
            string userInfoEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, userInfoEndpoint);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(requestMessage);
            var responseString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            dynamic googleUser = JsonConvert.DeserializeObject(responseString);
            if (googleUser.error != null)
            {
                throw new Exception($"Lỗi khi lấy thông tin người dùng Google: {googleUser.error.message}");
            }
            return googleUser; // Trả về đối tượng dynamic
        }
    }
}