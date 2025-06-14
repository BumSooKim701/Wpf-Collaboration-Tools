using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CollaborationTools.address;
using CollaborationTools.database;
using CollaborationTools.user;

namespace CollaborationTools.address 
{
    public partial class AddAddressWindow : Window
    {
        private readonly UserRepository _userRepository;
        private readonly AddressBookService _addressBookService;
        private ObservableCollection<User> _allUsers;
        private ObservableCollection<User> _filteredUsers;

        public event EventHandler<User> AddressAdded;

        public AddAddressWindow()
        {
            InitializeComponent();
            _userRepository = new UserRepository();
            _addressBookService = new AddressBookService();
            _allUsers = new ObservableCollection<User>();
            _filteredUsers = new ObservableCollection<User>();
            
            // 처음에는 빈 리스트로 시작
            UserListControl.ItemsSource = _filteredUsers;
            
            // 전체 사용자 목록은 미리 로드해두기
            LoadAllUsers();
        }

        private void LoadAllUsers()
        {
            try
            {
                Console.WriteLine("LoadAllUsers 시작");
                
                var users = _userRepository.GetAllUsers();
                Console.WriteLine($"DB에서 불러온 사용자 수: {users.Count}");
                
                var currentUserAddresses = _addressBookService.GetUserAddressBook(UserSession.CurrentUser.userId);
                var currentUserAddressIds = currentUserAddresses.Select(u => u.userId).ToHashSet();
                
                _allUsers.Clear();
                
                foreach (var user in users)
                {
                    // 현재 사용자와 이미 주소록에 있는 사용자는 제외
                    if (user.userId != UserSession.CurrentUser.userId && 
                        !currentUserAddressIds.Contains(user.userId))
                    {
                        _allUsers.Add(user);
                        Console.WriteLine($"로드된 사용자: {user.Name} ({user.Email})");
                    }
                }
                
                Console.WriteLine($"검색 가능한 사용자 수: {_allUsers.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadAllUsers Error: {ex.Message}");
                MessageBox.Show($"사용자 목록을 불러오는 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private void FilterUsers(string searchText)
        {
            // 검색어가 비어있으면 빈 리스트 표시
            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredUsers.Clear();
                UserListControl.ItemsSource = _filteredUsers;
                Console.WriteLine("검색어가 비어있음 - 빈 리스트 표시");
                return;
            }

            // 검색어가 있을 때만 필터링해서 표시
            var filtered = _allUsers.Where(user => 
                (user.Name?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true) ||
                (user.Email?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true)).ToList();
            
            _filteredUsers.Clear();
            foreach (var user in filtered)
            {
                _filteredUsers.Add(user);
            }
            
            UserListControl.ItemsSource = _filteredUsers;
            
            // 디버깅용 로그
            Console.WriteLine($"검색어: '{searchText}', 검색된 사용자: {_filteredUsers.Count}명");
        }

        private void UserSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                FilterUsers(textBox.Text);
            }
        }

        private void AddUserClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is User user)
            {
                AddressAdded?.Invoke(this, user);
                
                // 추가된 사용자를 목록에서 제거
                _allUsers.Remove(user);
                _filteredUsers.Remove(user);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}