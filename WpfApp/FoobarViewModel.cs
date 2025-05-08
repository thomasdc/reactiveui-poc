using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace Wpf;

public class FoobarViewModel : ReactiveObject
{
    private DateTimeOffset _time;
    public DateTimeOffset Time
    {
        get => _time;
        private set => this.RaiseAndSetIfChanged(ref _time, value);
    }

    private readonly ObservableAsPropertyHelper<string> _timeAsString;
    public string TimeAsString => _timeAsString.Value;
    
    private string _someText;
    public string SomeText
    {
        get => _someText;
        private set => this.RaiseAndSetIfChanged(ref _someText, value);
    }
    
    public ReactiveCommand<string, Unit> RunJob { get; }

    public Interaction<Unit, string?> FolderSelection { get; }
    
    public ReactiveCommand<Unit, Unit> BrowseFolder { get; }

    public FoobarViewModel()
    {
        Time = DateTimeOffset.Now;
        Observable.Interval(TimeSpan.FromSeconds(1))
            .Select(_ => DateTimeOffset.Now)
            .Subscribe(time => Time = time);

        this.WhenAnyValue(_ => _.Time)
            .Select((time, counter) => time.ToString("HH:mm:ss") + $" ({counter})")
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, _ => _.TimeAsString, out _timeAsString);
        
        // https://www.reactiveui.net/docs/handbook/commands/#controlling-executability
        var canExecuteJob = this.WhenAnyValue(_ => _.Time)
            .Select(time => time.Second % 3 == 0)
            .ObserveOn(RxApp.MainThreadScheduler);
        canExecuteJob.DistinctUntilChanged()
            .Subscribe(allowed => Console.WriteLine("Can execute job: " + allowed));
        RunJob = ReactiveCommand.CreateFromTask<string>(Run, canExecuteJob);
        
        // https://www.reactiveui.net/docs/handbook/interactions/
        FolderSelection = new Interaction<Unit, string?>();
        BrowseFolder = ReactiveCommand.CreateFromTask(OnBrowseFolder);
    }

    private async Task Run(string timeAsString)
    {
        Console.WriteLine($"Running!\t{timeAsString}");
        await Task.Delay(3000);
        Console.WriteLine($"Ended\t\t{timeAsString}");
    }

    private async Task OnBrowseFolder()
    {
        Console.WriteLine("Hello from OnBrowseFolder");
        var folder = await FolderSelection.Handle(Unit.Default);
        if (folder != null)
        {
            SomeText = folder;
        }
        
        Console.WriteLine("Bye from OnBrowseFolder");
    }
}
