using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CollaborationTools.authentication;
using CollaborationTools.database;
using CollaborationTools.user;

namespace CollaborationTools.login;

public partial class LoginPage : Page
{
    private readonly GoogleAuthentication _googleAuthentication = new();
    private readonly UserRepository _userRepository = new();

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
                    NavigatePage(_userRepository.FindUserByEmail(googleUser.Email));
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

        Page mainPage = new MainPage();
        NavigationService.Navigate(mainPage);
    }
}