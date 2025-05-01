using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using ReactiveUI;

namespace ReactiveUIPoc;

// https://www.reactiveui.net/docs/getting-started/compelling-example
public class AppViewModel : ReactiveObject
{
    private string? _searchTerm;
    public string? SearchTerm
    {
        get => _searchTerm;
        set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
    }

    private readonly ObservableAsPropertyHelper<IEnumerable<NugetDetailsViewModel>> _searchResults;
    public IEnumerable<NugetDetailsViewModel> SearchResults => _searchResults.Value;

    private readonly ObservableAsPropertyHelper<bool> _isAvailable;
    public bool IsAvailable => _isAvailable.Value;

    public AppViewModel()
    {
        _searchResults = this
            .WhenAnyValue(_ => _.SearchTerm)
            .Throttle(TimeSpan.FromMicroseconds(800))
            .Select(_ => _?.Trim())
            .DistinctUntilChanged()
            .Where(_ => !string.IsNullOrWhiteSpace(_))
            .SelectMany(SearchNuGetPackages)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, _ => _.SearchResults);
        
        _searchResults.ThrownExceptions.Subscribe(Console.WriteLine);

        _isAvailable = this
            .WhenAnyValue(_ => _.SearchResults)
            .Select(searchResults => searchResults != null)
            .ToProperty(this, _ => _.IsAvailable);
    }

    private async Task<IEnumerable<NugetDetailsViewModel>> SearchNuGetPackages(string? term, CancellationToken token)
    {
        var providers = new List<Lazy<INuGetResourceProvider>>();
        providers.AddRange(Repository.Provider.GetCoreV3());
        var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
        var source = new SourceRepository(packageSource, providers);

        var filter = new SearchFilter(false);
        var resource = await source.GetResourceAsync<PackageSearchResource>(token).ConfigureAwait(false);
        var metadata = await resource.SearchAsync(term, filter, 0, 10, NullLogger.Instance, token).ConfigureAwait(false);
        return metadata.Select(_ => new NugetDetailsViewModel(_));
    }
}

public class NugetDetailsViewModel : ReactiveObject
{
    private readonly IPackageSearchMetadata _metadata;
    private readonly Uri _defaultUrl;
    public Uri IconUrl => _metadata.IconUrl ?? _defaultUrl;
    public string Description => _metadata.Description;
    public Uri ProjectUrl => _metadata.ProjectUrl;
    public string Title => _metadata.Title;
    
    public ReactiveCommand<Unit, Unit> OpenPage { get; }

    public NugetDetailsViewModel(IPackageSearchMetadata metadata)
    {
        _metadata = metadata;
        _defaultUrl = new Uri("https://git.io/fAlfh");
        OpenPage = ReactiveCommand.Create(() =>
        {
            Process.Start(new ProcessStartInfo(this.ProjectUrl.ToString())
            {
                UseShellExecute = true
            });
        });
    }
}
