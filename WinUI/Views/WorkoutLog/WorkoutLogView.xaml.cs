using Microsoft.UI.Xaml.Controls;

namespace WinUI.Views.WorkoutLog;

public sealed partial class WorkoutLogView : Page
{
    public WorkoutLogView()
    {
        InitializeComponent();
        Loaded += (_, _) => ContentFrame.Navigate(typeof(WorkoutHistoryPage));
    }
}
