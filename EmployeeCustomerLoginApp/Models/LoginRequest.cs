using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeCustomerLoginApp.Models
{
    public class LoginRequest
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
