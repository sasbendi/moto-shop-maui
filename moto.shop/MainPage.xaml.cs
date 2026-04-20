using moto.shop.Services;
using System.Text.Json;

namespace moto.shop;

public partial class MainPage : ContentPage
{
    ApiService api = new ApiService();

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        ErrorLabel.Text = "";

        // ================= VALIDATION =================
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            ErrorLabel.Text = "Please fill all fields";
            return;
        }

        try
        {
            // ================= LOGIN =================
            var success = await api.Login(EmailEntry.Text, PasswordEntry.Text);

            if (!success)
            {
                ErrorLabel.Text = "Invalid login";
                return;
            }

            // ================= GET USER =================
            var userJson = await api.GetCurrentUser();
            var doc = JsonDocument.Parse(userJson);

            bool isAdmin = false;

            // direct format: { is_admin: 1 }
            if (doc.RootElement.TryGetProperty("is_admin", out var adminProp))
            {
                isAdmin = adminProp.GetInt32() == 1;
            }
            // laravel format: { data: { is_admin: 1 } }
            else if (doc.RootElement.TryGetProperty("data", out var dataProp))
            {
                if (dataProp.TryGetProperty("is_admin", out var nestedAdmin))
                {
                    isAdmin = nestedAdmin.GetInt32() == 1;
                }
            }

            // ================= REDIRECT =================
            if (isAdmin)
            {
                await Navigation.PushAsync(new dashboard(api));
            }
            else
            {
                ErrorLabel.Text = "You are not an admin";
            }
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = "Error: " + ex.Message;
        }
    }
}