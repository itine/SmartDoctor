using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace SmartDoctor.Data.JsonModels
{
    public class PatientModel
    {
        [Required, JsonProperty("UserId")]
        public string UserId { get; set; }

        [Required, JsonProperty("Fio")]
        public string Fio { get; set; }

        [Required, JsonProperty("DateBirth")]
        public DateTime DateBirth { get; set; }

        [Required, JsonProperty("WorkPlace")]
        public string WorkPlace { get; set; }

        [Required, JsonProperty("Gender")]
        public string Gender { get; set; }
        
        [Required, JsonProperty("PhoneNumber")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required, JsonProperty("Password")]
        public string Password { get; set; }
    }
}
