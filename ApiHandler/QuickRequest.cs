using System;
using System.Diagnostics;
using System.Threading.Tasks;

using RestSharp;

using DefiKindom_QuestRunner.Properties;

namespace DefiKindom_QuestRunner.ApiHandler
{
    internal class QuickRequest
    {
        public async Task<T> GetDfkApiResponse<T>(string route, object objectToPost = null)
        {
            try
            {
                using (var client = new RestClient(Settings.Default.ExecutorApi))
                {
                    var request = new RestRequest(route, Method.Post)
                    {
                        Timeout = 30000,
                        RequestFormat = DataFormat.Json
                    };
                    request.AddHeader("Content-type", "application/json");

                    if(objectToPost != null)
                        request.AddJsonBody(objectToPost);

                    var response = await client.ExecuteAsync<T>(request);
                    return response.Data;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }

            return default(T);
        }
    }
}
