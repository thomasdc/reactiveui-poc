using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;

namespace WpfApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new FoobarViewModel();
        
        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel,
                    viewModel => viewModel.TimeAsString,
                    view => view.TheBox.Text)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel,
                viewModel => viewModel.RunJob,
                view => view.TheButton,
                viewModel => viewModel.TimeAsString)
                .DisposeWith(disposables);
            
            this.WhenAnyObservable(view => view.ViewModel!.RunJob.IsExecuting)
                .Select(isExecuting => isExecuting ? Visibility.Visible : Visibility.Hidden)
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, view => view.TheProgressBar.Visibility)
                .DisposeWith(disposables);
        });
    }
}