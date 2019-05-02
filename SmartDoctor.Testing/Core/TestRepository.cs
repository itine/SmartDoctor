using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartDoctor.Testing.Models;

namespace SmartDoctor.Testing.Core
{
    public class TestRepository : ITestRepository
    {
        private readonly SmartDoctor_TestDataContext _context;

        public TestRepository(SmartDoctor_TestDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Questions>> GetTest()
        {
            var questions = await _context.Questions.ToListAsync();
            if (!questions.Any())
                throw new Exception("Questions not found");
            return questions;
        }
    }
}
