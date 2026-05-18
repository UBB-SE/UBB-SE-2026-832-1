using System.ComponentModel;
using System.Net.Http;
using ClassLibrary.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ClassLibrary.Proxies;
using WinUI.ViewModels;

namespace WinUI.Views.WorkoutLog;

public sealed partial class CreateWorkoutView : UserControl
{
    public CreateWorkoutViewModel ViewModel { get; }

    public event Action? WorkoutSaved;

    public CreateWorkoutView(int clientId)
    {
        ViewModel = new CreateWorkoutViewModel(new CreateWorkoutProxy(new HttpClient()));
        ViewModel.ClientId = clientId;
        ViewModel.WorkoutSaved += () => WorkoutSaved?.Invoke();
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        InitializeComponent();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CreateWorkoutViewModel.SelectedNewExercise) && ViewModel.SelectedNewExercise is null)
        {
            ExerciseComboBox.SelectedIndex = -1;
        }
    }

    private void RemoveExercise_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is TemplateExercise exercise)
        {
            ViewModel.RemoveExerciseCommand.Execute(exercise);
        }
    }

    private void ExerciseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is string exerciseName)
        {
            ViewModel.SelectedNewExercise = exerciseName;
        }
        else
        {
            ViewModel.SelectedNewExercise = null;
        }
    }
}

