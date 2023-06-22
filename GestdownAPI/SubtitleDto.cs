using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.Addic7ed.GestdownAPI
{
    public class SubtitleDto
    {
        public string? SubtitleId { get; set; }
        public string? Version { get; set; }
        public bool Completed { get; set; }
        public bool HearingImpaired { get; set; }
        public bool Corrected { get; set; }
        public bool HD {get; set; }
        public string? DownloadUri { get; set; }
        public string? Language { get; set; }
        public string? Discovered { get; set; }
        public int DownloadCount { get; set; }
    }
}
