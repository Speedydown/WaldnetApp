using Waldnet.Common;
using Waldnet.Data;
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
using Waldnet.Data.DataModel;
using Windows.UI;
using Windows.System;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using System.Threading.Tasks;

namespace Waldnet
{
    public sealed partial class PivotPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

        public PivotPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            //const string name = "TileTask";

            //if (BackgroundTaskRegistration.AllTasks.Any(task => task.Value.Name == name))
            //{
            //    // One register it once
            //    return;
            //}

            //var builder = new BackgroundTaskBuilder();
            //var trigger = new SystemTrigger(SystemTriggerType., false);

            //builder.Name = name;
            //builder.TaskEntryPoint = typeof(Tasks.LiveTileTask).FullName;
            //builder.SetTrigger(trigger);

            //var registration = builder.Register();
            //registration.Completed += RegistrationOnCompleted;



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

            List<NewsDay> News = await DataHandler.Instance.GetRegionalNews();
            List<NewsLink> Businessnews = await DataHandler.Instance.GetBusinessNews();
            List<NewsLink> SportsNews = await DataHandler.Instance.GetSportssNews();

            foreach (NewsLink n in SportsNews)
            {
                n.SetImage("Assets/Sport.png");
            }

            foreach (NewsLink n in Businessnews)
            {
                n.SetImage("Assets/business.png");
            }

            this.RegionalNews.ItemsSource = News;
            this.OndernemendNieuwsList.ItemsSource = new NewsDay[] { new NewsDay("Sport nieuws", SportsNews.GetRange(0, 8)), new NewsDay("Ondernemend nieuws", Businessnews.GetRange(0, 8)) };
            DataProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

         //   Task Tile = Task.Run(() => CreateTile(News));
        }

        private async Task CreateTile(List<NewsDay> News)
        {

            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150BlockAndText01);
            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");

            try
            {
                tileTextAttributes[4].InnerText = News[0].NewsLinks[0].Name;
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[3].InnerText = News[0].NewsLinks[1].Name;
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[2].InnerText = News[0].NewsLinks[2].Name;
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[0].InnerText = News[0].NewsLinks.Count.ToString();
                tileTextAttributes[1].InnerText = "Vandaag op wâldnet";
            }
            catch
            {

            }

            TileNotification tileNotification = new TileNotification(tileXml);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache. Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/>.</param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
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
            SearchResultList.ItemsSource = await DataHandler.Instance.GetSearchResult(SearchTextbox.Text);
            DataProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
    }
}
