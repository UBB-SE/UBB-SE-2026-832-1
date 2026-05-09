using Microsoft.UI.Xaml.Controls;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class UserDataView : Page
{
    public UserDataViewModel ViewModel { get; }

    public UserDataView()
    {
        this.InitializeComponent();
        this.ViewModel = new UserDataViewModel();
    }
}