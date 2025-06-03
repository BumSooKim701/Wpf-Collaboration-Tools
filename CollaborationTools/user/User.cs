namespace CollaborationTools.user;

public class User
{
    public string GoogleId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string PictureUri { get; set; }
    public string RefreshToken { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
}   