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
        public static async Task <ObservableCollection<VideoFileInfo>> GetVideosAsync()
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

            // Wait for all video files to be retrieved before returning
            await Task.CompletedTask;
            return videoCollection;
        }

        public async static Task<VideoFileInfo> LoadVideoInfo(StorageFile file)
        {
            var properties = await file.Properties.GetVideoPropertiesAsync();
            Windows.Storage.FileProperties.BasicProperties basicProperties = await file.GetBasicPropertiesAsync();

            // Create a new VideoFileInfo object      
            VideoFileInfo info = new VideoFileInfo(file, properties, file.DisplayName, basicProperties.Size);
            return info;
        }
    }
}
