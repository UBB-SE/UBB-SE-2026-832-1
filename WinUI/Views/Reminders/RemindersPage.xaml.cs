using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using WinUI.ViewModels;
using ClassLibrary.DTOs;

namespace WinUI.Views.Reminders;

public sealed partial class RemindersPage : Page
{
    public RemindersViewModel ViewModel { get; }

    public RemindersPage()
    {
        this.InitializeComponent();

        this.ViewModel = App.Services.GetRequiredService<RemindersViewModel>();
        this.DataContext = this.ViewModel;

        this.Loaded += async (s, e) => await this.ViewModel.LoadRemindersAsync();
    }

    private async void OnDeleteClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: ReminderDto reminder })
        {
            ContentDialog deleteDialog = new()
            {
                Title = "Delete Reminder",
                Content = "Are you sure you want to delete this reminder?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };

            if (await deleteDialog.ShowAsync() == ContentDialogResult.Primary)
            {
                await this.ViewModel.DeleteReminderAsync(reminder);
            }
        }
    }
}