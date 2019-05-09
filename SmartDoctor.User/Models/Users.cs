﻿using System;
using System.Collections.Generic;

namespace SmartDoctor.User.Models
{
    public partial class Users
    {
        public long UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public byte? Role { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}