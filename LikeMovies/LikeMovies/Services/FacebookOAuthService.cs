using Facebook; 
using LikeMovies.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace LikeMovies.Services
{
    public class FacebookOAuthService : IFacebookOAuthService
    {
        private readonly string _appId;
        private readonly string _appSecret;
        private static readonly HttpClient _httpClient = new HttpClient();

        public FacebookOAuthService()
        {
            _appId = ConfigurationManager.AppSettings["FacebookAppId"];
            _appSecret = ConfigurationManager.AppSettings["FacebookAppSecret"];
            if (string.IsNullOrEmpty(_appId) || string.IsNullOrEmpty(_appSecret))
            {
                throw new InvalidOperationException("Facebook AppId hoặc AppSecret chưa được cấu hình trong Web.config.");
            }
        }

        public string GenerateAuthorizationUrl(string redirectUri)
        {
            return $"https://www.facebook.com/v21.0/dialog/oauth?client_id={_appId}&redirect_uri={HttpUtility.UrlEncode(redirectUri)}&scope=email,public_profile";
        }

        public async Task<string> ExchangeCodeForAccessTokenAsync(string code, string redirectUri)
        {
            var tokenUrl = $"https://graph.facebook.com/v21.0/oauth/access_token?client_id={_appId}&redirect_uri={HttpUtility.UrlEncode(redirectUri)}&client_secret={_appSecret}&code={code}";

            var responseString = await _httpClient.GetStringAsync(tokenUrl);
            dynamic tokenData = JsonConvert.DeserializeObject<dynamic>(responseString);

            if (tokenData == null || tokenData.access_token == null)
            {
                var errorMessage = tokenData?.error?.message?.ToString() ?? "Không xác định";
                throw new Exception($"Không thể lấy access token từ Facebook. Lỗi: {errorMessage}");
            }
            return tokenData.access_token.ToString();
        }

        public async Task<dynamic> GetUserInfoAsync(string accessToken)
        {
            var fbClient = new FacebookClient(accessToken);
            dynamic fbUser = await fbClient.GetTaskAsync("me?fields=id,name,email,picture.type(large)");

            if (fbUser == null)
            {
                throw new Exception("Không thể lấy thông tin người dùng từ Facebook.");
            }
            return fbUser;
        }
    }
}