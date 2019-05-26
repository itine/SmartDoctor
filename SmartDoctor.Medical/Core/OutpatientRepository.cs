using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using SmartDoctor.Data.ContextModels;
using SmartDoctor.Data.Enums;
using SmartDoctor.Data.JsonModels;
using SmartDoctor.Data.Models;
using SmartDoctor.Helper;

namespace SmartDoctor.Medical.Core
{
    public class OutpatientRepository : IOutpatientRepository
    {
        private readonly SmartDoctor_MedicalContext _context;
        public OutpatientRepository(SmartDoctor_MedicalContext context)
        {
            _context = context;
        }

        public async Task ChangeStatus(CardStatusModel model)
        {
            var outpatientCard = await _context.OutpatientCards.FirstOrDefaultAsync(x => x.OutpatientCardId == model.CardId);
            if (outpatientCard == null)
                throw new Exception($"Outpatient card not found. {model.CardId}");
            outpatientCard.Status = model.Status;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OutpatientModel>> GetAllOutPatients()
        {
            var outpatients = await _context.OutpatientCards.Where(x => x.DiseaseId.HasValue).ToListAsync();
            var result = new List<OutpatientModel>();
            if (outpatients.Any())
                foreach (var outpatient in outpatients)
                    result.Add(await GetOutpatientById(outpatient.OutpatientCardId));
            return result;
        }

        public async Task<OutpatientModel> GetOutpatientById(long cardId)
        {
            var outpatient = await _context.OutpatientCards.FirstOrDefaultAsync(x => x.OutpatientCardId == cardId);
            var newOutpatient = new OutpatientModel
            {
                CreatedDate = outpatient.CreatedDate.Value,
                Description = outpatient.Description,
                OutpatientCardId = outpatient.OutpatientCardId,
                Status = (OutpatientStatuses)outpatient.Status
            };
            var userResponse = await RequestExecutor.ExecuteRequestAsync(
               MicroservicesEnum.User, RequestUrl.GetPatientById,
                   new Parameter[]
                   {
                        new Parameter( "patientId", (int)outpatient.PatientId, ParameterType.GetOrPost)
                   });
            var patientData = JsonConvert.DeserializeObject<MksResponse>(userResponse);
            if (!patientData.Success)
                throw new Exception(patientData.Data);
            var patientCtx = JsonConvert.DeserializeObject<Patients>(patientData.Data);
            newOutpatient.Patient = patientCtx;
            var diseaseResponse = await RequestExecutor.ExecuteRequestAsync(
                MicroservicesEnum.Medical, RequestUrl.GetDiseaseNameById,
                    new Parameter[] {
                        new Parameter("diseaseId", outpatient.DiseaseId.Value, ParameterType.GetOrPost)
                    });
            var diseaseResponseName = JsonConvert.DeserializeObject<MksResponse>(diseaseResponse);
            if (!diseaseResponseName.Success)
                throw new Exception(diseaseResponseName.Data);
            newOutpatient.Disease = JsonConvert.DeserializeObject<string>(diseaseResponseName.Data);
            return newOutpatient;
        }
    }
}
