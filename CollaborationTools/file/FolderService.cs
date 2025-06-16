using System.Net;
using System.Windows;
using CollaborationTools.authentication;
using Google;
using Google.Apis.Drive.v3.Data;
using GoogleFile = Google.Apis.Drive.v3.Data.File;

namespace CollaborationTools.file;

public class FolderService
{
    private readonly FileService _fileService = new();

    //공유 문서함 생성
    public async Task<GoogleFile> CreateTeamFolderAsync(string folderName)
    {
        var driveService = GoogleAuthentication.DriveService;

        try
        {
            var folderMetadata = new GoogleFile
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder"
            };

            var request = driveService.Files.Create(folderMetadata);
            request.Fields = "id, name, mimeType";
            var folder = await request.ExecuteAsync();

            return folder;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"팀 폴더 생성 실패: {ex.Message}", "오류",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }
    }

    // 폴더에 멤버 추가
    public async Task<bool> ShareFolderWithMemberAsync(string folderId, string email, string role = "writer")
    {
        var driveService = GoogleAuthentication.DriveService;

        try
        {
            var permission = new Permission
            {
                EmailAddress = email,
                Type = "user",
                Role = role // "reader", "writer", "owner"
            };

            await driveService.Permissions.Create(permission, folderId).ExecuteAsync();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"폴더 공유 실패: {ex.Message}", "오류",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    public async Task<bool> DeleteFolderWithContentsAsync(string folderId, int teamId)
    {
        var driveService = GoogleAuthentication.DriveService;
        Console.WriteLine("folder delete" + folderId);
        try
        {
            // 폴더가 존재하는지 먼저 확인
            var folderCheck = driveService.Files.Get(folderId);
            Console.WriteLine("folder check" + folderCheck.FileId);
            folderCheck.Fields = "id, name, mimeType";

            try
            {
                var folder = await folderCheck.ExecuteAsync();
            }
            catch (GoogleApiException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("folder empty");
                await driveService.Files.Delete(folderId).ExecuteAsync();
                return true; // 이미 삭제된 것으로 간주
            }

            // 폴더 내 파일 목록 조회
            var listRequest = driveService.Files.List();
            listRequest.Q = $"'{folderId}' in parents and trashed=false";
            listRequest.Fields = "files(id, name, mimeType)";

            var filesResult = await listRequest.ExecuteAsync();
            var filesInFolder = filesResult.Files;

            // 폴더 내 파일들 삭제
            if (filesInFolder != null && filesInFolder.Count > 0)
                foreach (var file in filesInFolder)
                    if (file.MimeType == "application/vnd.google-apps.folder")
                        await DeleteFolderWithContentsAsync(file.Id, teamId);
                    else
                        await _fileService.SafeDeleteFileAsync(file.Id, teamId);

            // 폴더 자체 삭제
            await driveService.Files.Delete(folderId).ExecuteAsync();
            return true;
        }
        catch (GoogleApiException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
        {
            Console.WriteLine("folder until delete");
            await driveService.Files.Delete(folderId).ExecuteAsync();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"폴더 삭제 실패: {ex.Message}", "오류",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    public async Task<IList<GoogleFile>> GetFilesInFolderAsync(string folderId)
    {
        var driveService = GoogleAuthentication.DriveService;
        if (driveService == null) throw new InvalidOperationException("Google Drive 서비스가 초기화되지 않았습니다.");

        try
        {
            var request = driveService.Files.List();
            request.Q = $"'{folderId}' in parents and trashed=false and mimeType != 'application/vnd.google-apps.folder'";
            request.Fields = "files(id,name,size,modifiedTime,mimeType,version,parents)";
            request.OrderBy = "modifiedTime desc";
            request.PageSize = 100;

            var result = await request.ExecuteAsync();


            return result.Files ?? new List<GoogleFile>();
        }
        catch (Exception ex)
        {
            throw new Exception($"폴더 내 파일 목록을 가져오는 중 오류가 발생했습니다: {ex.Message}");
        }
    }
    
    
}