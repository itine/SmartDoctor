using SmartDoctor.Data.Enums;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartDoctor.Web.Core
{
    public interface IControllerRepository
    {
        Task<RoleTypes> InitRole(ClaimsPrincipal claimsPrincipal);
    }
}
