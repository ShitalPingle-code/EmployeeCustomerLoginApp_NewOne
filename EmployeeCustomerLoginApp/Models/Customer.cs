namespace EmployeeCustomerLoginApp.Models;

public class Customer
{
    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;
}