using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CollaborationTools.team;
using Google;
using Microsoft.Win32;
using GoogleFile = Google.Apis.Drive.v3.Data.File;

namespace CollaborationTools.file;

public partial class FileManagerWindow : UserControl, INotifyPropertyChanged
{
    public static readonly DependencyProperty CurrentTeamProperty =
        DependencyProperty.Register(
            nameof(CurrentTeam),
            typeof(Team),
            typeof(FileManagerWindow),
            new PropertyMetadata(null, OnCurrentTeamChanged));

    private readonly string _folderId;
    private readonly FileService fileService = new();
    private readonly FolderService folderService = new();
    private Team _currentTeam;
    private string statusMessage = "준비됨";
    private ObservableCollection<FileItemViewModel> teamFiles = new();

    public FileManagerWindow() : this("primary")
    {
    }

    public FileManagerWindow(string folderId = "primary")
    {
        InitializeComponent();
        _folderId = folderId;
        LoadTeamFiles();
        DataContext = this;
    }

    public Team? CurrentTeam
    {
        get => (Team)GetValue(CurrentTeamProperty);
        set => SetValue(CurrentTeamProperty, value);
    }

    public string CurrentTeamName => CurrentTeam?.teamName ?? "개인 저장소";

    public ObservableCollection<FileItemViewModel> TeamFiles
    {
        get => teamFiles;
        set
        {
            teamFiles = value;
            OnPropertyChanged();
        }
    }

    public string StatusMessage
    {
        get => statusMessage;
        set
        {
            statusMessage = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private static void OnCurrentTeamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FileManagerWindow control)
        {
            control.OnPropertyChanged(nameof(CurrentTeamName));
            control.LoadTeamFiles();
        }
    }

    // LoadTeamFiles 메서드에 NoFilesMessage 처리 추가
    private async void LoadTeamFiles()
    {
        if (_folderId == "primary" && CurrentTeam == null)
        {
            try
            {
                StatusMessage = "내 드라이브 파일을 불러오는 중...";
                // 개인 구글 드라이브 최상위 파일 목록 불러오기
                var files = await folderService.GetFilesInFolderAsync("root"); // "root"는 구글 드라이브 내 드라이브 최상위 폴더

                TeamFiles.Clear();
                foreach (var file in files)
                {
                    var fileItem = new FileItemViewModel
                    {
                        FileId = file.Id,
                        FileName = file.Name,
                        FileSize = FormatFileSize(file.Size ?? 0),
                        ModifiedDate = file.ModifiedTime?.ToString("yyyy-MM-dd HH:mm") ?? "",
                        FileIcon = GetFileIcon(file.Name),
                        MimeType = file.MimeType
                    };
                    TeamFiles.Add(fileItem);
                }

                NoFilesMessage.Visibility = TeamFiles.Count == 0 ? Visibility.Visible : Visibility.Hidden;
                StatusMessage = TeamFiles.Count == 0 ? "파일이 없습니다" : $"{TeamFiles.Count}개의 파일";
            }
            catch (Exception ex)
            {
                StatusMessage = $"오류: {ex.Message}";
                NoFilesMessage.Visibility = Visibility.Visible;
                MessageBox.Show($"내 드라이브 파일 목록 오류: {ex.Message}");
            }

            return;
        }

        try
        {
            StatusMessage = "파일 목록을 불러오는 중...";
            var files = await folderService.GetFilesInFolderAsync(CurrentTeam.teamFolderId);

            TeamFiles.Clear();
            foreach (var file in files)
            {
                var fileItem = new FileItemViewModel
                {
                    FileId = file.Id,
                    FileName = file.Name,
                    FileSize = FormatFileSize(file.Size ?? 0),
                    ModifiedDate = file.ModifiedTime?.ToString("yyyy-MM-dd HH:mm") ?? "",
                    FileIcon = GetFileIcon(file.Name),
                    MimeType = file.MimeType
                };

                TeamFiles.Add(fileItem);
            }

            // NoFilesMessage 표시/숨김 처리
            NoFilesMessage.Visibility = TeamFiles.Count == 0 ? Visibility.Visible : Visibility.Hidden;
            StatusMessage = TeamFiles.Count == 0 ? "파일이 없습니다" : $"{TeamFiles.Count}개의 파일";
        }
        catch (Exception ex)
        {
            StatusMessage = $"오류: {ex.Message}";
            NoFilesMessage.Visibility = Visibility.Visible;
            MessageBox.Show($"파일 목록을 불러오는 중 오류가 발생했습니다: {ex.Message}");
        }
    }

