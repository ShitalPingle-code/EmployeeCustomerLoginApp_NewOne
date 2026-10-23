using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeCustomerLoginApp.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
