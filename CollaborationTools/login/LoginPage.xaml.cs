using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CollaborationTools.authentication;
using CollaborationTools.database;
using CollaborationTools.team;
using CollaborationTools.user;

namespace CollaborationTools.login;

public partial class LoginPage : Page
{
    private readonly GoogleAuthentication _googleAuthentication = new();
    private readonly UserRepository _userRepository = new();
    private readonly TeamService _teamService = new();

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            LoginButton.IsEnabled = false;
            Debug.WriteLine("Google 로그인 중...");

            // OAuth 인증 및 사용자 정보 수집
            var googleUser = await _googleAuthentication.AuthenticateGoogleAsync();

            var dbUser = _userRepository.FindUserByEmail(googleUser.Email);

            if (dbUser == null)
            {
                var result = _userRepository.AddUser(googleUser);

                if (!result)
                    txtStatus.Text = "회원 등록 실패";
                else
                {
                    Team newTeam = new Team
                    {
                        teamName = googleUser.Email,
                        teamMemberCount = 1,
                        dateOfCreated = DateTime.Now,
                        teamCalendarName = googleUser.Name,
                        teamCalendarId = string.Empty,
                        teamDescription = " ",
                        teamFolderId = string.Empty,
                        visibility = 0
                    };
                    
                    bool teamSuccess =_teamService.CreateTeam(newTeam);
                    
                    if (teamSuccess)
                    {
                        Team primaryTeam = _teamService.FindTeamByUuid(newTeam.uuid);
                        User user = _userRepository.FindUserByEmail(googleUser.Email);
                        
                        _userRepository.UpdatePrimaryTeamId(user, primaryTeam.teamId);
                        _teamService.AddTeamMemberByEmail(primaryTeam, googleUser.Email, 1);
                        
                        user = _userRepository.FindUserByEmail(googleUser.Email);
                        NavigatePage(_userRepository.FindUserByEmail(user.Email));
                    }
                }
            }
            else
            {
                Console.WriteLine("Login Success");
                Console.WriteLine("email: " + googleUser.Email + ". name: " + googleUser.Name);
                
                NavigatePage(dbUser);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"로그인 실패: {ex.Message}", "오류",
                MessageBoxButton.OK, MessageBoxImage.Error);
            txtStatus.Text = "로그인 실패";
        }
        finally
        {
            LoginButton.IsEnabled = true;
        }
    }

    private void NavigatePage(User user)
    {
        UserSession.Login(user);
        Console.WriteLine($"Login User is {user.TeamId}");

        Page mainPage = new MainPage();
        NavigationService.Navigate(mainPage);
    }
}