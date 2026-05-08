using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace WinUI.Views;

public sealed partial class TrainerDashboardView : Page
{
    public object? ViewModel { get; }

    private bool isDialogOpen = false;

    public static string FormatWorkoutDate(DateTime date)
    {
        return date.ToString("MMM dd, yyyy");
    }

    public TrainerDashboardView()
    {
        this.InitializeComponent();

        // Stub: viewmodel will be implemented later
        this.ViewModel = null;
        this.DataContext = this.ViewModel;
    }

    private async void OpenBuilderButton_Click(object sender, RoutedEventArgs eventArgs)
    {
        if (this.isDialogOpen || this.ViewModel == null)
        {
            return;
        }

        this.WorkoutBuilderDialog.XamlRoot = this.Content.XamlRoot;
        this.isDialogOpen = true;
        await this.WorkoutBuilderDialog.ShowAsync();
        this.isDialogOpen = false;
    }

    private void RemoveExercise_Click(object sender, RoutedEventArgs eventArgs)
    {
        if (this.ViewModel == null)
        {
            return;
        }

        var button = (Button)sender;
    }

    private void WorkoutBuilderDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (this.ViewModel == null)
        {
            args.Cancel = true;
            return;
        }
    }

    private async void DeleteWorkout_Tapped(object sender, TappedRoutedEventArgs eventArgs)
    {
        if (this.ViewModel == null || this.isDialogOpen)
        {
            return;
        }

        eventArgs.Handled = true;

        var button = (Button)sender;
        var workout = button.DataContext;

        if (workout == null)
        {
            return;
        }

        ContentDialog confirmDelete = new ContentDialog
        {
            Title = "Delete Routine?",
            Content = "Are you sure you want to remove this routine?",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = button.XamlRoot,
        };

        this.isDialogOpen = true;
        var result = await confirmDelete.ShowAsync();
        this.isDialogOpen = false;
    }

    private async void Card_Tapped(object sender, TappedRoutedEventArgs eventArgs)
    {
        if (this.ViewModel == null || this.isDialogOpen || eventArgs.Handled)
        {
            return;
        }

        var grid = sender as Grid;
        var workout = grid?.DataContext;

        if (workout == null)
        {
            return;
        }

        this.WorkoutBuilderDialog.Title = "Edit Routine";
        this.WorkoutBuilderDialog.XamlRoot = this.Content.XamlRoot;

        this.isDialogOpen = true;
        await this.WorkoutBuilderDialog.ShowAsync();
        this.isDialogOpen = false;
    }
}
