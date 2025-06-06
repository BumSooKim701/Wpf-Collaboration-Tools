using System.Windows;

namespace CollaborationTools.calendar;

public partial class ScheduleDetailsWindow : Window
{
    public ScheduleDetailsWindow()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}