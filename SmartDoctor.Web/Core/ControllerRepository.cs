using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using SmartDoctor.Data.ContextModels;
using SmartDoctor.Data.Enums;
using SmartDoctor.Data.Models;
using SmartDoctor.Helper;

namespace SmartDoctor.Web.Core
{
    public class ControllerRepository : IControllerRepository
    {

        public async Task<RoleTypes> InitRole(ClaimsPrincipal claimsPrincipal)
        {
            var identity = (ClaimsIdentity)claimsPrincipal.Identity;
            if (identity.Claims.Any())
            {
                var userId = identity.Claims.ToList()[0].Value;
                var userResponse = JsonConvert.DeserializeObject<MksResponse>(await RequestExecutor.ExecuteRequestAsync(
                   MicroservicesEnum.User, RequestUrl.GetUserById,
                       new Parameter[] {
                            new Parameter("userId", long.Parse(userId), ParameterType.RequestBody)
                       }));
                var user = JsonConvert.DeserializeObject<Users>(userResponse.Data);
                return (RoleTypes)user.Role;
            }
            return RoleTypes.None;
        }
    }
}
