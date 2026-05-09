using ClassLibrary.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WinUI.Services;

namespace WinUI.ViewModels;

public partial class UserDataViewModel : ObservableObject
{
    private readonly IUserService? userService;

    [ObservableProperty]
    private double weight;

    [ObservableProperty]
    private double height;

    [ObservableProperty]
    private DateTimeOffset birthdate = DateTimeOffset.Now;

    [ObservableProperty]
    private string selectedGender = string.Empty;

    [ObservableProperty]
    private string selectedGoal = string.Empty;

    public ObservableCollection<string> Genders { get; } = new() { "Male", "Female", "Other" };
    public ObservableCollection<string> Goals { get; } = new() { "Lose Weight", "Build Muscle", "Maintain" };

    public event EventHandler? DataSavedSuccessful;

    public UserDataViewModel()
    {
    }

    public UserDataViewModel(IUserService userService)
    {
        this.userService = userService;
    }

    [RelayCommand]
    private async Task SaveDataAsync()
    {
        if (this.userService == null) return;

        if (UserSession.UserId.HasValue)
        {
            var userDataDto = new UserDataDto
            {
                UserId = UserSession.UserId.Value,
                Weight = (int)this.Weight,
                Height = (int)this.Height,
                Age = DateTime.Now.Year - this.Birthdate.Year,
                Gender = this.SelectedGender,
                Goal = this.SelectedGoal
            };

            await this.userService.AddUserDataAsync(userDataDto);
            this.DataSavedSuccessful?.Invoke(this, EventArgs.Empty);
        }
    }
}