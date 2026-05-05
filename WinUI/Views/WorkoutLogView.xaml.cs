using System.Linq;
using System.Net.Http;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI.Services;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class WorkoutLogView : Page
{
    public WorkoutLogViewModel ViewModel { get; }

    private readonly UserSession userSession;

    public WorkoutLogView()
    {
        this.userSession = new UserSession();
        this.ViewModel = new WorkoutLogViewModel(
            new WorkoutLogService(new WorkoutLogServiceProxy(new HttpClient())));
        this.DataContext = this.ViewModel;
        this.InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        var clientId = this.userSession.CurrentClientId;
        this.ViewModel.LoadLogsAsyncCommand.Execute(clientId);
    }

    private void ToggleEditMode_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not int id)
        {
            return;
        }

        var item = this.ViewModel.Logs.FirstOrDefault(logItem => logItem.Id == id);
        if (item is not null)
        {
            this.ViewModel.ToggleEditModeCommand.Execute(item);
        }
    }

    private void SaveEditedLog_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not int id)
        {
            return;
        }

        var item = this.ViewModel.Logs.FirstOrDefault(logItem => logItem.Id == id);
        if (item is not null)
        {
            this.ViewModel.SaveEditedLogCommand.Execute(item);
        }
    }
}
