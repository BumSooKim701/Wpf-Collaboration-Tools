namespace CollaborationTools.user;

public class UserSession
{
    public static User? CurrentUser { get; private set; }

    public static bool IsLoggedIn
    {
        get { return CurrentUser != null; }
    }
    
    public static void Login(User user)
    {
        CurrentUser = user;
        Console.WriteLine($"Login User is {CurrentUser.Email}");
    }
    
    public static void Logout()
    {
        CurrentUser = null;
    }

}