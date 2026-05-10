using ClassLibrary.DTOs;
using ClassLibrary.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WinUI.Services;

namespace WinUI.ViewModels;

public sealed partial class RemindersViewModel : ObservableObject
{
    private readonly IRemindersService remindersService;
    private readonly int currentUserId;

    [ObservableProperty]
    private ObservableCollection<ReminderDto> reminders = new();

    [ObservableProperty]
    private ReminderDto? nextReminder;

    [ObservableProperty]
    private bool isBusy;

    public RemindersViewModel(IRemindersService remindersService)
    {
        this.remindersService = remindersService;
        this.currentUserId = 1;
    }

    [RelayCommand]
    public async Task LoadRemindersAsync()
    {
        IsBusy = true;
        try
        {
            var list = await this.remindersService.GetUserRemindersAsync(currentUserId);
            Reminders = new ObservableCollection<ReminderDto>(list);
            NextReminder = await this.remindersService.GetNextReminderAsync(currentUserId);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeleteReminderAsync(ReminderDto reminder)
    {
        await this.remindersService.DeleteReminderAsync(reminder.Id);
        Reminders.Remove(reminder);
        NextReminder = await this.remindersService.GetNextReminderAsync(currentUserId);
    }

    [RelayCommand]
    public async Task ConfirmConsumptionAsync(ReminderDto reminder)
    {
       
        await Task.Delay(100);
        await LoadRemindersAsync();
    }
}