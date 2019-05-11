using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using SmartDoctor.Data.ContextModels;
using SmartDoctor.Data.Models;
using SmartDoctor.Helper;
using System;
using System.Threading.Tasks;

namespace SmartDoctor.Medical.Core
{
    public class DiseaseRepository : IDiseaseRepository
    {
        private readonly SmartDoctor_MedicalContext _context;

        public DiseaseRepository(SmartDoctor_MedicalContext context)
        {
            _context = context;
        }

        public async Task<Diseases> DiseaseDiagnostic(long answerId)
        {
            var diseaseResponse = await RequestExecutor.ExecuteRequestAsync(
                  MicroservicesEnum.Testing, RequestUrl.EvaluateAnswer,
                      new Parameter[] {
                            new Parameter("id", 1, ParameterType.RequestBody)
                      });
            var diseaseName = JsonConvert.DeserializeObject<MksResponse>(diseaseResponse);
            return null;
        }

        public async Task<long> GetDiseaseIdByName(string name)
        {
            var disease = await _context.Diseases.FirstOrDefaultAsync(x => x.Name == name);
            if (disease == null)
                throw new Exception("Disease not found");
            return disease.DiseaseId;
        }
        

        public async Task<string> GetDiseaseNameById(long diseaseId)
        {
            var disease = await _context.Diseases.FirstOrDefaultAsync(x => x.DiseaseId == diseaseId);
            if (disease == null)
                throw new Exception("Disease not found");
            return disease.Name;
        }
    }
}
