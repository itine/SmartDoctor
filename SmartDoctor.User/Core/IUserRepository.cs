using SmartDoctor.Data.Enums;
using SmartDoctor.User.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDoctor.User.Core
{
    public interface IUserRepository
    {
        Task Registration(Users user);
        Task UpdateUserData(Users user);
        Task RemoveUser(long userId);
        Task<bool> Authorize(string phoneNumber, string password);
        Task<Users> GetUserById(long id);
        Task<IEnumerable<Users>> GetUsers();
        Task<RoleTypes> GetRole(string phoneNumber);
    }
}
