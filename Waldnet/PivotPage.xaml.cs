using Waldnet.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.System;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using System.Threading.Tasks;
using Windows.Storage;
using BackgroundTask;

namespace Waldnet
{
    public sealed partial class PivotPage : Page
    {
        private static DateTime? LastLoadedDT = null;

        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

        public PivotPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            await HandleWaldnetData();
        }

        private async Task HandleWaldnetData()
        {
            DataProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            this.NoInternetGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            if (LastLoadedDT == null || DateTime.Now.Subtract((DateTime)LastLoadedDT).Minutes > 5)
            {
                try
                {
                    List<NewsDay> News = (List<NewsDay>)await DataHandler.GetRegionalNews();
                    List<NewsLink> Businessnews = (List<NewsLink>)await DataHandler.GetBusinessNews();
                    List<NewsLink> SportsNews = (List<NewsLink>)await DataHandler.GetSportssNews();

                    foreach (NewsLink n in SportsNews)
                    {
                        n.SetImage("Assets/Sport.png");
                    }

                    foreach (NewsLink n in Businessnews)
                    {
                        n.SetImage("Assets/business.png");
                    }

                    this.RegionalNews.ItemsSource = News;
                    this.OndernemendNieuwsList.ItemsSource = new NewsDay[] { new NewsDay("Sportnieuws", SportsNews.GetRange(0, 8)), new NewsDay("Ondernemend nieuws", Businessnews.GetRange(0, 8)) };

                    if (LastLoadedDT == null)
                    {
                        NotificationHandler.Run();
                    }

                    ApplicationData applicationData = ApplicationData.Current;
                    ApplicationDataContainer localSettings = applicationData.LocalSettings;

                    try
                    {
                        localSettings.Values["LastNewsItem"] = News.First().NewsLinks.First().URL;
                    }
                    catch
                    {

                    }

                    LastLoadedDT = DateTime.Now;
                }
                catch (Exception)
                {
                    this.NoInternetGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }

            DataProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!Frame.Navigate(typeof(ItemPage), (e.ClickedItem as NewsLink).URL))
            {

            }
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string hexaColor = "#DCE7F3";

            foreach (PivotItem pi in pivot.Items)
            {
                (pi.Header as TextBlock).Foreground = new SolidColorBrush(Color.FromArgb(255,
                Convert.ToByte(hexaColor.Substring(1, 2), 16),
                Convert.ToByte(hexaColor.Substring(3, 2), 16),
                Convert.ToByte(hexaColor.Substring(5, 2), 16)));
            }


            hexaColor = "#07519A";

            ((pivot.SelectedItem as PivotItem).Header as TextBlock).Foreground = new SolidColorBrush(Color.FromArgb(255,
                Convert.ToByte(hexaColor.Substring(1, 2), 16),
                Convert.ToByte(hexaColor.Substring(3, 2), 16),
                Convert.ToByte(hexaColor.Substring(5, 2), 16)));

            if ((pivot.SelectedItem as PivotItem).Name == "SearchPivot")
            {
                SearchTextbox.Background = new SolidColorBrush(Colors.White);
                WaldNetSearchButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                WaldnetButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                WaldNetSearchButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                WaldnetButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        private async void PrivacyPolicyButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://waldnet.nl/wn/p/1/Copyright.html"));
        }

        private void SearchResultList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!Frame.Navigate(typeof(ItemPage), (e.ClickedItem as SearchResult).URL))
            {

            }
        }

        private void SearchTextbox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                this.Search();

                var control = sender as Control;
                var isTabStop = control.IsTabStop;
                control.IsTabStop = false;
                control.IsEnabled = false;
                control.IsEnabled = true;
                control.IsTabStop = isTabStop;
            }
        }

        private async void WaldnetButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://waldnet.nl/"));

        }

        private void WaldNetSearchButton_Click(object sender, RoutedEventArgs e)
        {
            this.Search();
        }

        private async void Search()
        {
            DataProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            SearchResultList.ItemsSource = await DataHandler.GetSearchResult(SearchTextbox.Text);
            DataProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.RefreshButton.IsEnabled = false;
            this.RegionalNews.ItemsSource = null;
            this.OndernemendNieuwsList.ItemsSource = null;
            SearchResultList.ItemsSource = null;
            LastLoadedDT = null;
            await this.HandleWaldnetData();
            this.RefreshButton.IsEnabled = true;
        }
    }
}
