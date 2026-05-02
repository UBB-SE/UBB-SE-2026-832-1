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
    }

    public void AddTab(string title, Type pageType)
    {
        Tabs.Add(new TabItemModel(pageType)
        {
            Title = title
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
