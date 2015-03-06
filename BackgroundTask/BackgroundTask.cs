﻿using System;
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
            try
            {
                ApplicationData applicationData = ApplicationData.Current;
                ApplicationDataContainer localSettings = applicationData.LocalSettings;
                IList<NewsDay> News = await DataHandler.GetRegionalNews();
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
            catch(Exception)
            {

            }            
        }

        private void CreateTile(IList<NewsLink> Content, int Counter)
        {
            //LargeTile
            XmlDocument RectangleTile = CreateRectangleTile(Content, Counter);
            XmlDocument SquareTile = CreateSquareTile();
            XmlDocument SmallTile = CreateSmallTile();


            //Badges
            XmlDocument badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeGlyph);
            XmlElement badgeElement = (XmlElement)badgeXml.SelectSingleNode("/badge");
            badgeElement.SetAttribute("value", Counter.ToString());

            BadgeNotification badge = new BadgeNotification(badgeXml);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);


            //Add tiles together
            IXmlNode node = RectangleTile.ImportNode(SquareTile.GetElementsByTagName("binding").Item(0), true);
            RectangleTile.GetElementsByTagName("visual").Item(0).AppendChild(node);

            node = RectangleTile.ImportNode(SmallTile.GetElementsByTagName("binding").Item(0), true);
            RectangleTile.GetElementsByTagName("visual").Item(0).AppendChild(node);

            TileNotification tileNotification = new TileNotification(RectangleTile);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }

        private XmlDocument CreateRectangleTile(IList<NewsLink> Content, int Counter)
        {
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150IconWithBadgeAndText);
            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");

            try
            {
                tileTextAttributes[0].InnerText = "Laatste nieuws:";
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[1].InnerText = Content[0].Name;
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[2].InnerText = Content[1].Name;
            }
            catch
            {

            }

            XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");

            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///assets/PompeBledTransparnt.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "Wâldnet.nl");

            return tileXml;
        }

        private XmlDocument CreateSquareTile()
        {
            XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare71x71IconWithBadge);
            XmlNodeList tileImageAttributes = squareTileXml.GetElementsByTagName("image");

            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///assets/PompeBledTransparnt.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "Wâldnet.nl");

            return squareTileXml;
        }

        private XmlDocument CreateSmallTile()
        {
            XmlDocument SmallTIle = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150IconWithBadge);
            XmlNodeList tileImageAttributes = SmallTIle.GetElementsByTagName("image");

            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///assets/PompeBledTransparnt.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "Wâldnet.nl");

            return SmallTIle;
        }
    }

}
