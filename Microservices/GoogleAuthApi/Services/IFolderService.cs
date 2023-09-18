namespace FoldersDataApi.Services
{
    public interface IFolderService
    {
        Task<List<dynamic>> GetFolderVideos(string folderId, string userId);
        Task<string> GetFolder(string id, string userId, bool edit);
        Task<string> UpdateFolder(string jsonString);
        Task<string> CreateFolder(string userId, string name);
        Task<bool> DeleteFolder(string id, string userId);
        Task<string> GetPublicFolders(string userId);
    }
}
