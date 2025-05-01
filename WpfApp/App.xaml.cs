using System.Configuration;
using System.Data;
using System.Reflection;
using System.Windows;
using ReactiveUI;
using Splat;

namespace WpfApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
    }
}