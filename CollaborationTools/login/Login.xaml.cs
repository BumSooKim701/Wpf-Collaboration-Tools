using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CollaborationTools.authentication;
using CollaborationTools.database;
using CollaborationTools.user;

namespace CollaborationTools.login;

public partial class Login : Page
{
    private LoginRepository loginRepository = new LoginRepository();
    GoogleAuthentication _googleAuthentication = new GoogleAuthentication();

    public Login()
    {
        InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            LoginButton.IsEnabled = false;
            Debug.Print("Google 로그인 중...");

            // OAuth 인증 및 사용자 정보 수집
            var googleUser = await _googleAuthentication.AuthenticateGoogleAsync();

            Debug.Print("로그인 성공!");
            Debug.Print("email: " + googleUser.Email + ". name: " + googleUser.Name);

            Page MainPage = new MainPage();
            NavigationService.Navigate(MainPage);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"로그인 실패: {ex.Message}", "오류",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Debug.Print("로그인 실패");
        }
        finally
        {
            LoginButton.IsEnabled = true;
        }
    }

    private void lnkRegister_Click(object sender, RoutedEventArgs e)
    {

    }

    private bool CheckLogin(string userId, string userPassword)
    {
        Console.WriteLine(userId);
        Console.WriteLine(userPassword);
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userPassword))
        {
            Console.WriteLine("Id나 비번 입력값 없음");
            return false;
        }

        User user = loginRepository.CheckUserId(userId);

        if (user == null)
        {
            Console.WriteLine("존재하지 않는 사용자");
            return false;
        }

        return true;
    }
}