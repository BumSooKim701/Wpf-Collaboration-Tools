using CollaborationTools.database;
using CollaborationTools.user;

namespace CollaborationTools.address
{
    public class AddressBookService
    {
        private readonly UserRepository _userRepository = new();
        private readonly AddressRepository _addressRepository = new();
        private readonly TeamMemberRepository _teamMemberRepository = new();

        public AddressBookService()
        {
            _userRepository = new UserRepository();
        }

        public List<User> GetUserAddressBook(int userId)
        {
            try
            {
                return _userRepository.FindUsersInAddressBook(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetUserAddressBook Error: {ex.Message}");
                return new List<User>();
            }
        }

        public bool AddToAddressBook(int targetUserId)
        {
            try
            {
                return _addressRepository.AddAddress(targetUserId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddToAddressBook Error: {ex.Message}");
                return false;
            }
        }

        public bool RemoveFromAddressBook(int userId, int targetUserId)
        {
            try
            {
                return _addressRepository.RemoveAddress(userId, targetUserId);;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RemoveFromAddressBook Error: {ex.Message}");
                return false;
            }
        }
    }
}