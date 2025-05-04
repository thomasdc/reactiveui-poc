using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;

namespace Wpf;

public partial class UserControl2
{
    public UserControl2()
    {
        InitializeComponent();
        
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
            
            this.WhenAnyObservable(view => view.ViewModel!.RunJob.CanExecute)
                .BindTo(this, view => view.TheBoxCanExecute.Text)
                .DisposeWith(disposables);
            
            this.WhenAnyObservable(view => view.ViewModel!.RunJob.IsExecuting)
                .Select(isExecuting => isExecuting ? Visibility.Visible : Visibility.Hidden)
                .BindTo(this, view => view.TheProgressBar.Visibility)
                .DisposeWith(disposables);
            
            this.Bind(ViewModel,
                    viewModel => viewModel.SomeText,
                    view => view.TheThirdBox.Text)
                .DisposeWith(disposables);
        });
    }
}