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
            KeyValuePair<string, Method> requestUrl, Parameter[] requestParameters)
        {
            var request = new RestRequest(requestUrl.Key, requestUrl.Value)
                .AddHeader("Content-type", "application/json");
            foreach (var param in requestParameters)
                request.AddParameter(param);
            //TODO: get url from config
            return await Execute($"http://localhost:{microservice.GetStringValue()}/", request);
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
        public static KeyValuePair<string, Method> PassTheTest = new KeyValuePair<string, Method> ("/PassTheTest", Method.POST);
        public static KeyValuePair<string, Method> GetQuestions = new KeyValuePair<string, Method> ("/GetQuestions", Method.GET);
        public static KeyValuePair<string, Method> GetAnswers = new KeyValuePair<string, Method> ("/GetAnswers", Method.GET);
        public static KeyValuePair<string, Method> GetDeseaseNameById = new KeyValuePair<string, Method> ("/GetDeseaseNameById", Method.GET);
        public static KeyValuePair<string, Method> EvaluateAnswer = new KeyValuePair<string, Method>("/EvaluateAnswer", Method.POST);
    }

    /// <summary>
    /// StringValue — mKs port
    /// </summary>
    public enum MicroservicesEnum
    {
        [StringValue("50001")]
        Desease,
        [StringValue("50002")]
        Testing
    }

}
