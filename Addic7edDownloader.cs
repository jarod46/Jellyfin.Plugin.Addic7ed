using MediaBrowser.Common;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Subtitles;
using MediaBrowser.Model.Globalization;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Providers;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Json;
using Jellyfin.Plugin.Addic7ed.GestdownAPI;
using Microsoft.AspNetCore.Http.Json;

namespace Jellyfin.Plugin.Addic7ed
{
    public class Addic7edDownloader : ISubtitleProvider
    {

        private readonly ILogger<Addic7edDownloader> _logger;
        private readonly HttpClient _httpClient;
        private ILocalizationManager _localizationManager;

        private readonly string _baseUrl = "https://api.gestdown.info/";

        public Addic7edDownloader(ILogger<Addic7edDownloader> logger, IHttpClientFactory httpClientFactory, ILocalizationManager localizationManager)
        {
            _logger = logger;

            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!.ToString();

            _httpClient = httpClientFactory.CreateClient();
            _localizationManager = localizationManager;
        }

        public string Name => "Addic7ed/Gestdown Subtitles";

        IEnumerable<VideoContentType> ISubtitleProvider.SupportedMediaTypes => new[] { VideoContentType.Episode };

        async Task<SubtitleResponse> ISubtitleProvider.GetSubtitles(string id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Try to get subs...");
            if (lastSearchResponse != null && lastSearchResponse.MatchingSubtitles != null)
            {
                var relatedSubs = lastSearchResponse.MatchingSubtitles.FirstOrDefault(s => s.SubtitleId == id);
                if (relatedSubs != null)
                {
                    _logger.LogInformation("Download subtitiles request : " + _baseUrl + "subtitles/download/" + id);
                    var response = await _httpClient.GetAsync(_baseUrl + "subtitles/download/" + id, cancellationToken).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        return new SubtitleResponse
                        {
                            Language = lastRequestLanguage,
                            Format = "srt",
                            IsForced = false,
                            Stream = response.Content.ReadAsStream(),
                        };
                    }
                    else
                    {
                        var failResponse = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        throw new HttpRequestException(failResponse);
                    }
                }
            }
            _logger.LogError("Subtitiles infos to download not found...");
            return new SubtitleResponse();
            
        }

        static SubtitleSearchResponse? lastSearchResponse = null;
        static string lastRequestLanguage = string.Empty;
        async Task<IEnumerable<RemoteSubtitleInfo>> ISubtitleProvider.Search(SubtitleSearchRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Look for show on addic7ed...");
            lastSearchResponse = null;
            lastRequestLanguage = string.Empty;
            var showQueryResult = await _httpClient.GetAsync(_baseUrl + "shows/search/" + request.SeriesName, cancellationToken);
            var subInfosList = new List<RemoteSubtitleInfo>();
            var culture = _localizationManager.FindLanguageInfo(request.Language);
            lastRequestLanguage = request.Language;
            var fullLangName = culture?.DisplayName.ToLower();
            if (showQueryResult.IsSuccessStatusCode)
            {
                var showsInfos = await showQueryResult.Content.ReadFromJsonAsync<ShowSearchResponse>(cancellationToken: cancellationToken);
                
                if (showsInfos != null && showsInfos.Shows != null && request.ParentIndexNumber != null && request.IndexNumber != null)
                {
                    var season = request.ParentIndexNumber.Value.ToString();
                    var episode = request.IndexNumber.Value.ToString();
                    foreach (var show in showsInfos.Shows)
                    {
                        _logger.LogInformation("Found show : " + show.Name);
                        _logger.LogInformation("Query : " + _baseUrl + "subtitles/get/" + show.Id + "/" + season + "/" + episode + "/" + fullLangName);
                        showQueryResult = await _httpClient.GetAsync(_baseUrl + "subtitles/get/" + show.Id + "/" + season + "/" + episode + "/" + fullLangName);
                        lastSearchResponse = await showQueryResult.Content.ReadFromJsonAsync<SubtitleSearchResponse>(cancellationToken: cancellationToken);
                        if (lastSearchResponse != null && lastSearchResponse.MatchingSubtitles != null)
                        {
                            foreach (var subtitle in lastSearchResponse.MatchingSubtitles)
                            {
                                _logger.LogInformation("Sub found : " + subtitle.Version);
                                var subInfos = new RemoteSubtitleInfo
                                {
                                    Author = "Multiples",
                                    Comment = string.Empty,
                                    DownloadCount = subtitle.DownloadCount,
                                    Format = "srt",
                                    ProviderName = Name,
                                    ThreeLetterISOLanguageName = request.Language,
                                    Id = subtitle.SubtitleId,
                                    Name = "[" + subtitle.Version + "] " + lastSearchResponse?.Episode?.Show + " | " + lastSearchResponse?.Episode?.Title + " | " + (subtitle.Completed ? "Completed" : "UnCompleted"),
                                    DateCreated = subtitle.Discovered != null ? DateTime.Parse(subtitle.Discovered) : DateTime.MinValue,
                                };
                                subInfosList.Add(subInfos);
                            }
                        }
                    }                        
                }
            }
            return subInfosList;
        }
    }
}
