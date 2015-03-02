using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Waldnet.Data.DataModel;
using System.Net;

namespace Waldnet.Data
{
    public class DataHandler
    {
        public static readonly DataHandler Instance = new DataHandler();

        private Semaphore DataSemaphore = new Semaphore(1,1);

        private DataHandler()
        {
        }

        public async Task<List<NewsDay>> GetRegionalNews()
        {
            List<NewsDay> NewsDays = new List<NewsDay>();

            try
            {
                NewsDays = PageParser.ParseRegionalNews(await this.GetDataFromURL());
            }
            catch(Exception)
            {

            }
           
            return NewsDays;

        }

        public async Task<NewsItem> GetNewsItemFromURL(string URL)
        {
            return await NewsItemParser.ParseNews(await this.GetDataFromURL(URL));
        }

        public async Task<string> GetDataFromURL(string URL = "http://waldnet.nl/regionaal.php")
        {
            string Output = string.Empty;

            if (this.DataSemaphore.WaitOne(10000))
            {

                try
                {
                    var client = new HttpClient();       
                    
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
             
    }
}
