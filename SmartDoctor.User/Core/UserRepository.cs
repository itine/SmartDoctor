using Microsoft.EntityFrameworkCore;
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
        public async Task<IEnumerable<Users>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

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
                       UserId = u.UserId.ToString(),
                       DateBirth = p.DateBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                       Fio = p.Fio,
                       Gender = p.Gender ? "male" : "female",
                       Password = u.Password,
                       PhoneNumber = u.PhoneNumber,
                       WorkPlace = p.WorkPlace
                   }).ToListAsync();
    }
}
