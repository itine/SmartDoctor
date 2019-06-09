using SmartDoctor.Data.JsonModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDoctor.Medical.Core
{
    public interface IOutpatientRepository
    {
        Task<IEnumerable<OutpatientModel>> GetAllOutPatients();
        Task<OutpatientModel> GetOutpatientById(long cardId);
        Task ChangeStatus(CardStatusModel model);
        Task UpdateDescription(CardDescriptionModel model);
        Task CreateOutpatientCard(long userId);
        Task<OutpatientModel> GetOutpatientByPatientAndDoctorId(DoctorPatientModel model);
    }
}
