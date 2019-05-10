using Microsoft.EntityFrameworkCore;
using SmartDoctor.Data.ContextModels;
using SmartDoctor.Data.Enums;
using SmartDoctor.Data.JsonModels;
using SmartDoctor.Helper;
using System;
using System.Collections.Generic;
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
                DateBirth = user.DateBirth,
                Fio = user.Fio,
                Gender = bool.Parse(user.Gender),
                WorkPlace = user.WorkPlace,
                UserId = newUser.UserId
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        //public async Task UpdateUserData(PatientModel model)
        //{
        //    var userCtx = await _context.Patients.FirstOrDefaultAsync(x => x. == model.);
        //    if (userCtx == null)
        //        throw new Exception("Person not found");
        //    userCtx.Password = user.Password;
        //    userCtx.PhoneNumber = user.PhoneNumber;
        //    userCtx.Role = user.Role;
        //    await _context.SaveChangesAsync();
        //}

        public async Task<Users> GetUserById(long id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == id);
            if (user == null)
                throw new Exception("User not found");
            return user;
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
                       DateBirth = p.DateBirth,
                       Fio = p.Fio,
                       Gender = p.Gender ? "male" : "female",
                       Password = u.Password,
                       PhoneNumber = u.PhoneNumber,
                       WorkPlace = p.WorkPlace
                   }).ToListAsync();
    }
}
