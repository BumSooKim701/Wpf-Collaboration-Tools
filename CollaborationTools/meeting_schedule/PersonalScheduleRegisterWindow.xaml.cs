using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CollaborationTools.meeting_schedule;

public partial class PersonalScheduleRegisterWindow : Window
{
    
    
    public PersonalScheduleRegisterWindow()
    {
        InitializeComponent();
        
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        
    }

    // 종일 이벤트 체크박스 처리
    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        
    }
    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        
    }
    
    // 종료날짜를 시작날짜 이후로 선택 가능하도록 하는 함수
    private void StartDatePickerChanged(object sender, SelectionChangedEventArgs e)
    {
        
    }

    private void StartTimePickerChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
    {
        
    }
    
    private void EndTimePickerChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
    {
        
    }
    
    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            this.DragMove();
        }
    }
    
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}