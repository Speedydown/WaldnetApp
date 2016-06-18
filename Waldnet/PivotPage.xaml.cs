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
using Windows.UI.ViewManagement;
using WaldnetLogic;
using BaseLogic.Notifications;

namespace Waldnet
{
    public sealed partial class PivotPage : Page
    {
        private static DateTime? LastLoadedDT = null;
        public static PivotPage Instance { get; private set; }

        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

        public PivotPage()
        {
            Instance = this;
            this.InitializeComponent();
            StatusBar.GetForCurrentView().ForegroundColor = Color.FromArgb(255, 7, 81, 154);
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

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
            await StatusBar.GetForCurrentView().HideAsync();
            await HandleWaldnetData();
        }

        public async Task HandleWaldnetData(bool OverrideTimer = false)
        {
            DataProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;

            if (OverrideTimer)
            {
                LastLoadedDT = DateTime.Now.AddHours(-1);
            }

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
            this.NoInternetGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.pivotControl.Visibility = Windows.UI.Xaml.Visibility.Visible;

            if (LastLoadedDT == null || DateTime.Now.Subtract((DateTime)LastLoadedDT).TotalMinutes > 5)
            {
                try
                {
                    List<NewsDay> News = (List<NewsDay>)await DataHandler.GetRegionalNews();
                    List<NewsLink> Businessnews = (List<NewsLink>)await DataHandler.GetBusinessNews();
                    List<NewsLink> SportsNews = (List<NewsLink>)await DataHandler.GetSportssNews();

                    this.RegionalNews.ItemsSource = News;
                    this.OndernemendNieuwsList.ItemsSource = new NewsDay[] { new NewsDay("Sportnieuws", SportsNews.GetRange(0, 8)), new NewsDay("Ondernemend nieuws", Businessnews.GetRange(0, 8)) };

                    if (LastLoadedDT == null)
                    {
                        NotificationHandler.Run("BackgroundTask.BackgroundTask", "WâldnetTileUpdateService", 30);
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
                    this.pivotControl.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
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

            foreach (PivotItem pi in this.pivotControl.Items)
            {
                (pi.Header as TextBlock).Foreground = new SolidColorBrush(Color.FromArgb(255,
                Convert.ToByte(hexaColor.Substring(1, 2), 16),
                Convert.ToByte(hexaColor.Substring(3, 2), 16),
                Convert.ToByte(hexaColor.Substring(5, 2), 16)));
            }


            hexaColor = "#07519A";

            ((this.pivotControl.SelectedItem as PivotItem).Header as TextBlock).Foreground = new SolidColorBrush(Color.FromArgb(255,
                Convert.ToByte(hexaColor.Substring(1, 2), 16),
                Convert.ToByte(hexaColor.Substring(3, 2), 16),
                Convert.ToByte(hexaColor.Substring(5, 2), 16)));

            if ((this.pivotControl.SelectedItem as PivotItem).Name == "SearchPivot")
            {
                SearchTextbox.Background = new SolidColorBrush(Colors.White);
                WaldNetSearchButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                WaldnetButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                RefreshButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                WaldNetSearchButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                WaldnetButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                RefreshButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
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

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(Settings)))
            {

            }
        }
    }
}
