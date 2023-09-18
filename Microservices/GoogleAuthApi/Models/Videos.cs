using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FoldersDataApi.Models
{
    public class Videos
    {
        readonly string[] ytFolders = new[] { "videos", "streams" };
        readonly int FoldersCount;
        readonly dynamic FolderJson;
        readonly int VideosPerChannelFolder;
        readonly string GoogleAuthKey;

        List<dynamic> NotReadyVideosList = new List<dynamic>();
        List<dynamic> ReadyVideosList = new List<dynamic>();
        bool AllTasksSet = false;
        int AppendCalls = 0;
        int LockedAmount = 0;

        object locker = new();
        object readyVideosListAddRangeLocker = new();

        public Videos(dynamic folderJson, string googleAuthKey)
        {
            FolderJson = folderJson;
            GoogleAuthKey = googleAuthKey;
            int channelsCount = Convert.ToInt32(FolderJson.channelsCount);
            if (channelsCount > 0)
            {
                VideosPerChannelFolder = 800 / channelsCount / ytFolders.Length;
                FoldersCount = channelsCount * ytFolders.Length;
            }
            else
                FoldersCount = VideosPerChannelFolder = 0;
        }

        public async Task<bool> Fetch()
        {
            if (FoldersCount == 0)
                return false;

            List<Task> channelsTasks = new List<Task>();
            foreach (dynamic channelJson in FolderJson.subChannelsJson)
            {
                Task task = Task.Run(async () =>
                {
                    await GetFolderVideo(channelJson, VideosPerChannelFolder);
                });
                channelsTasks.Add(task);
            }
            await Task.WhenAll(channelsTasks);
            return true;
        }

        private async Task GetFolderVideo(dynamic channelJson, int videosPerChannelFolder)
        {
            try
            {
                List<Task> channelTasks = new List<Task>();
                string channelId = channelJson.channelId;
                string thumbnail = channelJson.thumbnailUrl;
                foreach (string ytf in ytFolders)
                {
                    Task task = Task.Run(async () =>
                    {
                        var tmpChannelVideosResult = await GetChannelVideos(channelId, ytf, thumbnail, videosPerChannelFolder);
                        Task task1 = Task.Run(() => Console.WriteLine(channelId + " - " + ytf));
                        AddRangeAsync(tmpChannelVideosResult);
                    });
                    channelTasks.Add(task);
                }
                AllTasksSet = true;
                await Task.WhenAll(channelTasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private async Task<List<dynamic>> GetChannelVideos(string channelId, string folder, string channelThumbnail, int videosPerChannelFolder = 30)
        {
            object videoAdditionLocker = new();
            List<dynamic> videos = new List<dynamic>();
            List<Task> videosTasks = new List<Task>();
            string html = await GetHtml(channelId, folder);
            if (html == "")
                return new List<dynamic>();
            dynamic contents = GetContentFromYtHtml(html, folder);
            if (contents == null)
                return new List<dynamic>();
            int contentsLength = 0;
            foreach (object item in contents) { contentsLength++; }
            Task task1 = Task.Run(() => Console.WriteLine("got contentsLength"));
            videosPerChannelFolder = videosPerChannelFolder > contentsLength ? contentsLength : videosPerChannelFolder;
            for (int i = 0; i < videosPerChannelFolder; i++)
            {
                dynamic videoContent = contents[i];
                Task videoTask = Task.Run(() =>
                {
                    GetChannelVideo(channelId, channelThumbnail, videoAdditionLocker, videos, videoContent);
                });
                videosTasks.Add(videoTask);
            }
            await Task.WhenAll(videosTasks);
            return videos;
        }

        private dynamic GetContentFromYtHtml(string html, string folder)
        {
            var dynamicObject = JsonConvert.DeserializeObject<dynamic>(html)!;
            int tab = 0;
            switch (folder)
            {
                case "videos": tab = 1; break;
                case "streams": tab = 3; break;
            }
            dynamic tabs = dynamicObject.contents.twoColumnBrowseResultsRenderer.tabs;
            dynamic contents = "";
            try
            {
                contents = tabs[tab]?.tabRenderer?.content?.richGridRenderer?.contents;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                contents = tabs[0]?.tabRenderer?.content?.richGridRenderer?.contents;
            }
            return contents;
        }

        private void GetChannelVideo(string channelId, string channelThumbnail, object videoAdditionLocker, List<dynamic> videos, dynamic videoContent)
        {
            try
            {
                if (videoContent?.continuationItemRenderer != null)
                    return;
                dynamic content = videoContent.richItemRenderer.content.videoRenderer;
                dynamic video = new System.Dynamic.ExpandoObject();
                video.id = content.videoId;
                video.title = content.title.runs[0].text;
                video.channelId = channelId;
                video.channelThumbnail = channelThumbnail;
                string simpleLendth = content.thumbnailOverlays[0].thumbnailOverlayTimeStatusRenderer.text.simpleText;
                video.simpleLendth = simpleLendth?[0] >= 48 && simpleLendth?[0] <= 57 ? simpleLendth : "";

                video.viewCount = content.viewCountText?.simpleText;
                string ? viewCountText = content.viewCountText?.simpleText;
                if (viewCountText != null)
                {
                    StringBuilder viewCount = new StringBuilder();
                    foreach (char c in viewCountText)
                    {
                        if (c >= 48 && c <= 57)
                        {
                            viewCount.Append(c);
                        }
                        else if (c.ToString() == " " || (c.ToString() == ",")) { viewCount.Append(" "); }
                        else break;
                    }
                    video.viewCount = viewCount.ToString();
                }
                else
                    video.viewCount = "";
                lock (videoAdditionLocker)
                {
                    videos.Add(video);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(videoContent);
                Console.WriteLine(ex);
            }
        }

        private async Task<string> GetHtml(string channelId, string folder)
        {
            string html = "";
            Task downloadString = Task.Run(() =>
            {
                using (WebClient client = new WebClient())
                {
                    html = client.DownloadString($"https://www.youtube.com/channel/{channelId}/{folder}");
                }
            });
            string startSubstring = "var ytInitialData = ";
            string endSubstring = ";</script>";
            downloadString.Wait();
            return await GetSubstring(html, startSubstring, endSubstring);
        }

        private async Task<string> GetSubstring(string original, string start, string end)
        {
            int startId = original.IndexOf(start) + start.Length;
            int endId = original.IndexOf(end, startId);
            if (startId > 0 && endId > 0)
                return original[startId..endId];
            return "";
        }
        public async Task<List<dynamic>> ListAsync()
        {
            Console.WriteLine("result: " + ReadyVideosList.Count);
            return QuickSort();
        }

        private List<dynamic> QuickSort(int l = 0, int? r = null)
        {
            if (ReadyVideosList.Count == 0)
                return ReadyVideosList;
            try
            {
                if (r == null)
                    r = ReadyVideosList.Count - 1;
                var i = l;
                var j = (int)r;
                var pivot = ReadyVideosList[l].publishedAt;

                while (i <= j)
                {
                    while (ReadyVideosList[i].publishedAt > pivot)
                    {
                        i++;
                    }

                    while (ReadyVideosList[j].publishedAt < pivot)
                    {
                        j--;
                    }

                    if (i <= j)
                    {
                        var temp = ReadyVideosList[i];
                        ReadyVideosList[i] = ReadyVideosList[j];
                        ReadyVideosList[j] = temp;
                        i++;
                        j--;
                    }
                }

                if (l < j)
                    QuickSort(l, j);
                if (i < r)
                    QuickSort(i, r);
                return ReadyVideosList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return ReadyVideosList;
            }
        }

        private void AddRangeAsync(List<dynamic> items)
        {
            try
            {
                NotReadyVideosList.AddRange(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            AppendCalls++;
            //return;
            Task task = Task.Run(() => Task.Delay(1));
            task = AppendDataToVideos(task, 50);
            if (AllTasksSet && FoldersCount == AppendCalls)
            {
                Task.Delay(100);
                Task newTask = Task.Run(() => AppendDataToVideos(task));
                newTask.Wait();
            }
            task.Wait();
        }

        private Task AppendDataToVideos(Task task, int amount = 0)
        {
            lock (locker)
            {
                int notLockedAmount = NotReadyVideosList.Count - LockedAmount;
                if (notLockedAmount > amount)
                {
                    if (amount == 0)
                        amount = notLockedAmount;
                    LockedAmount += amount;
                    List<dynamic> tempVideos = NotReadyVideosList.Take(amount).ToList();
                    NotReadyVideosList.RemoveRange(0, amount);
                    LockedAmount -= amount;
                    task = Task.Run(() => AppendInfoToVideos(tempVideos));
                }
            }
            return task;
        }

        private async Task<bool> AppendInfoToVideos(List<dynamic> tempVideos)
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = CreateYtGetVideosUrl(tempVideos);
                var response = await client.GetAsync(url);
                Task message = Task.Run(() => Console.WriteLine(tempVideos.Count));
                string responseContent = await response.Content.ReadAsStringAsync();
                dynamic responseJson = JsonConvert.DeserializeObject<dynamic>(responseContent)!;
                List<Task> tasks = new List<Task>();
                foreach (dynamic responseItem in responseJson.items)
                {
                    Task task = Task.Run(() =>
                    {
                        AppendInfoToVideo(tempVideos, responseItem);
                    });
                    tasks.Add(task);
                }
                await Task.WhenAll(tasks);
                lock (readyVideosListAddRangeLocker)
                {
                    ReadyVideosList.AddRange(tempVideos);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private string CreateYtGetVideosUrl(List<dynamic> tempVideos)
        {
            var videosIds = new StringBuilder();
            foreach (var vid in tempVideos)
            {
                try
                {
                    if (vid != null)
                        videosIds.Append("id=" + vid.id + "&");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            string url = "https://content-youtube.googleapis.com/youtube/v3/videos?";
            url += videosIds.ToString();
            url += $"part=snippet&prettyPrint=true&key={GoogleAuthKey}";
            return url;
        }

        private void AppendInfoToVideo(List<dynamic> tempVideos, dynamic responseItem)
        {
            try
            {
                string responseItemId = responseItem.id;
                var tempVideo = tempVideos.FirstOrDefault(v => v.id == responseItemId);
                if (tempVideo != null)
                {
                    tempVideo.title = responseItem.snippet.title;
                    tempVideo.channelTitle = responseItem.snippet.channelTitle;
                    tempVideo.publishedAt = responseItem.snippet.publishedAt;
                    tempVideo.mediumThumbnail = responseItem.snippet.thumbnails.medium.url;
                    tempVideo.maxThumbnail = responseItem.snippet.thumbnails?.maxres?.url;
                }
                else
                    Console.WriteLine(responseItemId + " не найден!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(tempVideos);
            }
        }
    }
}
