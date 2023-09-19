using FoldersDataApi.ProtoServices;

namespace FoldersDataApi.Services
{
    public class UserDataService : IUserdataService
    {
        private readonly IUserProtoService _userService;
        public UserDataService(IUserProtoService userService)
        {
            _userService = userService;
        }

        public async Task<Dictionary<string, string>> GetUserData(string email, string youtubeId)
        {
            return await _userService.GetUserData(email, youtubeId);
        }

        public async Task<string> UpdateSubChannels(string id, string jsonString)
        {
            return await _userService.UpdateSubChannels(id, jsonString);
        }
        public async Task<bool> UpdateYoutubeId(string id, string youtubeId)
        {
            return await _userService.UpdateYoutubeId(id, youtubeId);
        }
    }
}
