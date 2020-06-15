using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace Playback.Services
{
    public static class VideoFileService
    {
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
                System.Diagnostics.Debug.WriteLine("Found a video!");
            }

        }
    }
}
