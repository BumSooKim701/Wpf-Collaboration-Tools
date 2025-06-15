using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using CollaborationTools.user;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json.Linq;

namespace CollaborationTools.authentication;

public class GoogleAuthentication
{
    private static readonly string[] Scopes =
    [
        CalendarService.Scope.Calendar,
        DriveService.Scope.Drive,
        "https://www.googleapis.com/auth/userinfo.profile",
        "https://www.googleapis.com/auth/userinfo.email",
        "https://www.googleapis.com/auth/drive.file"
    ];

    private static readonly string ApplicationName = "CollaborationTools App";
    public static CalendarService CalendarService;
    public static DriveService DriveService;
    private UserCredential _credential;
    private User? user;

    public async Task<User> AuthenticateGoogleAsync()
    {
        try
        {
            var credentialPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credential/client_secret.json");
            using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
            {
                var tokenPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credential/token/");

                _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(tokenPath, true));
            }

            // Calendar 서비스 초기화
            CalendarService = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = _credential,
                ApplicationName = ApplicationName
            });

            // Drive 서비스 초기화
            DriveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = _credential,
                ApplicationName = ApplicationName
            });

            // 사용자 정보 가져오기
            user = await GetUserInfoAsync();

            // 리프레시 토큰 저장 (중요: 첫 인증시에만 제공됨)
            user.RefreshToken = _credential.Token.RefreshToken;

            return user;
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

    private async Task<User> GetUserInfoAsync()
    {
        using (var httpClient = new HttpClient())
        {
            var accessToken = await _credential.GetAccessTokenForRequestAsync();

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.GetAsync(
                $"https://www.googleapis.com/oauth2/v3/userinfo?access_token={accessToken}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var userInfoJson = JObject.Parse(json);

                return new User
                {
                    GoogleId = userInfoJson["sub"]?.ToString(),
                    Email = userInfoJson["email"]?.ToString(),
                    Name = userInfoJson["name"]?.ToString(),
                    PictureUri = userInfoJson["picture"]?.ToString(),
                    CreatedAt = DateTime.Now,
                    LastLoginAt = DateTime.Now
                };
            }

            throw new Exception("사용자 정보를 가져올 수 없습니다.");
        }
    }

    public User getUserInfo()
    {
        return user;
    }
}