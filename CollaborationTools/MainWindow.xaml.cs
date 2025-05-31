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
        this.SizeChanged += MainWindow_SizeChanged;
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
    
    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // 현재 윈도우의 실제 크기
        Debug.WriteLine($"Window Size - Width: {this.Width}, Height: {this.Height}");
    
        // 클라이언트 영역(타이틀바 제외)의 크기
        Debug.WriteLine($"Client Size - Width: {this.ActualWidth}, Height: {this.ActualHeight}");
    
        // 구분선 추가
        Debug.WriteLine("-------------------");
    }

}

