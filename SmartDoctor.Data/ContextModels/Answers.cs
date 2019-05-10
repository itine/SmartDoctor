using System;

namespace SmartDoctor.Data.ContextModels
{
    public partial class Answers
    {
        public long AnswerId { get; set; }
        public string AnswerData { get; set; }
        public long? PatientId { get; set; }
        public DateTime? AnswerDate { get; set; }
        public bool? IsTakenToCalculate { get; set; }
        public long? DeseaseId { get; set; }
    }
}
