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
using Windows.Storage.Streams;

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
            StorageFolder videosFolder = KnownFolders.VideosLibrary;
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
            if (videoThumbnail == null )
            {
                videoThumbnail = await GetDefaultImage();
            }

            // Create a new VideoFileInfo object      
            VideoFileInfo info = new VideoFileInfo(file, properties, file.DisplayName, basicProperties.Size, videoThumbnail);
            return info;
        }

        private async static Task<BitmapImage> GetVideoThumbnailAsync(StorageFile file)
        {
            BitmapImage thumbnailImage = null;
            string fileName = (string)file.DisplayName + "-thumb.jpg";

            // Get the thumbnail file          
            StorageFolder playbackFolder = await KnownFolders.VideosLibrary.GetFolderAsync("Playback");
            StorageFolder thumbnailFolder = await playbackFolder.GetFolderAsync(".thumbs");
            var result = await thumbnailFolder.GetItemAsync(fileName);

            // Check file type of StorageItem
            if (result.IsOfType(StorageItemTypes.File))
            {
                thumbnailImage = await StorageFileToBitmapImage((StorageFile)result);
            }

            return thumbnailImage;
        }

        public static async Task<BitmapImage> GetDefaultImage()
        {
            // Get a default image instead
            StorageFile defaultThumbail = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/" + "Wide310x150Logo.scale-200.png"));
            return await StorageFileToBitmapImage(defaultThumbail);
        }

        public static async Task<BitmapImage> StorageFileToBitmapImage(StorageFile savedStorageFile)
        {
            using (IRandomAccessStream fileStream = await savedStorageFile.OpenAsync(FileAccessMode.Read))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.DecodePixelHeight = 500;
                bitmapImage.DecodePixelWidth = 500;             
                await bitmapImage.SetSourceAsync(fileStream);
                return bitmapImage;
            }
        }
    }
}
