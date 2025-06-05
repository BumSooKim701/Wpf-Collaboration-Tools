using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CollaborationTools.authentication;
using CollaborationTools.database;
using CollaborationTools.user;

namespace CollaborationTools.login;

public partial class LoginPage : Page
{
    private GoogleAuthentication _googleAuthentication = new GoogleAuthentication();
    private UserRepository _userRepository = new UserRepository(); 

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

            User? dbUser = _userRepository.FindUserByEmail(googleUser.Email);

            if (dbUser == null)
            {
                bool result = _userRepository.AddUser(googleUser);

                if (!result)
                {
                    txtStatus.Text = "회원 등록 실패";
                }
                else
                {
                    Page MainPage = new MainPage();
                    NavigationService.Navigate(MainPage);
                }
            }
            else
            {
                Debug.WriteLine("로그인 성공!");
                Debug.WriteLine("email: " + googleUser.Email + ". name: " + googleUser.Name);

                Page MainPage = new MainPage();
                NavigationService.Navigate(MainPage);
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
}