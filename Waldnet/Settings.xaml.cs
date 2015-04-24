using Waldnet.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;

namespace Waldnet
{
    public sealed partial class Settings : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public bool EnableReactions = true;
        public bool EnableImages = true;
        
        public Settings()
        {
            this.InitializeComponent();

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

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("EnableReactions"))
            {
                this.EnableReactions = (bool)(ApplicationData.Current.LocalSettings.Values["EnableReactions"]);
            }
            else
            {
                this.EnableReactions = true;
            }

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("EnableImages"))
            {
                this.EnableImages = (bool)(ApplicationData.Current.LocalSettings.Values["EnableImages"]);
            }
            else
            {
                this.EnableImages = true;
            }

            this.ToggleSwitch.DataContext = this.EnableReactions;
            this.ImageSwitch.DataContext = this.EnableImages;
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
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

        private void EnableReactions_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            this.EnableReactions = ToggleSwitch.IsOn;

            ApplicationData.Current.LocalSettings.Values["EnableReactions"] = this.EnableReactions;
        }

        private void ImageSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            this.EnableImages = ImageSwitch.IsOn;

            ApplicationData.Current.LocalSettings.Values["EnableImages"] = this.EnableImages;
        }
    }
}
