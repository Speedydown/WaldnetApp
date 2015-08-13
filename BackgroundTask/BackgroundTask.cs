using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using WRCHelperLibrary;

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
            try
            {
                ApplicationData applicationData = ApplicationData.Current;
                ApplicationDataContainer localSettings = applicationData.LocalSettings;
                IList<NewsDay> News = await DataHandler.GetRegionalNews();
                IList<NewsLink> NewsLink = new List<NewsLink>();

                string LastURL = string.Empty;

                //Debug
                //localSettings.Values["LastNewsItem"] = News[0].NewsLinks[1].URL;

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
                                CreateTiles(NewsLink.Cast<INewsLink>().ToList(), NotificationCounter);
                                BadgeHandler.CreateBadge(NotificationCounter);
                            }

                            return;
                        }

                        NewsLink.Add(nl);
                        NotificationCounter++;
                    }
                }
            }
            catch(Exception)
            {

            }            
        }

        private void CreateTiles(IList<INewsLink> Content, int Counter)
        {
            XmlDocument RectangleTile = TileXmlHandler.CreateRectangleTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150IconWithBadgeAndText), Content, Counter, "ms-appx:///assets/PompeBledTransparnt.png", "Wâldnet.nl");
            XmlDocument SquareTile = TileXmlHandler.CreateSquareTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150IconWithBadge), Content, "ms-appx:///assets/PompeBledTransparnt.png", "Wâldnet.nl");
            XmlDocument SmallTile = TileXmlHandler.CreateSmallSquareTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare71x71IconWithBadge), "ms-appx:///assets/PompeBledTransparnt.png", "Wâldnet.nl");

            TileXmlHandler.CreateTileUpdate(new XmlDocument[] { RectangleTile, SquareTile, SmallTile });
        }
    }

}
