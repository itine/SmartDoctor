using System.Collections.Generic;

namespace SmartDoctor.Data.Models
{
    public class AnswerModel
    {
        public long UserId { get; set; }
        public AnswerItem[] Answers { get; set; }
    }
    public class AnswerItem
    {
        public string Answer { get; set; }
    }
}
