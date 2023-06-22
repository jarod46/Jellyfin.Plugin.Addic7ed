using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.Addic7ed.GestdownAPI
{
    public class EpisodeDto
    {
        public int Season {get; set;}
        public int Number { get; set; }
        public string? Title { get; set; }
        public string? Show { get; set; }
        public string? Discovered { get; set; }
    }
}
