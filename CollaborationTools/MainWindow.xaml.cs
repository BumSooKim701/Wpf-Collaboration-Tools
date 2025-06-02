using System.Diagnostics;
using System.Windows;
using MaterialDesignThemes.Wpf;

namespace CollaborationTools;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        CenterWindowOnScreen();
    }
    
    private void CenterWindowOnScreen()
    {
        // 작업표시줄을 제외한 작업 영역
        double workAreaWidth = SystemParameters.WorkArea.Width;
        double workAreaHeight = SystemParameters.WorkArea.Height;
        double windowWidth = this.Width;
        double windowHeight = this.Height;
    
        this.Left = (workAreaWidth / 2) - (windowWidth / 2);
        this.Top = (workAreaHeight / 2) - (windowHeight / 2);
    }
    
}

