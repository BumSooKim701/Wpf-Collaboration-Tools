using System.Windows;
using CollaborationTools.database;
using CollaborationTools.user;
using MySqlConnector;

namespace CollaborationTools.login;

public partial class LoginService : Window
{
    private ConnectionPool _connectionPool = ConnectionPool.GetInstance();
    public LoginService()
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

    private bool CheckLogin(string userId, string userPassword)
    {
        
        
        return true;
    }
}