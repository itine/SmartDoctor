using SmartDoctor.Data.ContextModels;
using SmartDoctor.Data.Enums;
using SmartDoctor.Data.JsonModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDoctor.User.Core
{
    public interface IUserRepository
    {
        Task Registration(PatientModel user);
        Task UpdateUserData(PatientModel user);
        Task RemoveUser(long userId);
        Task<Users> Authorize(string phoneNumber, string password);
        Task<Users> GetUserById(long id);
        Task<Patients> GetPatientById(long id);
        Task<Patients> GetPatientByUserId(long id);
        Task<IEnumerable<Users>> GetUsers();
        Task<IEnumerable<PatientModel>> GetPatients();
        Task<RoleTypes> GetRole(string phoneNumber);
    }
}
