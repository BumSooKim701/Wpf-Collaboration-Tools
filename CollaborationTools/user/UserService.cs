using CollaborationTools.database;

namespace CollaborationTools.user;

public class UserService
{
    private readonly UserRepository _userRepository = new UserRepository();

    public bool IsExistUser(string email)
    {
        return _userRepository.FindUserByEmail(email) != null;
    }
}