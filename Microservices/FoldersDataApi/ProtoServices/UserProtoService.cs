using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using System.Text.Json;

namespace FoldersDataApi.ProtoServices;

public class UserProtoService : IUserProtoService
{
    readonly IConfiguration _config;
    readonly UserProto.UserProtoClient _client;

    public UserProtoService(IConfiguration configuration)
    {
        _config = configuration;
        var handler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
        var options = new GrpcChannelOptions
        {
            HttpClient = new HttpClient(handler)
        };
        var dbApiChannel = GrpcChannel.ForAddress(_config["DataBaseApi:Uri"], options);
        _client = new UserProto.UserProtoClient(dbApiChannel);
    }

    public async Task<string> UpdateSubChannels(string id, string jsonString)
    {
        var request = new UpdateSubChannelsRequest { Id = id, SubChannelsJson = jsonString };
        var response = await _client.UpdateSubChannelsAsync(request);
        return response.LastChannelsUpdate;
    }

    public async Task<Dictionary<string, string>> GetUserData(string email, string youtubeId)
    {
        var request = new GetUserDataRequest { Email = email, YoutubeId = youtubeId };
        var response = await _client.GetUserDataAsync(request);
        Dictionary<string, string> result = new Dictionary<string, string>()
        {
            { "id", response.Id.ToString() },
            { "youtubeId", response.YoutubeId },
            { "role", response.Role },
            { "subChannelsJson", response.SubChannelsJson },
            { "lastChannelsUpdate", response.LastChannelsUpdate },
            { "folders", response.Folders }
        };
        return result;
    }

    public async Task<bool> UpdateYoutubeId(string id, string youtubeId)
    {
        var request = new UpdateYoutubeIdRequest { Id = id, YoutubeId = youtubeId };
        var response = await _client.UpdateYoutubeIdAsync(request);
        return response.Success;
    }
}