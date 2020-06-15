using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Playback.Models;
using Playback.Views;

namespace Playback.Services
{
    public static class VideoFileService
    {
        private static ObservableCollection<VideoFileInfo> videoCollection { get; } = new ObservableCollection<VideoFileInfo>();
        public static async Task getVideosAsync()
        {
            QueryOptions options = new QueryOptions();
            options.FolderDepth = FolderDepth.Deep;
            options.FileTypeFilter.Add(".mp4");

            // Get the Videos library
            Windows.Storage.StorageFolder videosFolder = Windows.Storage.KnownFolders.VideosLibrary;
            var result = videosFolder.CreateFileQueryWithOptions(options);

            IReadOnlyList<StorageFile> videoFiles = await result.GetFilesAsync();

            foreach (StorageFile file in videoFiles)
            {
                // Check if file is locally stored. (No OneDrive or network locations)
                if (file.Provider.Id == "computer")
                {
                    videoCollection.Add(await LoadVideoInfo(file));
                }
            }

        }

        public async static Task<VideoFileInfo> LoadVideoInfo(StorageFile file)
        {
            var properties = await file.Properties.GetVideoPropertiesAsync();
            VideoFileInfo info = new VideoFileInfo(properties, file, file.DisplayName);
            return info;
        }
    }
}
