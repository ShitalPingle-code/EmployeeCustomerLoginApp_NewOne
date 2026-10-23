
namespace EmployeeCustomerLoginApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Logout",
            "Are you sure you want to logout?",
            "Yes",
            "No");

        if (!confirm)
            return;

        // Clear saved login information
        SecureStorage.Default.Remove("auth_token");
        SecureStorage.Default.Remove("employee_code");

        // Return to the employee login page
        await GoToAsync("//MainPage");
    }
}
