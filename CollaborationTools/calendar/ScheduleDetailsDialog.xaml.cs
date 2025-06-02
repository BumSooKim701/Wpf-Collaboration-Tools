using System.Windows;

namespace CollaborationTools.calendar;

public partial class ScheduleDetailsDialog : Window
{
    public ScheduleDetailsDialog()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}