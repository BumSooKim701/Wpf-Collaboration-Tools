using CollaborationTools.database;
using CollaborationTools.team;

namespace CollaborationTools.user;

public class UserService
{
    private readonly UserRepository _userRepository = new();
    private TeamMemberRepository _teamMemberRepository = new();

    public bool IsExistUser(string email)
    {
        return _userRepository.FindUserByEmail(email) != null;
    }

    public List<User?>? FindUsersByTeamMembers(List<TeamMember> teamMembers)
    {
        var users = new List<User?>();

        foreach (var member in teamMembers)
        {
            var user = _userRepository.FindUserById(member.userId);
            Console.WriteLine($"Find User is {user.Email}");
            users.Add(user);
        }

        return users;
    }
}