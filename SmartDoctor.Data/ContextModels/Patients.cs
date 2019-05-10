using System;
namespace SmartDoctor.Data.ContextModels
{
    public partial class Patients
    {
        public long PatientId { get; set; }
        public string Fio { get; set; }
        public DateTime DateBirth { get; set; }
        public string WorkPlace { get; set; }
        public bool Gender { get; set; }
        public string SpecificNumber { get; set; }
        public long UserId { get; set; }
    }
}
