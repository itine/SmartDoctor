using System;

namespace SmartDoctor.Data.ContextModels
{
    public partial class Users
    {
        public long UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public byte? Role { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Fio { get; set; }
    }
}
