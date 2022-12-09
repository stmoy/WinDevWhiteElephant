using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls;
using MUX = Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.ViewManagement;
using Windows.UI;

namespace WinDevWhiteElephant
{
    public sealed partial class RootPage : Page
    {
        public RootPage()
        {
            this.InitializeComponent();
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            TabViewItem MainPageTab = new TabViewItem()
            {
                IconSource = new MUX.SymbolIconSource { Symbol = Symbol.ViewAll },
                Header = "Game",
                DataContext = typeof(MainPage),
                Content = new Frame(),
                IsClosable = false
            };

            TabViewItem VideoPlayerTab = new TabViewItem()
            {
                IconSource = new MUX.SymbolIconSource { Symbol = Symbol.Video },
                Header = "Videos",
                DataContext = typeof(VideoPlayerPage),
                Content = new Frame(),
                IsClosable = false
            };

            // Pre-load the content from both pages
            (MainPageTab.Content as Frame).Navigate(MainPageTab.DataContext as System.Type);
            (VideoPlayerTab.Content as Frame).Navigate(VideoPlayerTab.DataContext as System.Type);

            RootTabView.TabItems.Add(MainPageTab);
            RootTabView.TabItems.Add(VideoPlayerTab);

            RootTabView.SelectedIndex = 0;
        }

        private void RootTabView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Extend into the title bar
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            Window.Current.SetTitleBar(CustomDragRegion);
            CustomDragRegion.MinWidth = 188;
        }
    }
}
