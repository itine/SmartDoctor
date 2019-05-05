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
        private readonly SmartDoctor_DeseasesContext _context;

        public DiseaseRepository(SmartDoctor_DeseasesContext context)
        {
            _context = context;
        }

        public async Task<Deseases> DiseaseDiagnostic(long answerId)
        {
            var diseaseResponse = await RequestExecutor.ExecuteRequestAsync(
                  MicroservicesEnum.Testing, RequestUrl.EvaluateAnswer,
                      new Parameter[] {
                            new Parameter("id", 1, ParameterType.RequestBody)
                      });
            var diseaseName = JsonConvert.DeserializeObject<MksResponse>(diseaseResponse);
            return null;
        }

        public async Task<Deseases> GetDiseaseById(int diseaseId)
        {
            var disease = await _context.Deseases.FirstOrDefaultAsync(x => x.DeseaseId == diseaseId);
            if (disease == null)
                throw new Exception("Disease not found");
            return disease;
        }
    }
}
