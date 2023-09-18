using FoldersDataApi.Services;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FoldersDataApi.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserdataService _userdataService;
        public UserController(IUserdataService userdataService)
        {
            _userdataService = userdataService;
        }

        public async Task<IActionResult> GetData(string email, string youtubeId)
        {
            if (youtubeId == null)
                youtubeId = "";
            var res = Json(await _userdataService.GetUserData(email, youtubeId));
            return res;
        }

        public async Task<string> UpdateSubChannels(string id, [FromBody] JsonDocument data)
        {
            var jsonString = data.RootElement.GetProperty("channels").ToString();
            return await _userdataService.UpdateSubChannels(id, jsonString);
        }

        public async Task<bool> UpdateYoutubeId(string id, string youtubeId)
        {
            return await _userdataService.UpdateYoutubeId(id, youtubeId);
        }

        //public async Task<IActionResult> GetSubscriptions(string id, string key)
        //{
        //    var yt = new YouTubeService(new BaseClientService.Initializer()
        //    {
        //        ApiKey = key,
        //    });
        //    var channelsRequest = yt.Channels.List("contentDetails");
        //    channelsRequest.Id = id;
        //    var channels = channelsRequest.Execute();
        //    List<string> subscriptions = new List<string>();
        //    foreach (var channel in channels.Items)
        //    {
        //        subscriptions.Add(channel.ContentOwnerDetails.ContentOwner.ToString());
        //    }
        //    return Json(subscriptions);
        //}
    }
}
