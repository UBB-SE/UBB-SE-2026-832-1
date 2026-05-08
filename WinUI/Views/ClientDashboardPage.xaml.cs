using System.ComponentModel;
using LiveChartsCore.SkiaSharpView.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class ClientDashboardPage : Page
{
    private CartesianChart? chart;

    public ClientDashboardViewModel? ViewModel { get; private set; }

    public ClientDashboardPage()
    {
        try
        {
            ViewModel = App.GetService<ClientDashboardViewModel>();
        }
        catch
        {
            // Service not registered; stub will be null
        }

        DataContext = ViewModel;
        InitializeComponent();

        if (ViewModel != null)
        {
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        Unloaded += Page_Unloaded;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel == null)
            return;

        // Initialize chart
        InitializeChart();

        // Load dashboard data
        await ViewModel.LoadAllAsync();

        // Check for progression updates
        try
        {
            var workoutState = App.GetService<IWorkoutUiState>();
            if (workoutState != null)
            {
                var note = workoutState.ProgressionHeadsUp;
                if (!string.IsNullOrWhiteSpace(note))
                {
                    ProgressionInfoBar.Message = note;
                    ProgressionInfoBar.IsOpen = true;
                    workoutState.ProgressionHeadsUp = null;
                }
            }
        }
        catch
        {
            // Service not available
        }
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (chart != null && ChartContainer.Children.Contains(chart))
        {
            ChartContainer.Children.Remove(chart);
        }
    }

    private void InitializeChart()
    {
        chart = new CartesianChart
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent)
        };

        SyncChartToViewModel();
        ChartContainer.Children.Add(chart);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ClientDashboardViewModel.ChartSeries) or nameof(ClientDashboardViewModel.ChartXAxes))
        {
            SyncChartToViewModel();
        }
    }

    private void SyncChartToViewModel()
    {
        if (chart == null || ViewModel == null)
            return;

        chart.Series = ViewModel.ChartSeries;
        chart.XAxes = ViewModel.ChartXAxes;
    }
}

/// <summary>
/// Stub interface for workout UI state notifications. Implement if not already defined.
/// </summary>
public interface IWorkoutUiState
{
    string? ProgressionHeadsUp { get; set; }
}
