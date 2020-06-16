using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Playback.Models
{
    public class VideoFileInfo
    {
        public StorageFile VideoFile { get; }

        public VideoProperties VideoProperties { get; }

        public string VideoName { get; }

        public ulong VideoSize { get; }

        public VideoFileInfo(StorageFile file, VideoProperties properties, string name, ulong size)
        {
            VideoFile = file;
            VideoProperties = properties;
            VideoName = name;
            VideoSize = size;
        }



    }
}
