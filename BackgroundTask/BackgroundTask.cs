using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;

namespace BackgroundTask
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            await GenerateNotifications();
            deferral.Complete();
        }

        private async Task GenerateNotifications()
        {
            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;
            IList<NewsDay> News = await new DataHandler().GetRegionalNews();
            IList<NewsLink> NewsLink = new List<NewsLink>();

            string LastURL = string.Empty;

            if (localSettings.Values["LastNewsItem"] != null)
            {
                LastURL = localSettings.Values["LastNewsItem"].ToString();
            }
            else
            {
                return;
            }

            int NotificationCounter = 0;

            foreach (NewsDay n in News)
            {
                foreach (NewsLink nl in n.NewsLinks)
                {
                    if (nl.URL == LastURL)
                    {
                        if (NotificationCounter > 0)
                        {
                            CreateTile(NewsLink, NotificationCounter);
                        }

                        return;
                    }

                    NewsLink.Add(nl);
                    NotificationCounter++;
                }
            }

            
        }

        private void CreateTile(IList<NewsLink> Content, int Counter)
        {
            //LargeTile
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150BlockAndText01);
            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");

            try
            {
                tileTextAttributes[2].InnerText = Content[0].Name;
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[3].InnerText = Content[1].Name;
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[4].InnerText = Content[2].Name;
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[0].InnerText = Counter.ToString();
                //tileTextAttributes[1].InnerText = "Vandaag op wâldnet";
            }
            catch
            {

            }

            TileNotification tileNotification = new TileNotification(tileXml);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);

            //squarw
            XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Block);
            tileTextAttributes = squareTileXml.GetElementsByTagName("text");

            try
            {
            //    tileTextAttributes[1].InnerText = Content[0].Name;
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[0].InnerText = Counter.ToString();
            }
            catch
            {

            }


            IXmlNode node = tileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").Item(0), true);
            tileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

            tileNotification = new TileNotification(tileXml);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }
    }

}
