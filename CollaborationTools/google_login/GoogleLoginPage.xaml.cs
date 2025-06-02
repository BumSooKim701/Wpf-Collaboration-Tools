using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace CollaborationTools.google_login;

public partial class GoogleLoginPage : Page
{
    GoogleAuthentication _googleAuthentication = new GoogleAuthentication();
    
    public GoogleLoginPage()
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
                
            Debug.WriteLine("로그인 성공! email: {0}. name: {1}", googleUser.Email, googleUser.Name);;
            
            Application.Current.Properties["GoogleUser"] = googleUser;
            NavigationService.Navigate(new MainPage());
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
}