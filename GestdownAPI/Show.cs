using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.Addic7ed.GestdownAPI
{
    public class Show
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public int nbSeasons { get; set; }
        public int[]? Seasons { get; set; }
        public int? TVDbId { get; set; }
    }
}
