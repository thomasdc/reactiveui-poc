using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using Microsoft.Win32;
using ReactiveUI;

namespace WpfApp;

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

            ViewModel!.FolderSelection.RegisterHandler(interaction =>
            {
                var folderDialog = new OpenFolderDialog
                {
                    Title = "Select folder...",
                };

                if (!string.IsNullOrWhiteSpace(interaction.Input))
                {
                    folderDialog.InitialDirectory = interaction.Input;
                }

                interaction.SetOutput(folderDialog.ShowDialog() == true ? folderDialog.FolderName : null);
            });
            
            this.BindCommand(ViewModel,
                    viewModel => viewModel.BrowseFolder,
                    view => view.TheButton2)
                .DisposeWith(disposables);
        });
    }
}