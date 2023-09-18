using System.Text.Json;

namespace FoldersDataApi.ProtoServices;

public interface IUserProtoService
{
    Task<Dictionary<string, string>> GetUserData(string email, string youtubeId);
    Task<string> UpdateSubChannels(string id, string jsonString);
    Task<bool> UpdateYoutubeId(string id, string youtubeId);
}