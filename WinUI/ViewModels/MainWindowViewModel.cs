using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinUI.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private int selectedTabIndex;

    public ObservableCollection<TabItemModel> Tabs { get; }

    public int SelectedTabIndex
    {
        get => selectedTabIndex;
        set
        {
            if (selectedTabIndex != value)
            {
                selectedTabIndex = value;
                OnPropertyChanged();
            }
        }
    }

    public MainWindowViewModel()
    {
        Tabs = new ObservableCollection<TabItemModel>();
        InitializeTabs();
    }

    private void InitializeTabs()
    {
        AddTab("Home", new Views.MainView());
    }

    public void AddTab(string title, object content)
    {
        Tabs.Add(new TabItemModel
        {
            Title = title,
            Content = content
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public sealed class TabItemModel
    {
        public string Title { get; set; } = string.Empty;
        public object Content { get; set; } = new object();
    }
}
