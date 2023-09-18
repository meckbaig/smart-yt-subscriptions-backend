using FoldersDataApi.ProtoServices;

namespace FoldersDataApi.Services;

public class AuthService : IAuthService
{
    private readonly IConnectionProtoService _connectionService;

    public AuthService(IConnectionProtoService connectionService)
    {
        _connectionService = connectionService;
    }

    public async Task<bool> GetConnectionState()
    {
        return await _connectionService.GetState();
    }
}