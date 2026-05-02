using Microsoft.UI.Xaml.Controls;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class MainWindowView : Page
{
    public MainWindowViewModel ViewModel { get; }

    public MainWindowView()
    {
        this.ViewModel = new MainWindowViewModel();
        this.InitializeComponent();
    }
}
