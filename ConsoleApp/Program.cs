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

    readonly ObservableAsPropertyHelper<string> _timeAsString;
    public string TimeAsString => _timeAsString.Value;
    
    public ViewModel()
    {
        Time = DateTimeOffset.Now;
        Observable.Interval(TimeSpan.FromSeconds(1))
            .Select(_ => DateTimeOffset.Now)
            .Subscribe(time => Time = time);

        this.WhenAnyValue(_ => _.Time)
            .Select((time, counter) => time.ToString("HH:mm:ss") + $" ({counter})")
            .ToProperty(this, _ => _.TimeAsString, out _timeAsString);
    }
}
