using System;

namespace SmartDoctor.Data.ContextModels
{
    public partial class OutpatientCards
    {
        public long OutpatientCardId { get; set; }
        public long PatientId { get; set; }
        public long DoctorId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? DiseaseId { get; set; }
        public byte? Status { get; set; }
        public string Description { get; set; }
    }
}
