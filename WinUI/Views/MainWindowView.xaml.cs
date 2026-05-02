using System.Collections.Specialized;
using Microsoft.UI.Xaml.Controls;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class MainWindowView : Page
{
    public MainWindowViewModel ViewModel { get; }

    public MainWindowView()
    {
        ViewModel = new MainWindowViewModel();
        InitializeComponent();
        ViewModel.AddTab("Home", typeof(MainView));
        ViewModel.Tabs.CollectionChanged += OnTabsCollectionChanged;
        PopulateTabs();
        mainTabView.SelectionChanged += OnTabSelectionChanged;
        Unloaded += OnViewUnloaded;
    }

    private void PopulateTabs()
    {
        foreach (TabItemModel tab in ViewModel.Tabs)
        {
            mainTabView.TabItems.Add(CreateTabViewItem(tab));
        }
    }

    private void OnTabsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.Action == NotifyCollectionChangedAction.Add && args.NewItems != null)
        {
            foreach (TabItemModel tab in args.NewItems)
            {
                mainTabView.TabItems.Add(CreateTabViewItem(tab));
            }
        }
    }

    private void OnTabSelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        ViewModel.SelectedTabIndex = mainTabView.SelectedIndex;
    }

    private void OnViewUnloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs args)
    {
        ViewModel.Tabs.CollectionChanged -= OnTabsCollectionChanged;
        mainTabView.SelectionChanged -= OnTabSelectionChanged;
    }

    private static TabViewItem CreateTabViewItem(TabItemModel tab)
    {
        object? content = Activator.CreateInstance(tab.PageType);
        return new TabViewItem
        {
            Header = tab.Title,
            IsClosable = false,
            Content = content ?? new TextBlock { Text = $"Could not load '{tab.Title}' ({tab.PageType.FullName})" }
        };
    }
}
