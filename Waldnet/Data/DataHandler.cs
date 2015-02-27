using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Waldnet.Data.DataModel;

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

            if (this.DataSemaphore.WaitOne(10000))
            {

                try
                {
                    var client = new HttpClient();
                    var response = await client.GetAsync(new Uri("http://waldnet.nl/regionaal.php"));
                    string Result = await response.Content.ReadAsStringAsync();

                    if (Result != null && Result.Length > 0)
                    {
                        NewsDays = PageParser.ParseRegionalNews(Result);
                    }
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

            this.DataSemaphore.Release();
            return NewsDays;

        }
             
    }
}
