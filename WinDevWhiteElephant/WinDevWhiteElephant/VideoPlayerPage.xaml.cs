using System;
using System.Collections.Generic;
using System.Net.Http;
using Windows.Data.Html;
using Windows.UI.Xaml.Controls;

using static System.Net.Http.HttpClientHandler;
using HtmlAgilityPack;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WinDevWhiteElephant
{
    public sealed partial class VideoPlayerPage : Page
    {
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

            Videos.Add(new Video() { URL = "https://youtu.be/cX9HrnX602M?t=18" });
            Videos.Add(new Video() { URL = "https://youtu.be/QGBdwZNAqIQ?t=20" });
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
            }
        }

        private async void GetTitleFromURL(string url)
        {
            string html = await client.GetStringAsync(url);

            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(html);
            string title = htmlDocument.DocumentNode.SelectSingleNode("//title").InnerText;

            Title = title;
        }

        public Video()
        {

        }

        public override string ToString()
        {
            return Title;
        }
    }
}
