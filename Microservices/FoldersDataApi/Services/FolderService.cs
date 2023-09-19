using FoldersDataApi.ProtoServices;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using FoldersDataApi.Models;

namespace FoldersDataApi.Services
{
    public class FolderService : IFolderService
    {
        private readonly IFolderProtoService _folderService;
        readonly IConfiguration _config;
        public FolderService(IFolderProtoService folderService, IConfiguration config)
        {
            _folderService = folderService;
            _config = config;
        }

        public async Task<List<dynamic>> GetFolderVideos(string folderId, string userId)
        {
            List<Task> tasks = new List<Task>();
            string folderString = await _folderService.GetFolder(folderId, userId, false);
            if (folderString == "")
                return new List<dynamic>();
            dynamic folderJson = JsonConvert.DeserializeObject<dynamic>(folderString)!;
            Videos videos = new Videos(folderJson, _config["DataBaseApi:Uri"]);
            await videos.Fetch();
            return await videos.ListAsync();
        }

        public async Task<string> GetFolder(string id, string userId, bool edit)
        {
            return await _folderService.GetFolder(id, userId, edit);
        }

        public async Task<string> UpdateFolder(string jsonString)
        {
            jsonString = await ChangeIconSizeAsync(jsonString);
            return await _folderService.UpdateFolder(jsonString);
        }

        private async Task<string> ChangeIconSizeAsync(string jsonString)
        {
            dynamic folderJson = JsonConvert.DeserializeObject<dynamic>(jsonString)!;
            if (folderJson.icon == "")
                return jsonString;
            string icon = folderJson.icon;
            string iconPrefix = icon.Substring(0, 50);
            if (!iconPrefix.Contains("base64"))
                return jsonString;
            iconPrefix = iconPrefix.Substring(0, iconPrefix.IndexOf("base64,") + "base64,".Length);
            string imageFormatString = iconPrefix[(iconPrefix.IndexOf("image/") + 6)..iconPrefix.IndexOf(";")];
            ImageFormat imageFormat = ParseImageFormat(imageFormatString);
            icon = icon.Replace(iconPrefix, "");
            byte[] bytes = Convert.FromBase64String(icon);
            await using var originalStream = new MemoryStream(bytes);
            await using var resizedStream = new MemoryStream();
            Bitmap bitmap = new Bitmap(originalStream);
            if (bytes.Length < 100_000)
                return jsonString;
            double diff = 130_000d / bytes.Length;
            new Bitmap(bitmap,
                new Size((int)(bitmap.Width * Math.Sqrt(diff)),
                         (int)(bitmap.Height * Math.Sqrt(diff))))
                .Save(resizedStream, imageFormat);

            byte[] byteImage = resizedStream.ToArray();
            string imgBase64 = Convert.ToBase64String(byteImage);
            folderJson.icon = iconPrefix + imgBase64;
            return JsonConvert.SerializeObject(folderJson);
        }

        private ImageFormat ParseImageFormat(string str)
        {
            if (str == "jpg")
                str = "jpeg";
            return (ImageFormat)typeof(ImageFormat)
                    .GetProperty(str, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase)
                    .GetValue(null);
        }

        public async Task<string> CreateFolder(string userId, string name)
        {
            return await _folderService.CreateFolder(userId, name);
        }

        public async Task<bool> DeleteFolder(string id, string userId)
        {
            return await _folderService.DeleteFolder(id, userId);
        }

        public async Task<string> GetPublicFolders(string userId)
        {
            return await _folderService.GetPublicFolders(userId);
        }
    }
}
