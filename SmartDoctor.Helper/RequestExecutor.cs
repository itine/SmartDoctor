using RestSharp;
using SmartDoctor.Data.Consts;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SmartDoctor.Helper
{
    public class RequestExecutor
    {
        public static async Task<string> ExecuteRequestAsync(MicroservicesEnum microservice,
            KeyValuePair<string, Method> requestUrl, Parameter[] requestParameters = null)
        {
            var request = new RestRequest(requestUrl.Key, requestUrl.Value)
                .AddHeader("Content-type", "application/json");
            if (requestParameters != null)
                foreach (var param in requestParameters)
                    request.AddParameter(param);
            //TODO: get url from config
            return await Execute($"http://localhost:{microservice.GetStringValue()}/", request);
        }

        public static async Task<string> ExecuteExternalRequestAsync(string url, Method method)
        {
            var request = new RestRequest(url, method);
            return await Execute(Scope.ExternalDrugApiUrl, request);
        }

        private static async Task<string> Execute(string address, IRestRequest request)
        {
            var cookieContainer = new CookieContainer();
            var client = new RestClient(address)
            {
                CookieContainer = cookieContainer
            };
            var response = await client.ExecuteTaskAsync((RestRequest)request);
            return response.Content;
        }
    }

    public static class RequestUrl
    {
        public static KeyValuePair<string, Method> PassTheTest = new KeyValuePair<string, Method>($"/{Scope.PassTheTest}", Method.POST);
        public static KeyValuePair<string, Method> GetQuestions = new KeyValuePair<string, Method>($"/{Scope.GetQuestions}", Method.GET);
        public static KeyValuePair<string, Method> GetAnswers = new KeyValuePair<string, Method>($"/{Scope.GetAnswers}", Method.GET);
        public static KeyValuePair<string, Method> GetDiseaseNameById = new KeyValuePair<string, Method>($"/{GetDiseaseNameById}", Method.POST);
        public static KeyValuePair<string, Method> EvaluateAnswer = new KeyValuePair<string, Method>($"/{EvaluateAnswer}", Method.POST);
    }

    /// <summary>
    /// StringValue — mKs port
    /// </summary>
    public enum MicroservicesEnum
    {
        [StringValue("50001")]
        Disease,
        [StringValue("50002")]
        Testing
    }

}
