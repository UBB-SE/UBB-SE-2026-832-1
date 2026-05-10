using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class UserDataView : Page
{
    public UserViewModel ViewModel { get; private set; }

    public UserDataView()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        this.ViewModel = (UserViewModel)e.Parameter;
        this.DataContext = this.ViewModel;

        this.ViewModel.SaveDataSuccess += (s, ev) =>
            this.Frame.Navigate(typeof(MainWindowView));
    }
}