namespace WpfApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        
        var viewModel = new FoobarViewModel();
        Control1.ViewModel = viewModel;
        Control2.ViewModel = viewModel;
    }
}