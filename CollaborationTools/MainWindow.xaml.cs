using System.Windows;
using System.Windows.Controls;

namespace CollaborationTools;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ContentControl mainContentArea;
    private Grid mainGrid;
    private TabControl sideTabControl;

    public MainWindow()
    {
        InitializeComponent();
        CenterWindowOnScreen();
        Title = "CollaborationTools";
    }

    private void CenterWindowOnScreen()
    {
        // 작업표시줄을 제외한 작업 영역
        var workAreaWidth = SystemParameters.WorkArea.Width;
        var workAreaHeight = SystemParameters.WorkArea.Height;
        var windowWidth = Width;
        var windowHeight = Height;

        Left = workAreaWidth / 2 - windowWidth / 2;
        Top = workAreaHeight / 2 - windowHeight / 2;
    }
}