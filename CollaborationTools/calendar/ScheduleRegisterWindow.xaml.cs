﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CollaborationTools.team;

namespace CollaborationTools.calendar;

public partial class ScheduleRegisterWindow : Window
{
    private readonly ScheduleItem _scheduleItem;
    private Team _currentTeam;
    public ScheduleRegisterWindow(ScheduleItem scheduleItem, Team currentTeam)
    {
        InitializeComponent();

        _currentTeam = currentTeam;
        _scheduleItem = scheduleItem;
        _scheduleItem.EndDateTime = _scheduleItem.StartDateTime.AddHours(1);

        DataContext = _scheduleItem;
    }

    public event EventHandler ScheduleSaved;

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
        {
            MessageBox.Show("일정명을 입력해주세요.");
        }
        else if (DataContext is ScheduleItem scheduleItem)
        {
            await ScheduleService.RegisterSchedule(scheduleItem, _currentTeam.teamId);

            ScheduleSaved?.Invoke(this, EventArgs.Empty);

            Close();
        }
    }

    // 종일 이벤트 체크박스 처리
    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        StartTimePicker.IsEnabled = false;
        EndTimePicker.IsEnabled = false;
        if (DataContext is ScheduleItem scheduleItem) scheduleItem.IsAllDayEvent = true;
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        StartTimePicker.IsEnabled = true;
        EndTimePicker.IsEnabled = true;
        if (DataContext is ScheduleItem scheduleItem) scheduleItem.IsAllDayEvent = false;
    }

    // 종료날짜를 시작날짜 이후로 선택 가능하도록 하는 함수
    private void StartDatePickerChanged(object sender, SelectionChangedEventArgs e)
    {
        if (StartDatePicker.SelectedDate.HasValue)
        {
            EndDatePicker.DisplayDateStart = StartDatePicker.SelectedDate.Value;

            if (EndDatePicker.SelectedDate.HasValue &&
                EndDatePicker.SelectedDate.Value < StartDatePicker.SelectedDate.Value)
                EndDatePicker.SelectedDate = null;
        }
        else
        {
            // 시작 날짜가 해제되면 제한도 해제
            EndDatePicker.DisplayDateStart = null;
        }
    }

    private void StartTimePickerChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
    {
        if (StartTimePicker.SelectedTime != null && EndTimePicker.SelectedTime != null)
            if (EndTimePicker.SelectedTime < StartTimePicker.SelectedTime)
                EndTimePicker.SelectedTime = StartTimePicker.SelectedTime;
    }

    private void EndTimePickerChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
    {
        if (StartTimePicker.SelectedTime != null && EndTimePicker.SelectedTime != null)
            if (EndTimePicker.SelectedTime < StartTimePicker.SelectedTime)
                StartTimePicker.SelectedTime = EndTimePicker.SelectedTime;
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left) DragMove();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}