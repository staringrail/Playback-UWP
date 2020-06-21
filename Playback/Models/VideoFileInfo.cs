using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace Playback.Models
{
    public class VideoFileInfo
    {
        public StorageFile VideoFile { get; }

        public VideoProperties VideoProperties { get; }

        public string VideoName { get; }

        public ulong VideoSize { get; }

        public string VideoSizeString => FormatBytes(VideoSize);

        public BitmapImage VideoThumbnailImage;

        public string VideoDuration => VideoProperties.Duration.ToString(@"hh\:mm\:ss");

        public string VideoStats => $"{VideoFile.DateCreated.ToString("MM/dd/yyyy")} | {VideoFile.DateCreated.ToString("hh:mm tt")} | {VideoSizeString}";


        public VideoFileInfo(StorageFile file, VideoProperties properties, string name, ulong size, BitmapImage thumbnail)
        {
            VideoFile = file;
            VideoProperties = properties;
            VideoName = name;
            VideoSize = size;
            VideoThumbnailImage = thumbnail;
        }

        private static string FormatBytes(ulong bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }
    }
}
