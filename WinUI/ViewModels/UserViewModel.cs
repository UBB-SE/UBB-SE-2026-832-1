using System;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using ClassLibrary.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WinUI.Services;

namespace WinUI.ViewModels;

public partial class UserViewModel : ObservableObject
{
    private const string ROLE_NUTRITIONIST = "Nutritionist";
    private const string ROLE_TRAINER = "Trainer";
    private const string ROLE_USER = "Client";

    private const string ERROR_USERNAME_EXISTS = "Username already exists.";
    private const string ERROR_INVALID_BIRTHDATE = "Please select a valid birthdate.";
    private const string ERROR_REGISTRATION_FAILED = "Registration failed.";
    private const string ERROR_USERNAME_PASSWORD_REQUIRED = "Username and Password are required.";
    private const string ERROR_INVALID_CREDENTIALS = "Invalid username or password.";
    private const string ERROR_SAVING_DATA_FORMAT = "Error: {0}";
    private const string ERROR_VALIDATION_LENGTH = "Username (min 3) and Password (min 6) required.";

    private readonly IUserService? userService;

    
    [ObservableProperty] private string userName = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private DateTimeOffset selectedDate = DateTimeOffset.Now;
    [ObservableProperty] private int weight;
    [ObservableProperty] private int height;
    [ObservableProperty] private string gender = string.Empty;
    [ObservableProperty] private string goal = string.Empty;

    public List<string> GenderOptions { get; } = new() { "female", "male" };
    public List<string> GoalOptions { get; } = new() { "cut", "bulk", "maintenance", "well-being" };

    private bool isTrainer;
    public bool IsTrainer
    {
        get => isTrainer;
        set
        {
            if (SetProperty(ref isTrainer, value) && value)
                IsNutritionist = false;
        }
    }

    private bool isNutritionist;
    public bool IsNutritionist
    {
        get => isNutritionist;
        set
        {
            if (SetProperty(ref isNutritionist, value) && value)
                IsTrainer = false;
        }
    }

    public int? LoggedInUserId { get; private set; }
    public string? LoggedInUsername { get; private set; }

    public event EventHandler? RegistrationValid;
    public event EventHandler? LoginSuccess;
    public event EventHandler? SaveDataSuccess;
    public event EventHandler? NavigateToRegister;
    public event EventHandler? NavigateToLogin;

    public UserViewModel() { }

    public UserViewModel(IUserService userService)
    {
        this.userService = userService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (this.userService == null) return;
        this.StatusMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(this.UserName) || string.IsNullOrWhiteSpace(this.Password))
        {
            this.StatusMessage = ERROR_USERNAME_PASSWORD_REQUIRED;
            return;
        }

        try
        {
            var user = await this.userService.LoginAsync(this.UserName, this.Password);
            if (user != null)
            {
                this.LoggedInUserId = user.Id;
                this.LoggedInUsername = user.Username;
                UserSession.SetCurrentSession(user.Id, user.Role);
                this.LoginSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                this.StatusMessage = ERROR_INVALID_CREDENTIALS;
            }
        }
        catch (Exception ex)
        {
            this.StatusMessage = string.Format(ERROR_SAVING_DATA_FORMAT, ex.Message);
        }
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (this.userService == null) return;
        this.StatusMessage = string.Empty;

        if (this.UserName.Length < 3 || this.Password.Length < 6)
        {
            this.StatusMessage = ERROR_VALIDATION_LENGTH;
            return;
        }

        if (await this.userService.CheckIfUsernameExistsAsync(this.UserName))
        {
            this.StatusMessage = ERROR_USERNAME_EXISTS;
            return;
        }

        string role = GetSelectedRole();

        if (role == ROLE_USER)
        {
            this.RegistrationValid?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            var registered = await this.userService.RegisterAsync(this.UserName, this.Password, role);
            if (registered != null)
            {
                this.LoggedInUserId = registered.Id;
                this.LoggedInUsername = registered.Username;
                UserSession.SetCurrentSession(registered.Id, registered.Role);
                this.LoginSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                this.StatusMessage = ERROR_REGISTRATION_FAILED;
            }
        }
    }

    [RelayCommand]
    private async Task SaveDataAsync()
    {
        if (this.userService == null) return;
        this.StatusMessage = string.Empty;

        try
        {
            int age = NutritionCalculator.CalculateAge(this.SelectedDate);
            if (age <= 0)
            {
                this.StatusMessage = ERROR_INVALID_BIRTHDATE;
                return;
            }

            var registered = await this.userService.RegisterAsync(this.UserName, this.Password, ROLE_USER);
            if (registered == null)
            {
                this.StatusMessage = ERROR_REGISTRATION_FAILED;
                return;
            }

            this.LoggedInUserId = registered.Id;
            this.LoggedInUsername = registered.Username;
            UserSession.SetCurrentSession(registered.Id, registered.Role);

            var userDataDto = new UserDataDto
            {
                UserId = registered.Id,
                Weight = this.Weight,
                Height = this.Height,
                Age = age,
                Gender = this.Gender,
                Goal = this.Goal,
            };

            await this.userService.AddUserDataAsync(userDataDto);
            this.SaveDataSuccess?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            this.StatusMessage = string.Format(ERROR_SAVING_DATA_FORMAT, ex.Message);
        }
    }

    [RelayCommand] private void GoToRegister() => this.NavigateToRegister?.Invoke(this, EventArgs.Empty);
    [RelayCommand] private void GoToLogin() => this.NavigateToLogin?.Invoke(this, EventArgs.Empty);
    [RelayCommand]
    private void Logout()
    {
        UserSession.Clear();
        this.LoggedInUserId = null;
        this.LoggedInUsername = null;
        this.NavigateToLogin?.Invoke(this, EventArgs.Empty);
    }
    private string GetSelectedRole()
    {
        if (this.IsTrainer) return ROLE_TRAINER;
        if (this.IsNutritionist) return ROLE_NUTRITIONIST;
        return ROLE_USER;
    }
}