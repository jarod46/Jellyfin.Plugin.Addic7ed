using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.Addic7ed.GestdownAPI
{
    public class SubtitleSearchResponse
    {
        public SubtitleDto[]? MatchingSubtitles { get; set; }
        public EpisodeDto? Episode { get; set; }
    }
}
