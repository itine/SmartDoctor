using SmartDoctor.Data.ContextModels;
using SmartDoctor.Data.Enums;
using System;

namespace SmartDoctor.Data.JsonModels
{
    public class OutpatientModel
    {
        public long OutpatientCardId { get; set; }
        public Patients Patient { get; set; }
        public Users Doctor { get; set; }
        public string CreatedDate { get; set; }
        public string Disease { get; set; }
        public OutpatientStatuses Status { get; set; }
        public string Description { get; set; }
    }
}
