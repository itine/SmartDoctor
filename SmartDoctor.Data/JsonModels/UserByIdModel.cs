using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SmartDoctor.Data.JsonModels
{
    public class UserByIdModel
    {
        [Required, JsonProperty("userId")]
        public long UserId { get; set; }
    }
}
