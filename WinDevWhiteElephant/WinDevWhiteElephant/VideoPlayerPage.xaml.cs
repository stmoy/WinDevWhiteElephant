using System;
using System.Collections.Generic;
using System.Net.Http;
using Windows.Data.Html;
using Windows.UI.Xaml.Controls;

using static System.Net.Http.HttpClientHandler;
using HtmlAgilityPack;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.ApplicationModel.Activation;

namespace WinDevWhiteElephant
{
    public sealed partial class VideoPlayerPage : Page
    {
        string[] urls =
        {
            "https://youtu.be/cX9HrnX602M?t=18",
            "https://youtu.be/QGBdwZNAqIQ?t=20",
            "https://youtube.com/shorts/k-cavEKkirc",
            "https://www.youtube.com/watch?v=G9PyW259YWY",
            "https://www.youtube.com/watch?v=q4a9CKgLprQ",
            "https://www.youtube.com/watch?v=SAXH6oSu1jY"

        };

        ObservableCollection<Video> Videos;

        public VideoPlayerPage()
        {
            this.InitializeComponent();

            PopulateVideos();

            VideosList.ItemsSource = Videos;
        }

        public void PopulateVideos()
        {
            Videos = new ObservableCollection<Video>();

            foreach (string url in urls)
            {
                Videos.Add(new Video()  { URL = url });
            }
        }

        private void VideosList_ItemClick(object sender, ItemClickEventArgs e)
        {
            VideoPlayer.Source = new Uri((e.ClickedItem as Video).URL);

        }
    }

    public sealed class Video : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string status)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(status));
        }

        private HttpClient client = new HttpClient();

        private string _title;
        public String Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        private String _url;

        public String URL
        {
            get { return _url; }
            set
            {
                _url = value;
                GetTitleFromURL(value);
                //Title = URL;
            }
        }

        private async void GetTitleFromURL(string url)
        {
            // TODO: This takes a LONG time to do. Can we do this after the app opens or off-thread?

            string html = await client.GetStringAsync(url);

            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(html);
            string title = htmlDocument.DocumentNode.SelectSingleNode("//title").InnerText;

            Title = title;
        }

        public Video() { }

        public override string ToString()
        {
            return Title;
        }
    }
}
