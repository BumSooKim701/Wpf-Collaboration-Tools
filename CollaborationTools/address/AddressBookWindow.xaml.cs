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
    public partial class AddressBookWindow : Window
    {
        private readonly UserRepository _userRepository;
        private readonly AddressBookService _addressBookService;
        private ObservableCollection<User> _addressBookItems;
        private ObservableCollection<User> _filteredItems;

        public AddressBookWindow()
        {
            InitializeComponent();
            _userRepository = new UserRepository();
            _addressBookService = new AddressBookService();
            _addressBookItems = new ObservableCollection<User>();
            _filteredItems = new ObservableCollection<User>();
            
            LoadAddressBook();
        }

        private void LoadAddressBook()
        {
            try
            {
                var addresses = _addressBookService.GetUserAddressBook(UserSession.CurrentUser.userId);
                _addressBookItems.Clear();
                
                foreach (var address in addresses)
                {
                    _addressBookItems.Add(address);
                }
                
                _filteredItems = new ObservableCollection<User>(_addressBookItems);
                AddressListControl.ItemsSource = _filteredItems;
                
                UpdateNoAddressMessage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"주소록을 불러오는 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private void FilterAddresses(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredItems = new ObservableCollection<User>(_addressBookItems);
            }
            else
            {
                var filtered = _addressBookItems.Where(user => 
                    user.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    user.Email.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
                
                _filteredItems = new ObservableCollection<User>(filtered);
            }
            
            AddressListControl.ItemsSource = _filteredItems;
            UpdateNoAddressMessage();
        }

        private void UpdateNoAddressMessage()
        {
            NoAddressMessage.Visibility = _filteredItems.Count == 0 ? Visibility.Visible : Visibility.Hidden;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                FilterAddresses(textBox.Text);
            }
        }

        private void AddAddressClick(object sender, RoutedEventArgs e)
        {
            var addAddressWindow = new AddAddressWindow();
            addAddressWindow.Owner = Application.Current.MainWindow;
            addAddressWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            
            addAddressWindow.AddressAdded += OnAddressAdded;
            addAddressWindow.Show();
        }

        private void OnAddressAdded(object sender, User addedUser)
        {
            try
            {
                bool success = _addressBookService.AddToAddressBook(addedUser.userId);
                
                if (success)
                {
                    _addressBookItems.Add(addedUser);
                    FilterAddresses(SearchTextBox.Text);
                    MessageBox.Show($"{addedUser.Name}님이 주소록에 추가되었습니다.");
                }
                else
                {
                    MessageBox.Show("주소록 추가에 실패했습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"주소록 추가 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private void CopyEmailClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is User user)
            {
                try
                {
                    Clipboard.SetText(user.Email);
                    MessageBox.Show($"{user.Email}이 클립보드에 복사되었습니다.", "복사 완료", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"클립보드 복사 중 오류가 발생했습니다: {ex.Message}");
                }
            }
        }

        private void RemoveAddressClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is User user)
            {
                var result = MessageBox.Show($"{user.Name}님을 주소록에서 제거하시겠습니까?", 
                    "주소록 제거", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bool success = _addressBookService.RemoveFromAddressBook(UserSession.CurrentUser.userId, user.userId);
                        
                        if (success)
                        {
                            _addressBookItems.Remove(user);
                            FilterAddresses(SearchTextBox.Text);
                            MessageBox.Show($"{user.Name}님이 주소록에서 제거되었습니다.");
                        }
                        else
                        {
                            MessageBox.Show("주소록 제거에 실패했습니다.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"주소록 제거 중 오류가 발생했습니다: {ex.Message}");
                    }
                }
            }
        }

        private void AddressItemClick(object sender, MouseButtonEventArgs e)
        {
            // 주소 아이템 클릭 시 추가 동작이 필요하면 여기에 구현
        }
    }
}
