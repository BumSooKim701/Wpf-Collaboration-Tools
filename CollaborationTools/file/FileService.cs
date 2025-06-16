using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using CollaborationTools.authentication;
using CollaborationTools.database;
using CollaborationTools.team;
using CollaborationTools.timeline;
using CollaborationTools.user;
using Google;
using Google.Apis.Calendar.v3;
using Google.Apis.Download;
using Google.Apis.Upload;
using MySqlConnector;
using GoogleFile = Google.Apis.Drive.v3.Data.File;

namespace CollaborationTools.file;

public class FileService
{
    private readonly TeamRepository _teamRepository = new();

    // 파일 업로드 (등록)
    public async Task<GoogleFile> UploadFileAsync(string folderId, string filePath)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            return null;
        
        var driveService = GoogleAuthentication.DriveService;

        if (driveService == null) throw new InvalidOperationException("Google Drive 서비스가 초기화되지 않았습니다.");

        try
        {
            // 파일 존재 확인
            if (!File.Exists(filePath)) throw new FileNotFoundException($"파일을 찾을 수 없습니다: {filePath}");

            var fileName = Path.GetFileName(filePath);
            var mimeType = GetMimeType(filePath);

            var fileMetadata = new GoogleFile
            {
                Name = fileName,
                Parents = new List<string> { folderId }
            };

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var request = driveService.Files.Create(fileMetadata, stream, mimeType);
            request.Fields = "id,name,size,modifiedTime,mimeType,version";

            var uploadedFile = await request.UploadAsync();
            if (uploadedFile.Status == UploadStatus.Completed) return request.ResponseBody;

            throw new Exception($"파일 업로드 실패: {uploadedFile.Exception?.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"파일 업로드 중 오류가 발생했습니다: {ex.Message}");
        }
    }

    // 새 파일 업로드 (최초 업로드)
    public async Task<GoogleFile> UploadNewFileAsync(string folderId, string filePath, int teamId)
    {
        var processedFile = await UploadFileAsync(folderId, filePath);
        if (teamId != 0)
        {
            IncreaseFileCount(teamId);    
        }
        
        return processedFile;
    }


    // 기존 파일 버전 업데이트 (핵심 기능)
    public async Task<GoogleFile> UpdateFileVersionAsync(string fileId, string filePath)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            return null;
        
        var driveService = GoogleAuthentication.DriveService;
        if (driveService == null)
            throw new InvalidOperationException("Google Drive 서비스에 연결할 수 없습니다.");

