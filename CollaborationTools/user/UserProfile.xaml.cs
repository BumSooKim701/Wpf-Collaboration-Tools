using System;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using CollaborationTools.user;
using CollaborationTools.team;
using CollaborationTools.memo;
using CollaborationTools.database;
using CollaborationTools.login;

namespace CollaborationTools.profile
{
    public partial class UserProfile : Window, INotifyPropertyChanged
    {
        public EventHandler? LogoutRequested;
        private UserRepository userRepository = new();
        private TeamRepository teamRepository = new();
        private MemoService memoService = new();
        
        private User? currentUser;
        private BitmapImage? profileImageSource;
        private string userName = "알 수 없음";
        private string userEmail = "알 수 없음";
        private string userId = "0";
        private string googleId = "알 수 없음";
        private string createdAt = "";
        private string lastLoginAt = "";
        private string primaryTeamName = "없음";
        private int teamCount = 0;
        private int memoCount = 0;
        private int fileCount = 0;
        private int scheduleCount = 0;

        public UserProfile()
        {
            InitializeComponent();
            DataContext = this;
            
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                SetDefaultValues();
                return;
            }
            
            InitializeServices();
            LoadUserProfile();
        }
        
        private void InitializeServices()
        {
            try
            {
                userRepository = new UserRepository();
                teamRepository = new TeamRepository();
                memoService = new MemoService();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"서비스 초기화 실패: {ex.Message}");
                MessageBox.Show("서비스 초기화에 실패했습니다. 애플리케이션을 다시 시작해주세요.", 
                    "초기화 오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public BitmapImage? ProfileImageSource
        {
            get => profileImageSource;
            set
            {
                profileImageSource = value;
                OnPropertyChanged();
            }
        }

        public string UserName
        {
            get => userName;
            set
            {
                userName = value;
                OnPropertyChanged();
            }
        }

        public string UserEmail
        {
            get => userEmail;
            set
            {
                userEmail = value;
                OnPropertyChanged();
            }
        }

        public string UserId
        {
            get => userId;
            set
            {
                userId = value;
                OnPropertyChanged();
            }
        }

        public string GoogleId
        {
            get => googleId;
            set
            {
                googleId = value;
                OnPropertyChanged();
            }
        }

        public string CreatedAt
        {
            get => createdAt;
            set
            {
                createdAt = value;
                OnPropertyChanged();
            }
        }

        public string LastLoginAt
        {
            get => lastLoginAt;
            set
            {
                lastLoginAt = value;
                OnPropertyChanged();
            }
        }

        public string PrimaryTeamName
        {
            get => primaryTeamName;
            set
            {
                primaryTeamName = value;
                OnPropertyChanged();
            }
        }

        public int TeamCount
        {
            get => teamCount;
            set
            {
                teamCount = value;
                OnPropertyChanged();
            }
        }

        public int MemoCount
        {
            get => memoCount;
            set
            {
                memoCount = value;
                OnPropertyChanged();
            }
        }

        public int FileCount
        {
            get => fileCount;
            set
            {
                fileCount = value;
                OnPropertyChanged();
            }
        }

        public int ScheduleCount
        {
            get => scheduleCount;
            set
            {
                scheduleCount = value;
                OnPropertyChanged();
            }
        }
        
        private void SetDefaultValues()
        {
            UserName = "알 수 없음";
            UserEmail = "알 수 없음";
            UserId = "0";
            GoogleId = "알 수 없음";
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            LastLoginAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            PrimaryTeamName = "없음";
            TeamCount = 0;
            MemoCount = 0;
            FileCount = 0;
            ScheduleCount = 0;
            ProfileImageSource = null;
        }

        private async void LoadUserProfile()
        {
            Console.WriteLine($"LoadUserProfile start - CurrentUser: {UserSession.CurrentUser?.Email ?? "null"}");
            
            try
            {
                if (UserSession.CurrentUser == null)
                {
                    MessageBox.Show("사용자 정보가 없습니다. 다시 로그인해주세요.", "오류", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    SetDefaultValues();
                    return;
                }
                
                currentUser = UserSession.CurrentUser;
                
                Console.WriteLine($"currentUser: {currentUser.userId}");
                
                // 기본 사용자 정보 설정
                UserName = currentUser.Name;
                UserEmail = currentUser.Email;
                UserId = currentUser.userId.ToString();
                GoogleId = currentUser.GoogleId;
                CreatedAt = currentUser.CreatedAt.ToString("yyyy-MM-dd HH:mm");
                LastLoginAt = currentUser.LastLoginAt.ToString("yyyy-MM-dd HH:mm");

                // 프로필 이미지 로드
                await LoadProfileImage();

                // 팀 정보 로드
                LoadTeamInfo();

                // 활동 통계 로드
                LoadActivityStats();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadUserProfile 오류: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        private async Task LoadProfileImage()
        {
            try
            {
                if (!string.IsNullOrEmpty(currentUser?.PictureUri))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(currentUser.PictureUri);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    
                    ProfileImageSource = bitmap;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"프로필 이미지 로드 실패: {ex.Message}");
                ProfileImageSource = null;
            }
        }

        private void LoadTeamInfo()
        {
            try
            {
                if (currentUser?.TeamId > 0)
                {
                    var primaryTeam = teamRepository.FindTeamById(currentUser.TeamId);
                    PrimaryTeamName = primaryTeam?.teamName ?? "없음";
                }

                // 사용자가 속한 모든 팀 수 계산
                var userTeams = teamRepository.FindTeamsByUser(currentUser);
                TeamCount = userTeams.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"팀 정보 로드 실패: {ex.Message}");
                PrimaryTeamName = "없음";
                TeamCount = 0;
            }
        }

        private void LoadActivityStats()
        {
            try
            {
                // 메모 개수 계산
                var userMemos = memoService.GetMemoItems(currentUser.TeamId);
                MemoCount = userMemos.Result.Count(m => m.EditorUserId == currentUser.userId);

                // 파일 개수는 실제 구현에 따라 조정 필요
                FileCount = 0; // TODO: 파일 서비스에서 사용자별 파일 개수 가져오기

                // 일정 개수는 실제 구현에 따라 조정 필요  
                ScheduleCount = 0; // TODO: 캘린더 서비스에서 사용자별 일정 개수 가져오기
            }
            catch (Exception ex)
            {
                Console.WriteLine($"활동 통계 로드 실패: {ex.Message}");
                MemoCount = 0;
                FileCount = 0;
                ScheduleCount = 0;
            }
        }

        private void RefreshProfile_Click(object sender, RoutedEventArgs e)
        {
            LoadUserProfile();
            MessageBox.Show("프로필 정보가 새로고침되었습니다.", "알림", 
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("정말 로그아웃하시겠습니까?", "로그아웃 확인", 
                                       MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                var tokenPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credential/token");
                DeleteAllFilesInDirectory(tokenPath);
                
                // 로그아웃 처리
                UserSession.Logout();
                LogoutRequested?.Invoke(this, EventArgs.Empty);
                Close();
            }
        }
        
        public static void DeleteAllFilesInDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
                Debug.WriteLine("디렉토리 내 모든 파일이 삭제되었습니다.");
            }
            else
            {
                Debug.WriteLine("해당 디렉토리가 존재하지 않습니다.");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
