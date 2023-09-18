using System.Text.Json;

namespace FoldersDataApi.ProtoServices;

public interface IFolderProtoService
{
    Task<string> GetFolder(string id, string userId, bool edit);
    Task<string> CreateFolder(string userId, string name);
    Task<string> UpdateFolder(string JsonString);
    Task<bool> DeleteFolder(string id, string userId);
    Task<string> GetPublicFolders(string userId);
}