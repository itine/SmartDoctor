using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using SmartDoctor.Data.ContextModels;
using SmartDoctor.Data.Enums;
using SmartDoctor.Data.JsonModels;
using SmartDoctor.Data.Models;
using SmartDoctor.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDoctor.User.Core
{
    public class UserRepository : IUserRepository
    {

        private readonly SmartDoctor_UsersContext _context;
        public UserRepository(SmartDoctor_UsersContext context)
        {
            _context = context;
        }
        public async Task<Users> Authorize(string phoneNumber, string password)
        {
            var person = await _context.Users.FirstOrDefaultAsync(p => p.PhoneNumber.Equals(phoneNumber));
            if (person == null)
                throw new Exception("Person not found");
            if (SecurePasswordHasher.ValidatePassword(password, person.Password))
                return person;
            else
                throw new Exception("wrong password");
        }

        public async Task RemoveUser(long userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
                throw new Exception("User not found");
            var patient = await _context.Patients.FirstOrDefaultAsync(x => x.UserId == userId);
            if (patient == null)
                throw new Exception("Patient not found");
            _context.Users.Remove(user);
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }

        public async Task Registration(PatientModel user)
        {
            if (!DateTime.TryParseExact(user.DateBirth, "dd/MM/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateBirth))
                throw new Exception("Date parsing error");
            var userInDb = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == user.PhoneNumber);
            if (userInDb != null)
                throw new Exception("User exists");

            var newUser = new Users
            {
                CreatedDate = DateTime.UtcNow,
                Password = SecurePasswordHasher.HashPassword(user.Password),
                PhoneNumber = user.PhoneNumber,
                Role = (byte)RoleTypes.Patient
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            var patient = new Patients
            {
                DateBirth = dateBirth,
                Fio = user.Fio,
                Gender = bool.Parse(user.Gender),
                WorkPlace = user.WorkPlace,
                UserId = newUser.UserId
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserData(PatientModel model)
        {
            if (!DateTime.TryParseExact(model.DateBirth, "dd/MM/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateBirth))
                throw new Exception("Date parsing error");
            var patientCtx = await _context.Patients.FirstOrDefaultAsync(x => x.UserId == long.Parse(model.UserId));
            if (patientCtx == null)
                throw new Exception("patient not found");
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == long.Parse(model.UserId));
            if (user == null)
                throw new Exception("user not found");
            patientCtx.DateBirth = dateBirth;
            patientCtx.Fio = model.Fio;
            patientCtx.Gender = model.Gender.Equals("male");
            patientCtx.WorkPlace = model.WorkPlace;
            user.PhoneNumber = model.PhoneNumber;
            user.Password = model.Password;
            await _context.SaveChangesAsync();
        }

        public async Task<Users> GetUserById(long id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == id);
            if (user == null)
                throw new Exception("User not found");
            return user;
        }

        public async Task<Patients> GetPatientById(long id)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(x => x.PatientId == id);
            if (patient == null)
                throw new Exception("Patient not found");
            return patient;
        }

        public async Task<Patients> GetPatientByUserId(long id)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(x => x.UserId == id);
            if (patient == null)
                throw new Exception("Patient not found");
            return patient;
        }
        public async Task<IEnumerable<Users>> GetUsers() =>
            await _context.Users.ToListAsync();

        public async Task<RoleTypes> GetRole(string phoneNumber)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber.Equals(phoneNumber));
            if (user == null)
                throw new Exception("User not found");
            return (RoleTypes)user.Role;
        }

        public async Task<IEnumerable<PatientModel>> GetPatients() =>
            await (from p in _context.Patients
                   join u in _context.Users on p.UserId equals u.UserId
                   select new PatientModel
                   {
                       PatientId = p.PatientId.ToString(),
                       UserId = u.UserId.ToString(),
                       DateBirth = p.DateBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                       Fio = p.Fio,
                       Gender = p.Gender ? "male" : "female",
                       Password = u.Password,
                       PhoneNumber = u.PhoneNumber,
                       WorkPlace = p.WorkPlace
                   }).ToListAsync();

        public async Task<IEnumerable<PatientModel>> GetPatientsNoReception()
        {
            var testingResponse = await RequestExecutor.ExecuteRequestAsync(
               MicroservicesEnum.Testing, RequestUrl.GetPatientsWithNoReception);
            var answersData = JsonConvert.DeserializeObject<MksResponse>(testingResponse);
            if (!answersData.Success)
                throw new Exception(answersData.Data);
            var patientsIds = JsonConvert.DeserializeObject<long[]>(answersData.Data);
            var result = new List<PatientModel>();
            if (patientsIds.Any())
                foreach (var id in patientsIds)
                {
                    result.AddRange(await (from p in _context.Patients
                           join u in _context.Users on p.UserId equals u.UserId
                           where p.PatientId == id
                           select new PatientModel
                           {
                               PatientId = p.PatientId.ToString(),
                               UserId = u.UserId.ToString(),
                               DateBirth = p.DateBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                               Fio = p.Fio,
                               Gender = p.Gender ? "male" : "female",
                               Password = u.Password,
                               PhoneNumber = u.PhoneNumber,
                               WorkPlace = p.WorkPlace
                           }).ToListAsync());
                }
            return result;
        }

        public async Task<PatientModel> GetPatientByFio(string fio) =>
            await (from p in _context.Patients
                   join u in _context.Users on p.UserId equals u.UserId
                   where p.Fio == fio
                   select new PatientModel
                   {
                       PatientId = p.PatientId.ToString(),
                       UserId = u.UserId.ToString(),
                       DateBirth = p.DateBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                       Fio = p.Fio,
                       Gender = p.Gender ? "male" : "female",
                       Password = u.Password,
                       PhoneNumber = u.PhoneNumber,
                       WorkPlace = p.WorkPlace
                   }).FirstOrDefaultAsync();

        public async Task<IEnumerable<PatientModel>> GetPatientsByIds(long[] ids)
        {
            var result = new List<PatientModel>();
            var medicalResponse = await RequestExecutor.ExecuteRequestAsync(
                MicroservicesEnum.Medical, RequestUrl.ActualizeIds,
                    new Parameter[] {
                            new Parameter("ids", JsonConvert.SerializeObject(ids), ParameterType.RequestBody)
                    });
            var medicalData = JsonConvert.DeserializeObject<MksResponse>(medicalResponse);
            if (!medicalData.Success)
                throw new Exception(medicalData.Data);
            ids = JsonConvert.DeserializeObject<long[]>(medicalData.Data);
            foreach (var id in ids)
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(x => x.PatientId == id);
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == patient.UserId);
                result.Add(new PatientModel
                {
                    PatientId = patient.PatientId.ToString(),
                    UserId = user.UserId.ToString(),
                    DateBirth = patient.DateBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Fio = patient.Fio,
                    Gender = patient.Gender ? "male" : "female",
                    Password = user.Password,
                    PhoneNumber = user.PhoneNumber,
                    WorkPlace = patient.WorkPlace
                });
            }
            return result;
        }

        public async Task<Users> GetUserByPatientId(long id)
        {
            var patient = await GetPatientById(id);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == patient.UserId);
            return user;
        }

        public async Task<IEnumerable<string>> GetDoctors()
        {
            var doctors = _context.Users.Where(x => x.Role == (byte)RoleTypes.Doctor);
            return await doctors.Select(x => x.Fio).ToListAsync();
        }

        public async Task<long> GetDoctorIdByFio(string fio)
        {
            var doctor = await _context.Users.FirstOrDefaultAsync(x => x.Fio.Equals(fio));
            if (doctor == null)
                throw new Exception("Doctor not found");
            return doctor.UserId;
        }
    }
}
