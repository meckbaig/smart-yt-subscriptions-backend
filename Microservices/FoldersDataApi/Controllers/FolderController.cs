using FoldersDataApi.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FoldersDataApi.Controllers
{
    public class FolderController : Controller
    {
        private readonly IFolderService _folderService;
        public FolderController(IFolderService folderService)
        {
            _folderService = folderService;
        }
        public async Task<IActionResult> GetVideos(string id, string userId)
        {
            var res = await _folderService.GetFolderVideos(id, userId);
            if (res.Count == 0)
            {
                return Problem(title: "Папка по указанному адресу не найдена");
            }
            var result = JsonConvert.SerializeObject(res.ToArray());
            return Content(result);

        }

        public async Task<IActionResult> Get(string id, string userId, bool edit)
        {
            // Console.WriteLine($"GetFolder");
            var res = await _folderService.GetFolder(id, userId, edit);
            if (res == "")
                return Problem(title: "Папка по указанному адресу не найдена");
            return Content(res);
        }

        public async Task<string> Create(string id, string name)
        {
            // Console.WriteLine($"CreateFolder");
            return await _folderService.CreateFolder(id, name);
        }

        public async Task<string> Update([FromBody] JsonDocument data)
        {
            //Console.WriteLine($"UpdateFolder");
            var serializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string jsonString = JsonObject.Create(data.RootElement).ToJsonString(serializerOptions);
            return await _folderService.UpdateFolder(jsonString);
        }

        public async Task<bool> Delete(string id, string userId)
        {
            //Console.WriteLine($"DeleteFolder");
            return await _folderService.DeleteFolder(id, userId);
        }

        public async Task<IActionResult> GetPublicFolders(string userId)
        {
            var res = await _folderService.GetPublicFolders(userId);
            return Content(res);
        }
    }
}
