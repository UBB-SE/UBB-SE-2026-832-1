using Microsoft.UI.Xaml.Controls;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class RegisterView : Page
{
    public RegisterViewModel ViewModel { get; }

    public RegisterView()
    {
        this.InitializeComponent();
        this.ViewModel = new RegisterViewModel();
    }
}