namespace CollaborationTools.user;

public class User
{
    private string _userId { get; set; }
    private string _name { get; set; }
    private string _phoneNumber { get; set; }
    private string _email { get; set; }
    private DateTime _registrationDate { get; set; }
    
    public User(string userId, string name, string phoneNumber, string email, DateTime registrationDate)
    {
        _userId = userId;
        _name = name;
        _phoneNumber = phoneNumber;
        _email = email;
        _registrationDate = registrationDate;
    }
}   