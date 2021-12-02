using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace WinDevWhiteElephant
{
    public sealed partial class MainPage : Page
    {
        ObservableCollection<CustomDataObject> Items;
        List<String> Players;

        int CurrentPlayerNumber = 0;

        object _storedItem;
        public MainPage()
        {
            this.InitializeComponent();

            LoadState();
        }

        public async void PrintState()
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile =
                await storageFolder.CreateFileAsync("WinDevWhiteElephant.txt",
                    Windows.Storage.CreationCollisionOption.ReplaceExisting);

            System.Diagnostics.Debug.WriteLine("Saved Data");

            String savedData = "";

            String newLine = "\n";

            int i = 0;
            foreach (var item in Items)
            {
                //System.Diagnostics.Debug.WriteLine("Gift " + i);
                //System.Diagnostics.Debug.WriteLine("    " + item.Title);
                //System.Diagnostics.Debug.WriteLine("    Giver: " + item.Giver);
                //System.Diagnostics.Debug.WriteLine("    Receiver: " + item.Receiver);
                //System.Diagnostics.Debug.WriteLine("    Steals: " + item.Steals);

                savedData += "Gift " + i;
                savedData += newLine;

                savedData += "    " + item.Title;
                savedData += newLine;

                savedData += "    Giver: " + item.Giver;
                savedData += newLine;

                savedData += "    Receiver: " + item.Receiver;
                savedData += newLine;

                savedData += "    Steals: " + item.Steals;
                savedData += newLine;


                i++;
            }

            System.Diagnostics.Debug.WriteLine(savedData);

            await Windows.Storage.FileIO.WriteTextAsync(sampleFile, savedData);
        }

        public void LoadState()
        {
            // load a setting that is local to the device
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Windows.Storage.ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)localSettings.Values["Backup"];
            if (composite != null)
            {

                ObservableCollection<CustomDataObject> LoadedItems = new ObservableCollection<CustomDataObject>();

                try
                {
                    foreach (var item in composite)
                    {
                        System.Diagnostics.Debug.WriteLine(item);

                        var splitItem = item.Value.ToString().Split(",");

                        var customObject = new CustomDataObject()
                        {
                            ImageLocation = item.Key,
                            Title = splitItem[0],
                            Receiver = splitItem[1],
                            Steals = Int32.Parse(splitItem[2]),
                            Giver = splitItem[3]
                        };

                        LoadedItems.Add(customObject);

                    }

                    if (composite.Count == 0)
                    {
                        LoadedItems = null;
                    }
                }

                catch (Exception e)
                {
                    LoadedItems = null;
                }

                Items = LoadedItems;
            }
        }

        public void SaveState()
        {
            // Save a setting locally on the device
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Windows.Storage.ApplicationDataCompositeValue composite = new Windows.Storage.ApplicationDataCompositeValue();

            foreach (var item in Items)
            {
                composite[item.ImageLocation] = item.Title + "," + item.Receiver + "," + item.Steals + "," + item.Giver;
            }

            localSettings.Values["Backup"] = composite;

        }

        public void ClearSaveState()
        {
            // Save a setting locally on the device
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Windows.Storage.ApplicationDataCompositeValue composite = new Windows.Storage.ApplicationDataCompositeValue();

            localSettings.Values["Backup"] = composite;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (Items == null)
            {
                List<CustomDataObject> tempList = CustomDataObject.GetDataObjects();
                Items = new ObservableCollection<CustomDataObject>(tempList);

            }
            BasicGridView.ItemsSource = Items;


            Players = new List<String>();
            foreach(var item in Items)
            {
                Players.Add(item.Giver);
            }

            var tempList2 = Players.OrderBy(a => Guid.NewGuid()).ToList();

            Players = tempList2;
        }

        private void BasicGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.NewSize);
        }

        private void BasicGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveState();

            var gift = (e.ClickedItem as CustomDataObject);


            ConnectedAnimation animation = null;

            // Get the collection item corresponding to the clicked item.
            if (BasicGridView.ContainerFromItem(e.ClickedItem) is GridViewItem container)
            {
                // Stash the clicked item for use later. We'll need it when we connect back from the detailpage.
                _storedItem = container.Content;

                // Prepare the connected animation.
                // Notice that the stored item is passed in, as well as the name of the connected element. 
                // The animation will actually start on the Detailed info page.
                animation = BasicGridView.PrepareConnectedAnimation("forwardAnimation", _storedItem, "connectedElement");

            }



            SmokeGrid.DataContext = gift;

            SmokeGrid.Visibility = Visibility.Visible;

            animation.TryStart(destinationElement);

        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SaveState();

            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("backwardsAnimation", destinationElement);

            // Collapse the smoke when the animation completes.
            animation.Completed += Animation_Completed;

            // If the connected item appears outside the viewport, scroll it into view.
            BasicGridView.ScrollIntoView(_storedItem, ScrollIntoViewAlignment.Default);
            BasicGridView.UpdateLayout();

            // Use the Direct configuration to go back (if the API is available). 
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                animation.Configuration = new DirectConnectedAnimationConfiguration();
            }

            // Play the second connected animation. 
            await BasicGridView.TryStartConnectedAnimationAsync(animation, _storedItem, "connectedElement");

            Animation_Completed(null, null);

        }

        private void Animation_Completed(ConnectedAnimation sender, object args)
        {
            SmokeGrid.Visibility = Visibility.Collapsed;
        }

        private void StealButton_Click(object sender, RoutedEventArgs e)
        {
            var item = ((sender as Button).DataContext) as CustomDataObject;

            item.Steals++;

            //if (item.Steals >= 3)
            //{
            //    (sender as Button).IsEnabled = false;
            //}
        }

        private void ReceiverTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var item = ((sender as TextBox).DataContext) as CustomDataObject;

            item.Receiver = (sender as TextBox).Text;
        }

        private void GiftNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var item = ((sender as TextBox).DataContext) as CustomDataObject;

            item.Title = (sender as TextBox).Text;
        }

        private void ClearSaveDataButton_Click(object sender, RoutedEventArgs e)
        {
            ClearSaveState();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintState();
        }

        private void NextPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentPlayerTextBlock.Text = Players[CurrentPlayerNumber];
            CurrentPlayerCountTextBlock.Text = (CurrentPlayerNumber + 1).ToString();
            
            CurrentPlayerNumber++;

            if (CurrentPlayerNumber == Players.Count)
            {
                (sender as Button).IsEnabled = false;
                return;
            }
        }
    }

    public class CustomDataObject : INotifyPropertyChanged
    {
        private string _giftTitle;
        public string Title
        {
            get
            {
                return _giftTitle;
            }
            set
            {
                _giftTitle = value;
                OnPropertyChanged("Title");
            }
        }
        public string ImageLocation { get; set; }

        private int _steals = 0;
        public int Steals
        {
            get
            {
                return _steals;
            }
            set
            {
                _steals = value;
                OnPropertyChanged("Steals");
            }
        }
        public string Giver { get; set; }

        private string _receiver;
        public string Receiver
        {
            get
            {
                return _receiver;
            }
            set
            {
                _receiver = value;
                OnPropertyChanged("Receiver");
            }
        }

        public CustomDataObject()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string status)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(status));


        }

        public static List<CustomDataObject> GetDataObjects()
        {
            List<CustomDataObject> objects = new List<CustomDataObject>();

            var files = Directory.GetFiles("./Assets/Gifts/");

            foreach (var fileName in files)
            {
                objects.Add(new CustomDataObject()
                {
                    ImageLocation = fileName,
                    Steals = 0,
                    Giver = GetGiverNameFromFile(fileName),
                });
            }

            return objects;
        }

        public static string GetGiverNameFromFile(string fileName)
        {
            // File string format : "<photo name>_<giver first name> <giver last name>.<filetype>"
            var fileStrings = fileName.Split('_');

            // Get the <giver first name> <giver last name>.<filetype> and split on the period
            var giverNameAndFileType = fileStrings[fileStrings.Count() - 1].Split('.');

            // Return the string "<giver first name> <giver last name>"
            return giverNameAndFileType[0];
        }
    }


}
