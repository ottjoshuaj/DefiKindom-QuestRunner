using System;
using System.Threading.Tasks;

using RestSharp;

using DefiKindom_QuestRunner.Properties;

namespace DefiKindom_QuestRunner.ApiHandler
{
    internal class QuickRequest
    {
        public enum ApiRequestTypes
        {
            TestEndpoint,
            ProfileCreate,
            MoveJewel,
            WalletCreate,
            HeroTransfer,
            StartQuest,
            StopQuest
        }

        public async Task<T> GetDfkApiResponse<T>(ApiRequestTypes type, object objectToPost = null)
        {
            try
            {
                var apiRoute = "";

                switch (type)
                {
                    case ApiRequestTypes.WalletCreate:
                        apiRoute = "/api/wallets/create";
                        break;

                    case ApiRequestTypes.HeroTransfer:
                        apiRoute = "/api/hero/transfer";
                        break;

                    case ApiRequestTypes.StartQuest:
                        apiRoute = "/api/quest/start";
                        break;

                    case ApiRequestTypes.StopQuest:
                        apiRoute = "/api/quest/stop";
                        break;

                    case ApiRequestTypes.TestEndpoint:
                        apiRoute = "/api/onboard";
                        break;

                    case ApiRequestTypes.ProfileCreate:
                        apiRoute = "/api/profile/create";
                        break;

                    case ApiRequestTypes.MoveJewel:
                        apiRoute = "/api/jewel/transfer";
                        break;
                }

                using (var client = new RestClient(Settings.Default.ExecutorApi))
                {
                    var request = new RestRequest(apiRoute, Method.Post)
                    {
                        Timeout = 300000,
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

            }

            return default(T);
        }
    }
}
