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
    public static class DataHandler
    {
        private static Semaphore DataSemaphore = new Semaphore(1, 1);

        public static IAsyncOperation<IList<NewsDay>> GetRegionalNews()
        {
            return GetRegionalNewsHelper().AsAsyncOperation();
        }

        private static async Task<IList<NewsDay>> GetRegionalNewsHelper()
        {
            IList<NewsDay> NewsDays = new List<NewsDay>();

            try
            {
                NewsDays = PageParser.ParseRegionalNews(await GetDataFromURL(""));
            }
            catch (Exception)
            {

            }

            return NewsDays;

        }

        public static IAsyncOperation<IList<NewsLink>> GetBusinessNews()
        {
            return GetBusinessNewsHelper().AsAsyncOperation();
        }

        private static async Task<IList<NewsLink>> GetBusinessNewsHelper()
        {
            List<NewsLink> NewsLinks = new List<NewsLink>();

            try
            {
                NewsLinks = PageParser.ParseBusinessNews(await GetDataFromURL("http://waldnet.nl/ondernemendnieuws.php"));
            }
            catch (Exception)
            {

            }

            return NewsLinks;
        }

        public static IAsyncOperation<IList<NewsLink>> GetSportssNews()
        {
            return GetSportssNewsHelper().AsAsyncOperation();
        }

        private static async Task<IList<NewsLink>> GetSportssNewsHelper()
        {
            List<NewsLink> NewsLinks = new List<NewsLink>();

            try
            {
                NewsLinks = PageParser.ParseBusinessNews(await GetDataFromURL("http://waldnet.nl/sportnieuws.php"));
            }
            catch (Exception)
            {

            }

            return NewsLinks;
        }

        public static IAsyncOperation<IList<SearchResult>> GetSearchResult(string Query)
        {
            return GetSearchResultHelper(Query).AsAsyncOperation();
        }

        private static async Task<IList<SearchResult>> GetSearchResultHelper(string Query)
        {
            IList<SearchResult> NewsLinks = new List<SearchResult>();

            try
            {
                NewsLinks = SearchResultParser.GetNewsLinksFromSearchResult(await Search(Query));
            }
            catch (Exception)
            {

            }

            return NewsLinks;
        }

        public static IAsyncOperation<NewsItem> GetNewsItemFromURL(string URL)
        {
            return GetNewsItemFromURLHelper(URL).AsAsyncOperation();
        }

        private static async Task<NewsItem> GetNewsItemFromURLHelper(string URL)
        {
            return await NewsItemParser.ParseNews(await GetDataFromURL(URL));
        }

        public static IAsyncOperation<string> GetDataFromURL(string URL)
        {
            return GetDataFromURLHelper(URL).AsAsyncOperation();
        }

        private static async Task<string> GetDataFromURLHelper(string URL)
        {
            if (URL == string.Empty)
            {
                URL = "http://waldnet.nl/regionaal.php";
            }

            string Output = string.Empty;

            if (DataSemaphore.WaitOne(10000))
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
                DataSemaphore.Release();
            }
            catch
            {

            }

            return Output;

        }

        public static IAsyncOperation<string> Search(string SearchTerm)
        {
            return SearchHelper(SearchTerm).AsAsyncOperation();
        }

        private static async Task<string> SearchHelper(string SearchTerm)
        {
            string Output = string.Empty;

            if (DataSemaphore.WaitOne(10000))
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
                DataSemaphore.Release();
            }
            catch
            {

            }

            return Output;
        }

    }
}
