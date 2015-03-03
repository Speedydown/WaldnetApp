using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Notifications;

namespace Tasks
{
    public sealed class LiveTileTask : IBackgroundTask
    {
        BackgroundTaskCancellationReason _cancelReason = BackgroundTaskCancellationReason.Abort;
        volatile bool _cancelRequested = false;
        BackgroundTaskDeferral _deferral = null;
        ThreadPoolTimer _periodicTimer = null;
        uint _progress = 0;
        IBackgroundTaskInstance _taskInstance = null; 


        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();
            _taskInstance = taskInstance; 

            _periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback), TimeSpan.FromSeconds(1)); 
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _cancelRequested = true;
            _cancelReason = reason;
        }

        private void PeriodicTimerCallback(ThreadPoolTimer timer)
        {
            if ((_cancelRequested == false) && (_progress < 100))
            {
                _progress += 10;
                _taskInstance.Progress = _progress;
            }
            else
            {
                _periodicTimer.Cancel();




                XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150ImageAndText01);
                XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
                tileTextAttributes[0].InnerText = "Hello World! My very own tile notification";

                XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");
                ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///assets/WideLogo.scale-240.png");
                ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "red graphic");
                _deferral.Complete();

                TileNotification tileNotification = new TileNotification(tileXml);

                TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
            }
        }

    }
}
