using System.Collections.Generic;

namespace SmartDoctor.Data.Models
{
    public class AnswerModel
    {
        public long UserId { get; set; }
        public string[] Answers { get; set; }
    }
}