    private async void OnFileDropAsync(object sender, DragEventArgs e)
    {
        DropZoneOverlay.Visibility = Visibility.Collapsed;

        // if (CurrentTeam?.teamFolderId == null)
        // {
        //     MessageBox.Show("팀을 먼저 선택해주세요.");
        //     return;
        // }

        try
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var validFiles = new List<string>();

                foreach (var filePath in files)
                    if (File.Exists(filePath))
                        validFiles.Add(filePath);
                    else
                        StatusMessage = $"파일을 찾을 수 없습니다: {Path.GetFileName(filePath)}";

                if (validFiles.Count > 0) await UploadOrUpdateFilesAsync(validFiles.ToArray());
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"드래그 앤 드롭 오류: {ex.Message}";
            MessageBox.Show($"파일 처리 중 오류가 발생했습니다: {ex.Message}");
        }
    }

    private async Task UploadOrUpdateFilesAsync(string[] filePaths)
    {
        try
        {
            UploadProgressBar.Visibility = Visibility.Visible;

            foreach (var filePath in filePaths)
            {
                if (!File.Exists(filePath))
                {
                    StatusMessage = $"파일을 찾을 수 없습니다: {Path.GetFileName(filePath)}";
                    continue;
                }

                // 100MB 크기 제한 확인
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > 100 * 1024 * 1024) // 100MB
                {
                    StatusMessage = $"파일 크기가 100MB를 초과합니다: {fileInfo.Name}";
                    MessageBox.Show($"파일 크기가 100MB를 초과합니다: {fileInfo.Name}");
                    continue;
                }

                StatusMessage = $"처리 중: {Path.GetFileName(filePath)}";

                var fileName = Path.GetFileName(filePath);

                // 기존 파일이 있는지 확인
                var existingFileId = await fileService.FindExistingFileAsync(CurrentTeam?.teamFolderId ?? "root" , fileName);

                GoogleFile processedFile;

                if (!string.IsNullOrEmpty(existingFileId))
                {
                    // 기존 파일 버전 업데이트
                    StatusMessage = $"버전 업데이트 중: {fileName}";
                    processedFile = await fileService.UpdateFileVersionAsync(existingFileId, filePath);

                    // UI에서 기존 파일 항목 업데이트
                    var existingItem = TeamFiles.FirstOrDefault(f => f.FileId == existingFileId);
                    if (existingItem != null)
                    {
                        existingItem.FileSize = FormatFileSize(processedFile.Size ?? 0);
                        existingItem.ModifiedDate = processedFile.ModifiedTime?.ToString("yyyy-MM-dd HH:mm") ?? "";
                        // 버전 정보 새로고침
                        if (existingItem.IsExpanded) await LoadFileVersions(existingItem);
                    }
                }
                else
                {
                    // 새 파일 업로드
                    StatusMessage = $"새 파일 업로드 중: {fileName}";
                    processedFile = await fileService.UploadNewFileAsync(CurrentTeam?.teamFolderId ?? "root", filePath, CurrentTeam.teamId);

                    // UI에 새 파일 항목 추가
                    if (processedFile != null)
                    {
                        var fileItem = new FileItemViewModel
                        {
                            FileId = processedFile.Id,
                            FileName = processedFile.Name,
                            FileSize = FormatFileSize(processedFile.Size ?? 0),
                            ModifiedDate = processedFile.ModifiedTime?.ToString("yyyy-MM-dd HH:mm") ?? "",
                            FileIcon = GetFileIcon(processedFile.Name),
                            MimeType = processedFile.MimeType
                        };
                        TeamFiles.Add(fileItem);
                    }
                }

                NoFilesMessage.Visibility = TeamFiles.Count == 0 ? Visibility.Visible : Visibility.Hidden;
            }

            StatusMessage = $"처리 완료: {TeamFiles.Count}개 파일";
        }
        catch (Exception ex)
        {
            StatusMessage = $"파일 처리 중 오류: {ex.Message}";
            MessageBox.Show($"파일 처리 중 오류가 발생했습니다: {ex.Message}");
        }
        finally
        {
            UploadProgressBar.Visibility = Visibility.Collapsed;
        }
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        try
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                DropZoneOverlay.Visibility = Visibility.Visible;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                DropZoneOverlay.Visibility = Visibility.Collapsed;
            }

            e.Handled = true;
        }
        catch (Exception ex)
        {
            e.Effects = DragDropEffects.None;
            DropZoneOverlay.Visibility = Visibility.Collapsed;
            Console.WriteLine($"드래그 오버 오류: {ex.Message}");
        }
    }

    private async Task UploadFiles(string[] filePaths)
    {
        try
        {
            UploadProgressBar.Visibility = Visibility.Visible;

            foreach (var filePath in filePaths)
            {
                // 파일 존재 확인
                if (!File.Exists(filePath))
                {
                    StatusMessage = $"파일을 찾을 수 없습니다: {Path.GetFileName(filePath)}";
                    continue;
                }

                // 파일 크기 확인 (100MB 제한)
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > 100 * 1024 * 1024) // 100MB
                {
                    StatusMessage = $"파일이 너무 큽니다 (100MB 제한): {fileInfo.Name}";
                    MessageBox.Show($"파일 크기가 100MB를 초과합니다: {fileInfo.Name}");
                    continue;
                }

                StatusMessage = $"업로드 중: {Path.GetFileName(filePath)}";

                var uploadedFile = await fileService.UploadFileAsync(
                    CurrentTeam.teamFolderId,
                    filePath);

                if (uploadedFile != null)
                {
                    var fileItem = new FileItemViewModel
                    {
                        FileId = uploadedFile.Id,
                        FileName = uploadedFile.Name,
                        FileSize = FormatFileSize(uploadedFile.Size ?? 0),
                        ModifiedDate = uploadedFile.ModifiedTime?.ToString("yyyy-MM-dd HH:mm") ?? "",
                        FileIcon = GetFileIcon(uploadedFile.Name),
                        MimeType = uploadedFile.MimeType
                    };

                    TeamFiles.Add(fileItem);
                }
            }

            // NoFilesMessage 숨김 처리
            NoFilesMessage.Visibility = TeamFiles.Count == 0 ? Visibility.Visible : Visibility.Hidden;
            StatusMessage = $"{TeamFiles.Count}개의 파일";
        }
        catch (Exception ex)
        {
            StatusMessage = $"업로드 오류: {ex.Message}";
            MessageBox.Show($"파일 업로드 중 오류가 발생했습니다: {ex.Message}");
        }
        finally
        {
            UploadProgressBar.Visibility = Visibility.Collapsed;
        }
    }

    private async void FileItem_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is Grid grid && grid.DataContext is FileItemViewModel fileItem)
        {
            fileItem.IsExpanded = !fileItem.IsExpanded;

            if (fileItem.IsExpanded && fileItem.Versions.Count == 0) await LoadFileVersions(fileItem);
        }
    }

    private async Task LoadFileVersions(FileItemViewModel fileItem)
    {
        try
        {
            StatusMessage = $"{fileItem.FileName}의 버전을 불러오는 중...";

            var versions = await fileService.GetFileVersionsAsync(fileItem.FileId);

            fileItem.Versions.Clear();
            foreach (var version in versions)
                fileItem.Versions.Add(new FileVersionViewModel
                {
                    VersionId = version.Id,
                    VersionName = $"버전 {version.Version}",
                    ModifiedDate = version.ModifiedTime?.ToString("yyyy-MM-dd HH:mm") ?? "",
                    FileSize = FormatFileSize(version.Size ?? 0),
                    FileId = fileItem.FileId
                });

            StatusMessage = $"{fileItem.FileName}: {fileItem.Versions.Count}개의 버전";
        }
        catch (Exception ex)
        {
            StatusMessage = $"버전 로드 오류: {ex.Message}";
            MessageBox.Show($"파일 버전을 불러오는 중 오류가 발생했습니다: {ex.Message}");
        }
    }

    private async void DownloadFile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.CommandParameter is FileItemViewModel fileItem)
            await DownloadFile(fileItem.FileId, fileItem.FileName);
    }

    private async void DownloadVersion_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.CommandParameter is FileVersionViewModel version)
        {
            var parentFile = TeamFiles.FirstOrDefault(f => f.FileId == version.FileId);
            var fileName = parentFile?.FileName ?? "unknown";
            await DownloadFile(version.FileId,
                $"{Path.GetFileNameWithoutExtension(fileName)}_{version.VersionName}{Path.GetExtension(fileName)}");
        }
    }

    private async Task DownloadFile(string fileId, string fileName)
    {
        try
        {
            // 파일명에서 특수문자 제거
            fileName = SanitizeFileName(fileName);

            var saveFileDialog = new SaveFileDialog
            {
                FileName = fileName,
                Filter = "모든 파일 (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                // 디렉토리 존재 확인 및 생성
                var directory = Path.GetDirectoryName(saveFileDialog.FileName);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                StatusMessage = $"다운로드 중: {fileName}";

                await fileService.DownloadFileAsync(fileId, saveFileDialog.FileName);

                StatusMessage = "다운로드 완료";
                MessageBox.Show("파일이 성공적으로 다운로드되었습니다.");
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"다운로드 오류: {ex.Message}";
            MessageBox.Show($"파일 다운로드 중 오류가 발생했습니다: {ex.Message}");
        }
    }

    private string SanitizeFileName(string fileName)
    {
        // 파일명에서 사용할 수 없는 문자들을 제거
        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (var c in invalidChars) fileName = fileName.Replace(c, '_');
        return fileName;
    }

    private async void DeleteFile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.CommandParameter is FileItemViewModel fileItem)
        {
            var result = MessageBox.Show(
                $"'{fileItem.FileName}' 파일을 삭제하시겠습니까?",
                "파일 삭제 확인",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
                try
                {
                    StatusMessage = $"삭제 중: {fileItem.FileName}";

                    await fileService.SafeDeleteFileAsync(fileItem.FileId, CurrentTeam.teamId);
                    TeamFiles.Remove(fileItem);

                    StatusMessage = $"{TeamFiles.Count}개의 파일";
                }
                catch (GoogleApiException ex) when (ex.HttpStatusCode == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show(Application.Current.MainWindow, "해당 파일에 대한 권한이 없습니다.", "오류",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    StatusMessage = $"삭제 오류: {ex.Message}";
                    MessageBox.Show($"파일 삭제 중 오류가 발생했습니다: {ex.Message}");
                }
        }
    }

    private void RefreshFiles_Click(object sender, RoutedEventArgs e)
    {
        LoadTeamFiles();
    }

    private string GetFileIcon(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return extension switch
        {
            ".pdf" => "FilePdfBox",
            ".doc" or ".docx" => "FileWord",
            ".xls" or ".xlsx" => "FileExcel",
            ".ppt" or ".pptx" => "FilePowerpoint",
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" => "FileImage",
            ".mp4" or ".avi" or ".mov" or ".wmv" => "FileVideo",
            ".mp3" or ".wav" or ".wma" => "FileMusic",
            ".zip" or ".rar" or ".7z" => "FileArchive",
            ".txt" => "FileDocument",
            _ => "File"
        };
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        var order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class FileItemViewModel : INotifyPropertyChanged
{
    private bool isExpanded;

    public string FileId { get; set; }
    public string FileName { get; set; }
    public string FileSize { get; set; }
    public string ModifiedDate { get; set; }
    public string FileIcon { get; set; }
    public string MimeType { get; set; }
    public ObservableCollection<FileVersionViewModel> Versions { get; set; } = new();

    public bool IsExpanded
    {
        get => isExpanded;
        set
        {
            isExpanded = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ExpandIcon));
        }
    }

    public string ExpandIcon => IsExpanded ? "ChevronUp" : "ChevronDown";

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class FileVersionViewModel
{
    public string VersionId { get; set; }
    public string VersionName { get; set; }
    public string ModifiedDate { get; set; }
    public string FileSize { get; set; }
    public string FileId { get; set; }
}