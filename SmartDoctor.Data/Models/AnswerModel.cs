using System.Collections.Generic;

namespace SmartDoctor.Data.Models
{
    public class AnswerModel
    {
        public long PatientId { get; set; }
        public IEnumerable<string> Answers { get; set; }
    }
}
