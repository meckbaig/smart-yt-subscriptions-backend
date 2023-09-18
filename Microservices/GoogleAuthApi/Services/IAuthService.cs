using Microsoft.AspNetCore.Mvc;

namespace FoldersDataApi.Services
{
    public interface IAuthService
    {
        Task<bool> GetConnectionState();
    }
}
