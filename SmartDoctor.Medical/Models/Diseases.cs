using System;
using System.Collections.Generic;

namespace SmartDoctor.Medical.Models
{
    public partial class Diseases
    {
        public int DiseaseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
