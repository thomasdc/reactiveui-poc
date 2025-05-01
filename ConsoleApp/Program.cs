using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

// https://www.youtube.com/watch?v=IH2yx7b9DNY
var viewModel = new ViewModel();
viewModel.WhenAnyValue(_ => _.TimeAsString)
    .Subscribe(Console.WriteLine);

Console.ReadLine();

public class ViewModel : ReactiveObject
{
    private DateTimeOffset _time;
    public DateTimeOffset Time
    {
        get => _time;
        private set => this.RaiseAndSetIfChanged(ref _time, value);
    }

    private readonly ObservableAsPropertyHelper<string> _timeAsString;
    public string TimeAsString => _timeAsString.Value;
    
    public ReactiveCommand<string, Unit> RunJob { get; }
    
    public ViewModel()
    {
        Time = DateTimeOffset.Now;
        Observable.Interval(TimeSpan.FromSeconds(1))
            .Select(_ => DateTimeOffset.Now)
            .Subscribe(time => Time = time);

        this.WhenAnyValue(_ => _.Time)
            .Select((time, counter) => time.ToString("HH:mm:ss") + $" ({counter})")
            .ToProperty(this, _ => _.TimeAsString, out _timeAsString);
        
        // https://www.reactiveui.net/docs/handbook/commands/#controlling-executability
        var canExecuteJob = this.WhenAnyValue(_ => _.Time)
            .Select(time => time.Second % 3 == 0);
        canExecuteJob.DistinctUntilChanged()
            .Subscribe(allowed => Console.WriteLine("Can execute job: " + allowed));
        RunJob = ReactiveCommand.CreateFromTask<string>(Run, canExecuteJob);

        this.WhenAnyValue(_ => _.TimeAsString)
            .InvokeCommand(RunJob);
    }

    public async Task Run(string timeAsString)
    {
        Console.WriteLine($"Running!\t{timeAsString}");
        await Task.Delay(100);
        Console.WriteLine($"Ended\t\t{timeAsString}");
    }
}
