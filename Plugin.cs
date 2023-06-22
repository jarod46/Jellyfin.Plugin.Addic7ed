using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.Addic7ed
{
    public class Plugin : BasePlugin<BasePluginConfiguration>, IHasWebPages
    {
        public override string Name => "Addic7ed/Gestdown Subtitles";

        public override Guid Id => Guid.Parse("bacb8d95-1cb2-4763-affb-8af0037b3e7f");

        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        public static Plugin? Instance { get; private set; }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new PluginPageInfo[] { };
            //return new[]
            //{
            //    new PluginPageInfo
            //    {
            //        Name = this.Name,
            //        EmbeddedResourcePath = string.Format("{0}.Configuration.configPage.html", GetType().Namespace)
            //    }
            //};
        }
    }
}