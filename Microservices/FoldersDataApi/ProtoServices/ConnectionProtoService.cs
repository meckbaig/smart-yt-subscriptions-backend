using Grpc.Net.Client;
using Grpc.Net.Client.Web;

namespace FoldersDataApi.ProtoServices;

public class ConnectionProtoService : IConnectionProtoService
{
    readonly IConfiguration _config;
    readonly ConnectionProto.ConnectionProtoClient _client;

    public ConnectionProtoService(IConfiguration configuration)
    {
        _config = configuration;
        var handler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
        var options = new GrpcChannelOptions
        {
            HttpClient = new HttpClient(handler)
        };
        var dbApiChannel = GrpcChannel.ForAddress(_config["DataBaseApi:Uri"], options);
        _client = new ConnectionProto.ConnectionProtoClient(dbApiChannel);
    }

    public async Task<bool> GetState()
    {
        var request = new GetStateRequest { };
        var response = await _client.GetStateAsync(request);
        return response.Success;
    }
}