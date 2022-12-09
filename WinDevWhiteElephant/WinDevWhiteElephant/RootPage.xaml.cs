using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls;
using MUX = Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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

    }
}
