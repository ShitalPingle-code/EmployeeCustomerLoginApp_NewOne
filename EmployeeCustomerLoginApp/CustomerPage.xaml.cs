
using EmployeeCustomerLoginApp.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace EmployeeCustomerLoginApp;

public partial class CustomerPage : ContentPage
{
    private readonly HttpClient client = new HttpClient
    {
        BaseAddress = new Uri("https://dummyapis.ciitstudent.com/")
    };

    public CustomerPage()
    {
        InitializeComponent();
    }

    // Retrieve customers
    private async void OnLoadCustomersClicked(object sender, EventArgs e)
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            MessageLabel.Text = "";

            string? token =
                await SecureStorage.Default.GetAsync("auth_token");

            if (string.IsNullOrWhiteSpace(token))
            {
                await DisplayAlert(
                    "Login Required",
                    "Please login first.",
                    "OK");
                return;
            }

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response =
                await client.GetAsync("api/customer");

            if (response.IsSuccessStatusCode)
            {
                List<Customer>? customers =
                    await response.Content
                        .ReadFromJsonAsync<List<Customer>>();

                CustomersCollectionView.ItemsSource =
                    customers ?? new List<Customer>();

                MessageLabel.Text =
                    $"Total customers: {customers?.Count ?? 0}";
            }
            else
            {
                MessageLabel.Text =
                    $"Retrieve failed: {(int)response.StatusCode} - {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            MessageLabel.Text = ex.Message;
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    // Insert customer
    private async void OnAddCustomerClicked(object sender, EventArgs e)
    {
        try
        {
            string? token =
                await SecureStorage.Default.GetAsync("auth_token");

            if (string.IsNullOrWhiteSpace(token))
            {
                await DisplayAlert(
                    "Login Required",
                    "Please login first.",
                    "OK");
                return;
            }

            Customer customer = new Customer
            {
                CustomerId = 0,
                CustomerName = CustomerNameEntry.Text?.Trim() ?? "",
                EmailAddress = EmailEntry.Text?.Trim() ?? "",
                MobileNumber = MobileEntry.Text?.Trim() ?? "",
                City = CityEntry.Text?.Trim() ?? ""
            };

            if (string.IsNullOrWhiteSpace(customer.CustomerName) ||
                string.IsNullOrWhiteSpace(customer.EmailAddress) ||
                string.IsNullOrWhiteSpace(customer.MobileNumber) ||
                string.IsNullOrWhiteSpace(customer.City))
            {
                MessageLabel.Text = "Please enter all customer details.";
                return;
            }

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response =
                await client.PostAsJsonAsync("api/customer", customer);

            if (response.IsSuccessStatusCode)
            {
                Customer? result =
                    await response.Content.ReadFromJsonAsync<Customer>();

                await DisplayAlert(
                    "Success",
                    $"Customer inserted successfully. ID: {result?.CustomerId}",
                    "OK");

                CustomerIdEntry.Text =
                    result?.CustomerId.ToString() ?? "";

                CustomerNameEntry.Text = "";
                EmailEntry.Text = "";
                MobileEntry.Text = "";
                CityEntry.Text = "";

                // Refresh the customer list after inserting
                OnLoadCustomersClicked(sender, e);
            }
            else
            {
                string error =
                    await response.Content.ReadAsStringAsync();

                MessageLabel.Text =
                    $"Insert failed: {(int)response.StatusCode}. {error}";
            }
        }
        catch (Exception ex)
        {
            MessageLabel.Text = ex.Message;
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    // Select a customer from the list
    private void OnSelectCustomerClicked(object sender, EventArgs e)
    {
        if (sender is Button button &&
            button.CommandParameter is Customer customer)
        {
            CustomerIdEntry.Text = customer.CustomerId.ToString();
            CustomerNameEntry.Text = customer.CustomerName;
            EmailEntry.Text = customer.EmailAddress;
            MobileEntry.Text = customer.MobileNumber;
            CityEntry.Text = customer.City;

            MessageLabel.Text =
                $"Selected customer ID: {customer.CustomerId}";
        }
    }
}
