﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
                CreatedDate = outpatient.CreatedDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                Description = outpatient.Description,
                OutpatientCardId = outpatient.OutpatientCardId,
                Status = (OutpatientStatuses)outpatient.Status
            };
            var userResponse = await RequestExecutor.ExecuteRequestAsync(
               MicroservicesEnum.User, RequestUrl.GetPatientById,
                   new Parameter[]
                   {
                        new Parameter("patientId", (int)outpatient.PatientId, ParameterType.GetOrPost)
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

        public async Task CreateOutpatientCard(long userId)
        {
            var userResponse = await RequestExecutor.ExecuteRequestAsync(
               MicroservicesEnum.User, RequestUrl.GetUsers);
            var userData = JsonConvert.DeserializeObject<MksResponse>(userResponse);
            if (!userData.Success)
                throw new Exception(userData.Data);
            var users = JsonConvert.DeserializeObject<List<Users>>(userData.Data);
            var freeDoctor = users.FirstOrDefault(x => x.Role == (byte)RoleTypes.Doctor);
            if (freeDoctor == null)
                throw new Exception("Free doctors not found");
            var userResponse2 = await RequestExecutor.ExecuteRequestAsync(
              MicroservicesEnum.User, RequestUrl.GetPatientByUserId, new Parameter[]
                   {
                        new Parameter("userId", (int)userId, ParameterType.GetOrPost)
                   });
            var userData2 = JsonConvert.DeserializeObject<MksResponse>(userResponse2);
            if (!userData2.Success)
                throw new Exception(userData2.Data);
            var patient = JsonConvert.DeserializeObject<Patients>(userData2.Data);
            var newOutpatientCard = new OutpatientCards
            {
                CreatedDate = DateTime.UtcNow,
                Description = "Card created successfully",
                DoctorId = freeDoctor.UserId,
                PatientId = patient.PatientId,
                Status = (byte)OutpatientStatuses.OutpatientCardCreating
            };
            _context.OutpatientCards.Add(newOutpatientCard);
            await _context.SaveChangesAsync();
        }

        public async Task<OutpatientModel> GetOutpatientByPatientAndDoctorId(DoctorPatientModel model)
        {
            var outpatient = await _context.OutpatientCards
                .FirstOrDefaultAsync(x => x.PatientId == model.PatientId && x.DoctorId == model.DoctorId);
            var newOutpatient = new OutpatientModel
            {
                CreatedDate = outpatient.CreatedDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                Description = outpatient.Description,
                OutpatientCardId = outpatient.OutpatientCardId,
                Status = (OutpatientStatuses)outpatient.Status
            };
            var userResponse = await RequestExecutor.ExecuteRequestAsync(
               MicroservicesEnum.User, RequestUrl.GetPatientById,
                   new Parameter[]
                   {
                        new Parameter("patientId", (int)outpatient.PatientId, ParameterType.GetOrPost)
                   });
            var patientData = JsonConvert.DeserializeObject<MksResponse>(userResponse);
            if (!patientData.Success)
                throw new Exception(patientData.Data);
            var patientCtx = JsonConvert.DeserializeObject<Patients>(patientData.Data);
            newOutpatient.Patient = patientCtx;
            var diseaseResponse = await RequestExecutor.ExecuteRequestAsync(
                MicroservicesEnum.Medical, RequestUrl.GetDiseaseNameById,
                    new Parameter[] {
                        new Parameter("diseaseId", model.DiseaseId, ParameterType.GetOrPost)
                    });
            var diseaseResponseName = JsonConvert.DeserializeObject<MksResponse>(diseaseResponse);
            var diseaseName = string.Empty;
            if (diseaseResponseName.Success)
                diseaseName = JsonConvert.DeserializeObject<string>(diseaseResponseName.Data);
            outpatient.DiseaseId = model.DiseaseId;
            await _context.SaveChangesAsync();
            newOutpatient.Disease = diseaseName;
            return newOutpatient;
        }

        public async Task UpdateDescription(CardDescriptionModel model)
        {
            var outpatientCard = await _context.OutpatientCards.FirstOrDefaultAsync(x => x.OutpatientCardId == model.CardId);
            if (outpatientCard == null)
                throw new Exception($"Outpatient card not found. {model.CardId}");
            outpatientCard.Description += "\n" + model.Description;
          
            if (!outpatientCard.DiseaseId.HasValue)
            {
                var diseaseResponse = await RequestExecutor.ExecuteRequestAsync(
                     MicroservicesEnum.Medical, RequestUrl.GetDiseaseIdByName,
                new Parameter[] {
                        new Parameter("name", model.Disease, ParameterType.GetOrPost)
                });
                var diseaseResponseName = JsonConvert.DeserializeObject<MksResponse>(diseaseResponse);
                if (!diseaseResponseName.Success)
                    throw new Exception("Disease not found");
                outpatientCard.DiseaseId = JsonConvert.DeserializeObject<long>(diseaseResponseName.Data);
            }
                
            await _context.SaveChangesAsync();
        }

        public async Task<long[]> ActualizeIds(long[] ids)
        {
            var existingIds = await _context.OutpatientCards
                .Where(x => x.Status == (byte)OutpatientStatuses.OutpatientCardCreating)
                .Select(x => x.PatientId)
                .Intersect(ids)
                .ToArrayAsync();
            if (existingIds == null)
                existingIds = new long[0];
            return existingIds;
        }

        public async Task SetDisease(string disease)
        {
            throw new NotImplementedException();
        }
    }
}
