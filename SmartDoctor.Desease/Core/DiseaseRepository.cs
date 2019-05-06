using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using SmartDoctor.Data.Models;
using SmartDoctor.Helper;
using SmartDoctor.Testing.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDoctor.Desease.Core
{
    public class DiseaseRepository : IDiseaseRepository
    {
        private readonly SmartDoctor_DiseasesContext _context;

        public DiseaseRepository(SmartDoctor_DiseasesContext context)
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

        public async Task<string> GetDeseaseNameById(int diseaseId)
        {
            var disease = await _context.Diseases.FirstOrDefaultAsync(x => x.DiseaseId == diseaseId);
            if (disease == null)
                throw new Exception("Disease not found");
            return disease.Name;
        }
    }
}
