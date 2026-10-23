
using EmployeeCustomerLoginApp.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace EmployeeCustomerLoginApp;

public partial class MainPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://dummyapis.ciitstudent.com/")
    };

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        MessageLabel.Text = string.Empty;

        string employeeCode = EmployeeCodeEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(employeeCode) ||
            string.IsNullOrWhiteSpace(password))
        {
            MessageLabel.Text =
                "Please enter employee code and password.";
            return;
        }

        try
        {
            LoginButton.IsEnabled = false;
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            var request = new LoginRequest
            {
                EmployeeCode = employeeCode,
                Password = password
            };

            HttpResponseMessage response =
                await _httpClient.PostAsJsonAsync(
                    "api/login", request);

            string responseBody =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                MessageLabel.Text =
                    $"Login failed: {(int)response.StatusCode}";
                return;
            }

            var result =
                JsonSerializer.Deserialize<LoginResponse>(
                    responseBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (result == null ||
                string.IsNullOrWhiteSpace(result.Token))
            {
                MessageLabel.Text =
                    "Token was not returned by the API.";
                return;
            }

            await SecureStorage.Default.SetAsync(
                "auth_token", result.Token);

            await SecureStorage.Default.SetAsync(
                "employee_code", employeeCode);

            //await DisplayAlert(
            //    "Success",
            //    "Employee login successful!",
            //    "OK");

            // Navigate to CustomerPage after successful login
         
            await Shell.Current.GoToAsync("//CustomerPage");
        }
        catch (HttpRequestException)
        {
            MessageLabel.Text =
                "Cannot connect to the server. Check your internet connection.";
        }
        catch (JsonException)
        {
            MessageLabel.Text =
                "Invalid response received from the server.";
        }
        catch (Exception ex)
        {
            MessageLabel.Text = ex.Message;
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            LoginButton.IsEnabled = true;
        }
    }
}
