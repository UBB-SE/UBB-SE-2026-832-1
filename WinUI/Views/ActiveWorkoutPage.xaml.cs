using System;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using ClassLibrary.Models;
using WinUI.Services;

namespace WinUI.Views;

public sealed partial class ActiveWorkoutPage : Page
{
    public object? ViewModel { get; }
    public int ClientId { get; private set; }
    private bool isFocusDialogOpen;

    public ActiveWorkoutPage()
    {
        this.InitializeComponent();

        // Stub: viewmodel will be implemented later
        this.ViewModel = null;
        this.DataContext = this.ViewModel;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is int clientId && clientId != 0)
        {
            this.ClientId = clientId;
        }
        else
        {
            var userSession = new UserSession();
            this.ClientId = userSession.CurrentClientId;
        }
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (this.ViewModel == null)
        {
            return;
        }
    }

    private async void OpenFocusMode_Click(object sender, RoutedEventArgs e)
    {
        if (this.ViewModel == null || this.isFocusDialogOpen)
        {
            return;
        }

        var dialog = new ContentDialog
        {
            XamlRoot = this.Content.XamlRoot,
            FullSizeDesired = true,
            IsPrimaryButtonEnabled = false,
            IsSecondaryButtonEnabled = false
        };

        var focusPage = new FocusModeView(this.ViewModel, dialog);
        dialog.Content = focusPage;
        this.isFocusDialogOpen = true;
        try
        {
            await dialog.ShowAsync();
        }
        finally
        {
            this.isFocusDialogOpen = false;
        }
    }

    private void GoalRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        if (this.ViewModel == null)
        {
            return;
        }

        if (sender is RadioButton rb && rb.Tag is string goal)
        {
            // Set goal on viewmodel when available
        }
    }

    private async void CreateCustomWorkout_Click(object sender, RoutedEventArgs e)
    {
        if (this.ViewModel == null)
        {
            return;
        }

        var createView = new CreateWorkoutView();

        var dialog = new ContentDialog
        {
            XamlRoot = this.Content.XamlRoot,
            Title = "Create Custom Workout",
            Content = createView,
            PrimaryButtonText = "Save Routine",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            MinWidth = 700
        };

        dialog.PrimaryButtonClick += (d, args) =>
        {
            args.Cancel = true;
        };

        await dialog.ShowAsync();
    }

    private void StartCustomWorkout_Click(object sender, RoutedEventArgs e)
    {
        if (this.ViewModel == null)
        {
            return;
        }

        if (sender is Button btn && btn.Tag is WorkoutTemplate template)
        {
            // Execute SelectCustomWorkoutCommand when viewmodel available
        }
    }

    private void ApplyGoalsButton_Click(object sender, RoutedEventArgs e)
    {
        this.TargetGoalsButton.Flyout.Hide();
        if (this.ViewModel == null)
        {
            return;
        }
    }

    private void ConfirmDeloadButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.ViewModel == null)
        {
            return;
        }

        if (sender is Button btn && btn.Tag is Notification notification)
        {
            // Execute ConfirmDeloadCommand when viewmodel available
        }
    }
}
