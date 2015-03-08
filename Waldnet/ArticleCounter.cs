using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;

namespace Waldnet
{
    public static class ArticleCounter
    {
        public static void AddArticleCount()
        {
            int Counter = GetCurrentCount() + 1;

            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;

            try
            {
                localSettings.Values["NumberOfArticles"] = Counter;
            }
            catch
            {
                
            }

            if (Counter == 25)
            {
                ShowRateDialog();
            }
        }

        private static int GetCurrentCount()
        {
            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;

            try
            {
                return (int)localSettings.Values["NumberOfArticles"];
            }
            catch
            {
                return 0;
            }
        }

        private static async Task ShowRateDialog()
        {
            var messageDialog = new Windows.UI.Popups.MessageDialog("Wij bieden Wâldnet kostenloos aan en we zouden het op prijs stellen als u de Wâldnet app een positieve review geeft.", "Bedankt");
            messageDialog.Commands.Add(
            new Windows.UI.Popups.UICommand("Review", CommandInvokedHandler));
            messageDialog.Commands.Add(
            new Windows.UI.Popups.UICommand("Annuleren", CommandInvokedHandler));
            await messageDialog.ShowAsync();
        }


        private static void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label == "Review")
            {
                Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + "85acd0f3-b88f-4fae-a90e-5f20268f1bc8"));
            }
        }

    }
}
