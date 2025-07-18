﻿using System.Windows;
using System.Windows.Input;

namespace CollaborationTools.memo;

public partial class MemoCreateWindow : Window
{
    private readonly MemoItem _memoItem = new();

    public MemoCreateWindow()
    {
        InitializeComponent();
        DataContext = _memoItem;
    }

    public event EventHandler<MemoItem> MemoCreated;


    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        MemoCreated?.Invoke(this, _memoItem);
        Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left) DragMove();
    }
}