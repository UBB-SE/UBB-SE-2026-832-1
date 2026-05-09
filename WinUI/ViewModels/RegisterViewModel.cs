using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using WinUI.Services;

namespace WinUI.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IUserService? userService;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    private bool isTrainer;
    public bool IsTrainer
    {
        get => isTrainer;
        set => SetProperty(ref isTrainer, value);
    }

    private bool isNutritionist;
    public bool IsNutritionist
    {
        get => isNutritionist;
        set => SetProperty(ref isNutritionist, value);
    }

    public event EventHandler? RegistrationSuccessful;
    public event EventHandler? NavigateToUserData;
    public event EventHandler? NavigateToLogin;

    public RegisterViewModel()
    {
    }

    public RegisterViewModel(IUserService userService)
    {
        this.userService = userService;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (this.userService == null) return;

        if (await this.userService.CheckIfUsernameExistsAsync(this.Username))
        {
            return;
        }

        string role = "Client";
        if (this.IsTrainer)
        {
            role = "Trainer";
        }
        else if (this.IsNutritionist)
        {
            role = "Nutritionist";
        }

        var user = await this.userService.RegisterAsync(this.Username, this.Password, role);

        if (user != null)
        {
            UserSession.UserId = user.Id;
            UserSession.Role = role;

            if (role == "Client")
            {
                this.NavigateToUserData?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                this.RegistrationSuccessful?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    [RelayCommand]
    private void GoToLogin()
    {
        this.NavigateToLogin?.Invoke(this, EventArgs.Empty);
    }
}