using Microsoft.EntityFrameworkCore;
using SmartDoctor.Data.Enums;
using SmartDoctor.Helper;
using SmartDoctor.User.Models;
using System;
using System.Collections.Generic;
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
        public async Task<bool> Authorize(string phoneNumber, string password)
        {
            var person = await _context.Users.FirstOrDefaultAsync(p => p.PhoneNumber.Equals(phoneNumber));
            if (person == null)
                throw new Exception("Person not found");
            return SecurePasswordHasher.ValidatePassword(password, person.Password);
        }      

        public async Task RemoveUser(long userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
                throw new Exception("User not found");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task Registration(Users user)
        {
            var userInDb = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == user.PhoneNumber);
            if (userInDb != null)
                throw new Exception("User exists");
            var newUser = new Users
            {
                CreatedDate = DateTime.UtcNow,
                Password = SecurePasswordHasher.HashPassword(user.Password),
                PhoneNumber = user.PhoneNumber,
                Role = user.Role ?? (byte)RoleTypes.Patient
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserData(Users user)
        {
            var userCtx = await _context.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId);
            if (userCtx == null)
                throw new Exception("Person not found");
            userCtx.Password = user.Password;
            userCtx.PhoneNumber = user.PhoneNumber;
            userCtx.Role = user.Role;
            await _context.SaveChangesAsync();
        }

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
    }
}
