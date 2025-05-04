using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;

namespace WpfApp;

public partial class UserControl1
{
    public UserControl1()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyObservable(view => view.ViewModel!.RunJob.IsExecuting)
                .Where(isExecuting => isExecuting)
                .Scan(0, (acc, _) => acc + 1)
                .BindTo(this, view => view.TheFirstBox.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel,
                    viewModel => viewModel.SomeText,
                    view => view.TheThirdBox.Text)
                .DisposeWith(disposables);
        });
    }
}