        try
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"파일을 찾을 수 없습니다: {filePath}");

            var fileName = Path.GetFileName(filePath);
            var mimeType = GetMimeType(filePath);

            // 기존 파일의 메타데이터 유지 (이름은 변경하지 않음)
            var fileMetadata = new GoogleFile
            {
                Name = fileName
            };

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var request = driveService.Files.Update(fileMetadata, fileId, stream, mimeType);
            request.Fields = "id,name,size,modifiedTime,mimeType,version";

            var uploadResult = await request.UploadAsync();
            if (uploadResult.Status == UploadStatus.Completed) return request.ResponseBody;

            throw new Exception($"파일 업데이트 실패: {uploadResult.Exception?.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"파일 버전 업데이트 중 오류 발생: {ex.Message}");
        }
    }

    // 파일이 이미 존재하는지 확인
    public async Task<string> FindExistingFileAsync(string folderId, string fileName)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            return null;
        
        var driveService = GoogleAuthentication.DriveService;
        if (driveService == null)
            throw new InvalidOperationException("Google Drive 서비스에 연결할 수 없습니다.");

        try
        {
            var request = driveService.Files.List();
            request.Q = $"'{folderId}' in parents and name='{fileName}' and trashed=false";
            request.Fields = "files(id,name)";

            var result = await request.ExecuteAsync();
            return result.Files?.FirstOrDefault()?.Id;
        }
        catch (Exception ex)
        {
            throw new Exception($"파일 검색 중 오류 발생: {ex.Message}");
        }
    }

    public async Task DownloadFileAsync(string fileId, string savePath)
    {
        var driveService = GoogleAuthentication.DriveService;
        if (driveService == null) throw new InvalidOperationException("Google Drive 서비스가 초기화되지 않았습니다.");

        try
        {
            // 저장할 디렉토리 확인 및 생성
            var directory = Path.GetDirectoryName(savePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) Directory.CreateDirectory(directory);

            var request = driveService.Files.Get(fileId);
            using var stream = new FileStream(savePath, FileMode.Create, FileAccess.Write);

            var downloadProgress = await request.DownloadAsync(stream);

            if (downloadProgress.Status == DownloadStatus.Failed)
                throw new Exception($"파일 다운로드 실패: {downloadProgress.Exception?.Message}");
        }
        catch (DirectoryNotFoundException ex)
        {
            throw new Exception($"저장 경로를 찾을 수 없습니다: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"파일 다운로드 중 오류가 발생했습니다: {ex.Message}");
        }
    }

    public async Task<IList<GoogleFile>> GetFileVersionsAsync(string fileId)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            return null;
        
        var driveService = GoogleAuthentication.DriveService;
        if (driveService == null) throw new InvalidOperationException("Google Drive 서비스가 초기화되지 않았습니다.");

        try
        {
            var request = driveService.Revisions.List(fileId);
            request.Fields = "revisions(id,modifiedTime,size)";

            var result = await request.ExecuteAsync();

            // Google Drive API의 Revision을 File 객체로 변환
            var versions = new List<GoogleFile>();
            if (result.Revisions != null)
                for (var i = 0; i < result.Revisions.Count; i++)
                {
                    var revision = result.Revisions[i];
                    versions.Add(new GoogleFile
                    {
                        Id = revision.Id,
                        ModifiedTime = revision.ModifiedTime,
                        Size = revision.Size,
                        Version = i + 1 // 순서대로 버전 번호 할당
                    });
                }

            return versions;
        }
        catch (Exception ex)
        {
            throw new Exception($"파일 버전 목록을 가져오는 중 오류가 발생했습니다: {ex.Message}");
        }
    }

    public async Task<bool> SafeDeleteFileAsync(string fileId, int teamId)
    {
        var driveService = GoogleAuthentication.DriveService;

        try
        {
            // 먼저 파일이 존재하는지 확인
            var getRequest = driveService.Files.Get(fileId);
            getRequest.Fields = "id, name, trashed";
            var file = await getRequest.ExecuteAsync();

            if (file.Trashed == true)
            {
                Console.WriteLine("파일이 이미 휴지통에 있습니다.");
                return true;
            }

            // 파일 삭제 실행
            await driveService.Files.Delete(fileId).ExecuteAsync();
            DecreaseFileCount(teamId);
            return true;
        }
        catch (GoogleApiException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
        {
            Console.WriteLine("파일을 찾을 수 없습니다. 이미 삭제되었을 수 있습니다.");
            return true; // 이미 삭제된 것으로 간주
        }
        catch (GoogleApiException ex) when (ex.HttpStatusCode == HttpStatusCode.Forbidden)
        {
            Debug.WriteLine(ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"파일 삭제 중 오류 발생: {ex.Message}", "오류",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private string GetMimeType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();

        // 확장자별 MIME 타입 매핑
        return extension switch
        {
            // 문서 파일
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",

            // 이미지 파일
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".svg" => "image/svg+xml",
            ".webp" => "image/webp",
            ".ico" => "image/x-icon",

            // 비디오 파일
            ".mp4" => "video/mp4",
            ".avi" => "video/x-msvideo",
            ".mov" => "video/quicktime",
            ".wmv" => "video/x-ms-wmv",
            ".flv" => "video/x-flv",
            ".webm" => "video/webm",
            ".mkv" => "video/x-matroska",

            // 오디오 파일
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".wma" => "audio/x-ms-wma",
            ".aac" => "audio/aac",
            ".ogg" => "audio/ogg",
            ".flac" => "audio/flac",

            // 텍스트 파일
            ".txt" => "text/plain",
            ".html" or ".htm" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".csv" => "text/csv",

            // 압축 파일
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            ".7z" => "application/x-7z-compressed",
            ".tar" => "application/x-tar",
            ".gz" => "application/gzip",

            // 기타
            ".exe" => "application/x-msdownload",
            ".msi" => "application/x-msi",
            ".apk" => "application/vnd.android.package-archive",
            ".dmg" => "application/x-apple-diskimage",

            // 기본값
            _ => "application/octet-stream"
        };
    }
    
    private bool IncreaseFileCount(int teamId)
    {
        var result = false;
        var userId = UserSession.CurrentUser.userId;
        MySqlConnection connection = null;
        ConnectionPool _connectionPool = ConnectionPool.GetInstance();

        try
        {
            connection = _connectionPool.GetConnection();
            
            using (var command =
                   new MySqlCommand(
                       "UPDATE team_member SET file_count = file_count + 1 WHERE user_id = @userId and team_id = @teamId;",
                       connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@teamId", teamId);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult > 0) result = true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"searching to email error: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }
    
    private static bool DecreaseFileCount(int teamId)
    {
        var result = false;
        var userId = UserSession.CurrentUser.userId;
        MySqlConnection connection = null;
        ConnectionPool _connectionPool = ConnectionPool.GetInstance();

        try
        {
            connection = _connectionPool.GetConnection();
            
            using (var command =
                   new MySqlCommand(
                       "UPDATE team_member SET file_count = file_count - 1 WHERE user_id = @userId and team_id = @teamId and file_count > 0;",
                       connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@teamId", teamId);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult > 0) result = true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"searching to email error: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }
}