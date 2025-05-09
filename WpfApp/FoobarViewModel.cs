using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Shapes;
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

    public Interaction<string?, string?> FolderSelection { get; }
    
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
        RunJob = ReactiveCommand.CreateRunInBackground<string>(Run, canExecuteJob);
        
        // https://www.reactiveui.net/docs/handbook/interactions/
        FolderSelection = new Interaction<string?, string?>();
        BrowseFolder = ReactiveCommand.CreateFromTask(OnBrowseFolder);
        
        RunJob.Subscribe(_ =>
        {
            Console.WriteLine("From subscriber: run job finished");
        });
    }

    private void Run(string timeAsString)
    {
        Console.WriteLine($"Running!\t{timeAsString}");
        Thread.Sleep(3000);
        Console.WriteLine($"Ended\t\t{timeAsString}");
    }

    private async Task OnBrowseFolder()
    {
        Console.WriteLine("Hello from OnBrowseFolder");
        var folder = await FolderSelection.Handle(Directory.Exists(SomeText) ? SomeText : null);
        if (folder != null)
        {
            SomeText = folder;
        }
        
        Console.WriteLine("Bye from OnBrowseFolder");
    }
}
