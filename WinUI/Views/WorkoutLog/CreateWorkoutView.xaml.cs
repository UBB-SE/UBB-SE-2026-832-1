using System.Net.Http;
using ClassLibrary.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI.Services;
using WinUI.ViewModels;

namespace WinUI.Views.WorkoutLog;

public sealed partial class CreateWorkoutView : UserControl
{
    public CreateWorkoutViewModel ViewModel { get; }

    public event Action? WorkoutSaved;

    public CreateWorkoutView(int clientId)
    {
        ViewModel = new CreateWorkoutViewModel(new CreateWorkoutService(new HttpClient()));
        ViewModel.ClientId = clientId;
        ViewModel.WorkoutSaved += () => WorkoutSaved?.Invoke();
        InitializeComponent();
    }

    private void RemoveExercise_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is TemplateExercise exercise)
        {
            ViewModel.RemoveExerciseCommand.Execute(exercise);
        }
    }
}
