using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LikeMovies.Services.Interfaces
{
    public interface IGoogleOAuthService
    {
        string GenerateAuthorizationUrl(string redirectUri);
        Task<string> ExchangeCodeForAccessTokenAsync(string code, string redirectUri);
        Task<dynamic> GetUserInfoAsync(string accessToken);
    }
}
