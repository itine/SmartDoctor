using RestSharp;
using SmartDoctor.Data.Consts;
using SmartDoctor.Data.Enums;
using System.Net;
using System.Threading.Tasks;

namespace SmartDoctor.Helper
{
    public class RequestExecutor
    {
        public static async Task<string> ExecuteRequestAsync(MicroservicesEnum microservice, 
            RequestUrls requestUrl, Parameter[] requestParameters)
        {
            var method = (byte)requestUrl == Scope.GetMethodId ? Method.GET : Method.POST;
            var request = new RestRequest(requestUrl.GetStringValue(), method)
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
    
    public enum RequestUrls : byte
    {
        [StringValue("/GetTest")]
        GetTest = Scope.GetMethodId,
        [StringValue("/PassTheTest")]
        PassTheTest = Scope.PostMethodId
    }

    /// <summary>
    /// StringValue — mKs port
    /// </summary>
    public enum MicroservicesEnum
    {
        [StringValue("50001")]
        Desease,
        [StringValue("50002")]
        Todo
    }

}
