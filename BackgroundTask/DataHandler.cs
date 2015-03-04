using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Windows.Foundation;

namespace BackgroundTask
{
    public sealed class DataHandler
    {
        private Semaphore DataSemaphore = new Semaphore(1, 1);

        public DataHandler()
        {
        }

        public IAsyncOperation<IList<NewsDay>> GetRegionalNews()
        {
            return GetRegionalNewsHelper().AsAsyncOperation();
        }

        private async Task<IList<NewsDay>> GetRegionalNewsHelper()
        {
            List<NewsDay> NewsDays = new List<NewsDay>();

            try
            {
                NewsDays = PageParser.ParseRegionalNews(await this.GetDataFromURL(""));
            }
            catch (Exception)
            {

            }

            return NewsDays;

        }

        public IAsyncOperation<IList<NewsLink>> GetBusinessNews()
        {
            return GetBusinessNewsHelper().AsAsyncOperation();
        }

        private async Task<IList<NewsLink>> GetBusinessNewsHelper()
        {
            List<NewsLink> NewsLinks = new List<NewsLink>();

            try
            {
                NewsLinks = PageParser.ParseBusinessNews(await this.GetDataFromURL("http://waldnet.nl/ondernemendnieuws.php"));
            }
            catch (Exception)
            {

            }

            return NewsLinks;
        }

        public IAsyncOperation<IList<NewsLink>> GetSportssNews()
        {
            return GetSportssNewsHelper().AsAsyncOperation();
        }


        private async Task<IList<NewsLink>> GetSportssNewsHelper()
        {
            List<NewsLink> NewsLinks = new List<NewsLink>();

            try
            {
                NewsLinks = PageParser.ParseBusinessNews(await this.GetDataFromURL("http://waldnet.nl/sportnieuws.php"));
            }
            catch (Exception)
            {

            }

            return NewsLinks;
        }

        public IAsyncOperation<string> GetDataFromURL(string URL)
        {
            if (URL == string.Empty)
            {
                URL = "http://waldnet.nl/regionaal.php";
            }

            return GetDataFromURLHelper(URL).AsAsyncOperation();
        }

        private async Task<string> GetDataFromURLHelper(string URL = "http://waldnet.nl/regionaal.php")
        {
            string Output = string.Empty;

            if (this.DataSemaphore.WaitOne(10000))
            {

                try
                {
                    var client = new System.Net.Http.HttpClient();

                    var response = await client.GetAsync(new Uri(URL));

                    var ByteArray = await response.Content.ReadAsByteArrayAsync();
                    Output = Encoding.GetEncoding("iso-8859-1").GetString(ByteArray, 0, ByteArray.Length);
                }
                catch (HttpRequestException)
                {
                    //ErrorDialog.ShowError("Geen verbinding", "Geen verbinding naar de server. Controleer uw internet verbinding.");
                }
                catch (TaskCanceledException)
                {
                    // ErrorDialog.ShowError("Geen verbinding", "Geen verbinding naar de server. Controleer uw internet verbinding.");
                }
                //catch (Exception e)
                //{
                //   // ErrorDialog.ShowError("OOOps..", "Er gaat iets mis.");

                //   // if (SendExceptions)
                //    {
                //    //    new AppException(0, e);
                //    }
                //}


            }

            try
            {
                this.DataSemaphore.Release();
            }
            catch
            {

            }

            return Output;

        }

        private async Task<string> Search(string SearchTerm)
        {
            string Output = string.Empty;

            if (this.DataSemaphore.WaitOne(10000))
            {
                try
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("q", SearchTerm) 
                    });

                    var myHttpClient = new HttpClient();
                    var response = await myHttpClient.PostAsync("http://waldnet.nl/zoeken.php", formContent);

                    var ByteArray = await response.Content.ReadAsByteArrayAsync();
                    Output = Encoding.GetEncoding("iso-8859-1").GetString(ByteArray, 0, ByteArray.Length);
                }
                catch (HttpRequestException)
                {
                    //ErrorDialog.ShowError("Geen verbinding", "Geen verbinding naar de server. Controleer uw internet verbinding.");
                }
                catch (TaskCanceledException)
                {
                    // ErrorDialog.ShowError("Geen verbinding", "Geen verbinding naar de server. Controleer uw internet verbinding.");
                }
                //catch (Exception e)
                //{
                //   // ErrorDialog.ShowError("OOOps..", "Er gaat iets mis.");

                //   // if (SendExceptions)
                //    {
                //    //    new AppException(0, e);
                //    }
                //}


            }

            try
            {
                this.DataSemaphore.Release();
            }
            catch
            {

            }

            return Output;
        }

    }
}
