using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ClassLibrary.Proxies;
using ClassLibrary.Proxies.Interfaces;
using WinUI.ViewModels.CalendarIntegration;
using Windows.Storage.Pickers;

namespace WinUI.Views.CalendarIntegration;

public sealed partial class CalendarIntegrationPage : Page
{
    private const string CLIENT_ROLE = "Client";
    private readonly CalendarIntegrationViewModel viewModel;
    private readonly IUserSession userSession;

    public CalendarIntegrationPage()
    {
        this.InitializeComponent();
        this.userSession = new UserSession();
        this.viewModel = new CalendarIntegrationViewModel(
            new CalendarWorkoutCatalogProxy(new HttpClient()),
            new CalendarExportProxy(),
            this.userSession);
        this.DataContext = this.viewModel;
        this.GenerateCalendarButton.Click += this.GenerateCalendarButton_Click;
        this.Loaded += this.CalendarIntegrationPage_Loaded;
    }

    private async void CalendarIntegrationPage_Loaded(object sender, RoutedEventArgs eventArgs)
    {
        bool hasClientRole = this.userSession.IsClient;
        if (!hasClientRole || this.userSession.CurrentClientId <= 0)
        {
            this.viewModel.SetErrorStatus("Calendar integration is available only for client users.");
            this.GenerateCalendarButton.IsEnabled = false;
            return;
        }

        await this.viewModel.EnsureWorkoutsLoadedAsync();
    }

    private async void GenerateCalendarButton_Click(object sender, RoutedEventArgs eventArgs)
    {
        await ExecuteCalendarGenerationAndSaveFlowAsync();
    }

    private async Task ExecuteCalendarGenerationAndSaveFlowAsync()
    {
        try
        {
            this.GenerateCalendarButton.IsEnabled = false;
            this.viewModel.ClearStatus();

            CalendarIntegrationViewModel.CalendarGenerationResult calendarGenerationResult = await this.viewModel.GenerateCalendarForExportAsync();
            if (!calendarGenerationResult.IsSuccessful)
            {
                this.viewModel.SetErrorStatus(calendarGenerationResult.Message);
                return;
            }

            string generatedCalendarContent = calendarGenerationResult.GeneratedCalendarContent;
            await SaveGeneratedCalendarContentWithPickerAsync(generatedCalendarContent);
        }
        catch (InvalidOperationException exception)
        {
            this.viewModel.SetErrorStatus(exception.Message);
        }
        catch (Exception exception)
        {
            await HandleCalendarSaveExceptionAsync(exception);
        }
        finally
        {
            this.GenerateCalendarButton.IsEnabled = true;
        }
    }

    private async Task SaveGeneratedCalendarContentWithPickerAsync(string generatedCalendarContent)
    {
        FileSavePicker calendarFileSavePicker = CreateCalendarFileSavePicker();
        if (!TryGetApplicationWindowHandle(out IntPtr applicationWindowHandle))
        {
            return;
        }

        WinRT.Interop.InitializeWithWindow.Initialize(calendarFileSavePicker, applicationWindowHandle);
        var selectedStorageFile = await calendarFileSavePicker.PickSaveFileAsync();
        if (selectedStorageFile == null)
        {
            return;
        }

        await Windows.Storage.FileIO.WriteTextAsync(selectedStorageFile, generatedCalendarContent);
        this.viewModel.SetSuccessStatus($"Calendar file '{selectedStorageFile.Name}' saved successfully! You can now import it into your calendar application.");
    }

    private static FileSavePicker CreateCalendarFileSavePicker()
    {
        FileSavePicker calendarFileSavePicker = new FileSavePicker();
        calendarFileSavePicker.SuggestedStartLocation = PickerLocationId.Downloads;
        calendarFileSavePicker.FileTypeChoices.Add("iCalendar", new List<string> { ".ics" });
        return calendarFileSavePicker;
    }

    private bool TryGetApplicationWindowHandle(out IntPtr applicationWindowHandle)
    {
        applicationWindowHandle = GetActiveWindow();
        if (applicationWindowHandle == IntPtr.Zero)
        {
            this.viewModel.SetErrorStatus("Unable to initialize save dialog window handle.");
            return false;
        }

        return true;
    }

    private async Task HandleCalendarSaveExceptionAsync(Exception calendarSaveException)
    {
        if (calendarSaveException is COMException)
        {
            string? fallbackCalendarPath = await this.viewModel.SaveGeneratedCalendarToDownloadsFallbackAsync();
            if (!string.IsNullOrWhiteSpace(fallbackCalendarPath))
            {
                this.viewModel.SetSuccessStatus($"Save dialog unavailable. Calendar saved to: {fallbackCalendarPath}");
            }
            else
            {
                this.viewModel.SetErrorStatus("Error saving calendar file: could not open the save dialog.");
            }

            return;
        }

        this.viewModel.SetErrorStatus($"Error saving calendar file: {calendarSaveException.Message}");
    }

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern IntPtr GetActiveWindow();
}

