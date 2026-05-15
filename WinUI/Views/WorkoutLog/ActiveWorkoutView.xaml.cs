using System.Net.Http;
using ClassLibrary.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ClassLibrary.Proxies;
using WinUI.ViewModels;

namespace WinUI.Views.WorkoutLog;

public sealed partial class ActiveWorkoutView : Page
{
    public ActiveWorkoutViewModel ViewModel { get; }
    public int ClientId { get; private set; }

    public ActiveWorkoutView()
    {
        ViewModel = new ActiveWorkoutViewModel(
            new ActiveWorkoutProxy(new HttpClient()),
            new WorkoutUiState());
        ViewModel.WorkoutFinished += () => Frame.GoBack();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is int clientId)
        {
            ClientId = clientId;
        }
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.LoadNotificationsCommand.Execute(ClientId);
        ViewModel.LoadCustomWorkouts(ClientId);
    }

    private void GoalRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton rb && rb.Tag is string goal)
        {
            ViewModel.SelectedGoal = goal;
        }
    }

    private void ApplyGoalsButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ApplyTargetGoalsCommand.Execute(ClientId);
    }

    private void StartCustomWorkout_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is WorkoutTemplate template)
        {
            ViewModel.SelectCustomWorkoutCommand.Execute(template);
        }
    }

    private async void CreateCustomWorkout_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            Title = "Create Custom Workout",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.None,
            XamlRoot = XamlRoot,
        };

        var createView = new CreateWorkoutView(ClientId);
        createView.WorkoutSaved += () => dialog.Hide();
        dialog.Content = createView;

        await dialog.ShowAsync();
        ViewModel.LoadCustomWorkouts(ClientId);
    }

    private async void OpenFocusMode_Click(object sender, RoutedEventArgs e)
    {
        await ShowFocusMode();
    }

    private async Task ShowFocusMode()
    {
        var dialog = new ContentDialog
        {
            Title = string.Empty,
            DefaultButton = ContentDialogButton.None,
            XamlRoot = XamlRoot,
        };

        var focusView = new FocusModeView(ViewModel);
        focusView.ExitRequested += () => dialog.Hide();
        dialog.Content = focusView;

        await dialog.ShowAsync();
    }

    private void ConfirmDeloadButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Notification notification)
        {
            ViewModel.ConfirmDeloadCommand.Execute(notification);
        }
    }
}

