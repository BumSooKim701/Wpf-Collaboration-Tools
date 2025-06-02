using System.IO;
using System.Net.Http;
using CollaborationTools.google_user;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json.Linq;

namespace CollaborationTools.google_login;

public class GoogleAuthentication
{
    private static readonly string[] Scopes =
    [
        CalendarService.Scope.Calendar,
        "https://www.googleapis.com/auth/userinfo.profile",
        "https://www.googleapis.com/auth/userinfo.email"
    ];
    private static readonly string ApplicationName = "CollaborationTools App";
    public static CalendarService CalendarService;
    private UserCredential _credential;
    private GoogleUserInfo? userInfo;
    
    public async Task<GoogleUserInfo> AuthenticateGoogleAsync()
    {
        try
        {
            string credentialPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credential/client_secret.json");
            using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
            {
                string tokenPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credential/token.json");
                    
                _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("tokenPath", true));
            }

            // Calendar 서비스 초기화
            CalendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = ApplicationName,
            });
                
            // 사용자 정보 가져오기
            userInfo = await GetUserInfoAsync();
        
            // 리프레시 토큰 저장 (중요: 첫 인증시에만 제공됨)
            userInfo.RefreshToken = _credential.Token.RefreshToken;
        
            return userInfo;
        }
        catch (FileNotFoundException)
        {
            throw new Exception("credentials.json 파일을 찾을 수 없습니다.");
        }
        catch (Exception ex)
        {
            throw new Exception($"인증 실패: {ex.Message}");
        }
    }
    
    private async Task<GoogleUserInfo> GetUserInfoAsync()
    {
        using (var httpClient = new HttpClient())
        {
            var accessToken = await _credential.GetAccessTokenForRequestAsync();
                
            httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            
            var response = await httpClient.GetAsync(
                $"https://www.googleapis.com/oauth2/v3/userinfo?access_token={accessToken}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var userInfoJson = JObject.Parse(json);

                return new GoogleUserInfo
                {
                    GoogleId = userInfoJson["sub"]?.ToString(),
                    Email = userInfoJson["email"]?.ToString(),
                    Name = userInfoJson["name"]?.ToString(),
                    GivenName = userInfoJson["given_name"]?.ToString(),
                    FamilyName = userInfoJson["family_name"]?.ToString(),
                    PictureUrl = userInfoJson["picture"]?.ToString(),
                    CreatedAt = DateTime.Now,
                    LastLoginAt = DateTime.Now
                };
            }

            throw new Exception("사용자 정보를 가져올 수 없습니다.");
        }
    }

    public GoogleUserInfo getUserInfo()
    {
        return userInfo;
    }
}