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
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

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
            // Get video properties
            var properties = await file.Properties.GetVideoPropertiesAsync();
            BasicProperties basicProperties = await file.GetBasicPropertiesAsync();

            // Get thumbnail image
            // TODO, fetch thumbnail from Playback.thumbs folder and use thumbnail based on name of video
            var videoThumbnail = await GetVideoThumbnailAsync(file);

            // Create a new VideoFileInfo object      
            VideoFileInfo info = new VideoFileInfo(file, properties, file.DisplayName, basicProperties.Size, videoThumbnail);
            return info;
        }

        private async static Task<BitmapImage> GetVideoThumbnailAsync(StorageFile file)
        {
            // Get thumbnail
            const uint requestedSize = 300;
            const ThumbnailMode thumbnailMode = ThumbnailMode.VideosView;
            const ThumbnailOptions thumbnailOptions = ThumbnailOptions.UseCurrentScale;
            var thumbnail = await file.GetThumbnailAsync(thumbnailMode, requestedSize, thumbnailOptions);

            // Create a bitmap to be the image source
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(thumbnail);
            thumbnail.Dispose();

            return bitmapImage;
        }
    }
}
