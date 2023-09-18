using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using System.Text.Json;

namespace FoldersDataApi.ProtoServices;

public class FolderProtoService : IFolderProtoService
{
    readonly IConfiguration _config;
    readonly FolderProto.FolderProtoClient _client;

    public FolderProtoService(IConfiguration configuration)
    {
        _config = configuration;
        var handler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
        var options = new GrpcChannelOptions
        {
            HttpClient = new HttpClient(handler)
        };
        var dbApiChannel = GrpcChannel.ForAddress(_config["DataBaseApi:Uri"], options);
        _client = new FolderProto.FolderProtoClient(dbApiChannel);
    }

    public async Task<string> GetFolder(string id, string userId, bool edit)
    {
        var request = new GetFolderRequest { Id = id, UserId = userId, Edit = edit };
        var response = await _client.GetFolderAsync(request);
        return response.JsonString;
    }

    public async Task<string> UpdateFolder(string jsonString)
    {
        var request = new UpdateFolderRequest { JsonString = jsonString };
        var response = await _client.UpdateFolderAsync(request);
        return response.JsonString;
    }

    public async Task<string> CreateFolder(string userId, string name)
    {
        var request = new CreateFolderRequest { UserId = userId, Name = name };
        var response = await _client.CreateFolderAsync(request);
        return response.JsonString;
    }

    public async Task<bool> DeleteFolder(string id, string userId)
    {
        var request = new DeleteFolderRequest { Id = id, UserId = userId };
        var response = await _client.DeleteFolderAsync(request);
        return response.Success;
    }
    public async Task<string> GetPublicFolders(string userId)
    {
        if (userId == null)
            userId = "";
        var request = new GetPublicFoldersRequest { UserId = userId };
        var response = await _client.GetPublicFoldersAsync(request);
        return response.JsonString;
    }
}