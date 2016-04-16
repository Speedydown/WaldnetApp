using Waldnet.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.System;
using Windows.Phone.UI.Input;
using BackgroundTask;
using System.Threading.Tasks;
using Windows.Storage;
using WaldnetLogic;
using BaseLogic.ClientIDHandler;
using BaseLogic.ArticleCounter;

// The Pivot Application template is documented at http://go.microsoft.com/fwlink/?LinkID=391641

namespace Waldnet
{
    /// <summary>
    /// A page that displays details for a single item within a group.
    /// </summary>
    public sealed partial class ItemPage : Page
    {
        RelayCommand _checkedGoBackCommand;
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        public bool EnableReactions = true;
        private bool FullsizeImage = false;

        public ItemPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            _checkedGoBackCommand = new RelayCommand(
                                    () => this.CheckGoBack(),
                                    () => this.CanCheckGoBack()
                                );

            navigationHelper.GoBackCommand = _checkedGoBackCommand;
        }

        private bool CanCheckGoBack()
        {
            return true;
        }

        private void CheckGoBack()
        {
            if (this.FullsizeImage)
            {
                this.FullsizeImage = false;
                NewsContent.Visibility = Windows.UI.Xaml.Visibility.Visible;
                FullImage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                FullImageScrollViewer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                NavigationHelper.GoBack();
            }
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
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("EnableReactions"))
            {
                this.EnableReactions = (bool)(ApplicationData.Current.LocalSettings.Values["EnableReactions"]);
            }
            else
            {
                this.EnableReactions = true;
            }

            //Hide reactions
            this.ReactionFooterGrid.Visibility = (this.EnableReactions ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed);
            this.ReactionsListview.Visibility = (this.EnableReactions ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed);
            this.ReactionsHeaderContent.Visibility = (this.EnableReactions ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed);
            //

            string URL = (string)e.NavigationParameter;

            NewsItem NI = await DataHandler.GetNewsItemFromURL(URL);

            LayoutRoot.DataContext = NI;
            
            await ArticleCounter.AddArticleCount("Wij bieden Wâldnet kostenloos aan en we zouden het op prijs stellen als u de Wâldnet app een positieve review geeft.", "Bedankt");
            DataProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            Task t = Task.Run(() => ClientIDHandler.instance.PostAppStats(ClientIDHandler.AppName.Wâldnet));
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
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

        private void ImagesListview_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.FullsizeImage = true;
            NewsContent.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            FullImage.Source = new BitmapImage(new Uri(e.ClickedItem as string));
            FullImageScrollViewer.Visibility = Windows.UI.Xaml.Visibility.Visible;
            FullImage.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void FullImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.FullsizeImage = false;
            NewsContent.Visibility = Windows.UI.Xaml.Visibility.Visible;
            FullImage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            FullImageScrollViewer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(new Uri("http://waldnet.nl"));
        }
    }
}