using System.Windows;

namespace CollaborationTools.login;

public partial class Login : Window
{
    public Login()
    {
        InitializeComponent();
    }

    private void btnLogin_Click(object sender, RoutedEventArgs e)
    {
        string userId = txtUserId.Text;
        string userPassword = txtPassword.Password;
    }

    private void lnkRegister_Click(object sender, RoutedEventArgs e)
    {
        
    }
}