using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CollaborationTools.team;

namespace CollaborationTools
{
    public partial class MainPage : Page
    {
        private Team currentTeam;

        public MainPage()
        {
            InitializeComponent();

            // SideBar의 PropertyChanged 이벤트 구독
            SideBarControl.PropertyChanged += SideBarControl_PropertyChanged;

            // MenuBar의 SelectionChanged 이벤트 구독
            MenuBarControl.MenuChanged += MenuBarControl_MenuChanged;

            // 초기 화면은 홈으로 설정
            ShowHomeView();
        }

        private void SideBarControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SideBar.SelectedTeam))
            {
                currentTeam = SideBarControl.SelectedTeam;
                UpdateCurrentTeamInViews();
                UpdateTeamInfo();
            }
        }

        private void MenuBarControl_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            switch (e.MenuType)
            {
                case MenuType.Home:
                    ShowHomeView();
                    break;
                case MenuType.Calendar:
                    ShowCalendarView();
                    break;
                case MenuType.File:
                    ShowFileView();
                    break;
                case MenuType.Memo:
                    ShowMemoView();
                    break;
            }
        }

        private void ShowHomeView()
        {
            HomeView.Visibility = Visibility.Visible;
            CalendarView.Visibility = Visibility.Collapsed;
            FileView.Visibility = Visibility.Collapsed;
            MemoView.Visibility = Visibility.Collapsed;
        }

        private void ShowCalendarView()
        {
            HomeView.Visibility = Visibility.Collapsed;
            CalendarView.Visibility = Visibility.Visible;
            FileView.Visibility = Visibility.Collapsed;
            MemoView.Visibility = Visibility.Collapsed;
        }

        private void ShowFileView()
        {
            HomeView.Visibility = Visibility.Collapsed;
            CalendarView.Visibility = Visibility.Collapsed;
            FileView.Visibility = Visibility.Visible;
            MemoView.Visibility = Visibility.Collapsed;
        }

        private void ShowMemoView()
        {
            HomeView.Visibility = Visibility.Collapsed;
            CalendarView.Visibility = Visibility.Collapsed;
            FileView.Visibility = Visibility.Collapsed;
            MemoView.Visibility = Visibility.Visible;
        }

        private void UpdateCurrentTeamInViews()
        {
            // 모든 뷰에 현재 팀 정보 전달
            if (HomeCalendarView != null)
            {
                HomeCalendarView.CurrentTeam = currentTeam;
            }

            if (HomeFileView != null)
            {
                HomeFileView.CurrentTeam = currentTeam;
            }

            if (CalendarView != null)
            {
                CalendarView.CurrentTeam = currentTeam;
            }

            if (FileView != null)
            {
                FileView.CurrentTeam = currentTeam;
            }
            
            if (MemoView != null)
            {
                MemoView.CurrentTeam = currentTeam;
            }
        }

        private void UpdateTeamInfo()
        {
            if (currentTeam != null)
            {
                TeamInfoText.Text = $"팀명: {currentTeam.teamName}\n" +
                                    $"팀 설명: {currentTeam.teamDescription ?? "설명 없음"}\n" +
                                    $"생성일: {currentTeam.dateOfCreated:yyyy-MM-dd}";
            }
            else
            {
                TeamInfoText.Text = "팀을 선택해주세요";
            }
        }

        // MenuBar에서 사용할 이벤트 인자
        public class MenuChangedEventArgs : System.EventArgs
        {
            public MenuType MenuType { get; set; }

            public MenuChangedEventArgs(MenuType menuType)
            {
                MenuType = menuType;
            }
        }
    }
}