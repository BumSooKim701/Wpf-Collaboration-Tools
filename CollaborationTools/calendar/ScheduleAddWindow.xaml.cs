using System.Windows;

namespace CollaborationTools.calendar;

public partial class ScheduleAddWindow : Window
{
    public ScheduleAddWindow()
    {
        InitializeComponent();
    }


    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        
    }
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}