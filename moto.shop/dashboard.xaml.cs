using moto.shop.Services;

namespace moto.shop;

public partial class dashboard : ContentPage
{
    private readonly ApiService api;

    public dashboard(ApiService apiService)
    {
        InitializeComponent();
        api = apiService; // ✅ use shared instance
    }

    private async void OnGetAllClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await api.GetUsers();
            ResultLabel.Text = result;
        }
        catch (Exception ex)
        {
            ResultLabel.Text = ex.Message;
        }
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                ResultLabel.Text = "Fill all fields!";
                return;
            }

            var result = await api.CreateUser(
                NameEntry.Text,
                EmailEntry.Text,
                PasswordEntry.Text
            );

            ResultLabel.Text = result;
        }
        catch (Exception ex)
        {
            ResultLabel.Text = ex.Message;
        }
    }

    private async void OnUpdateClicked(object sender, EventArgs e)
    {
        try
        {
            if (!int.TryParse(IdEntry.Text, out int id))
            {
                ResultLabel.Text = "Invalid ID";
                return;
            }

            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                ResultLabel.Text = "Enter name to update";
                return;
            }

            var result = await api.UpdateUser(id, NameEntry.Text);
            ResultLabel.Text = result;
        }
        catch (Exception ex)
        {
            ResultLabel.Text = ex.Message;
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        try
        {
            if (!int.TryParse(IdEntry.Text, out int id))
            {
                ResultLabel.Text = "Invalid ID";
                return;
            }

            var result = await api.DeleteUser(id);
            ResultLabel.Text = result;
        }
        catch (Exception ex)
        {
            ResultLabel.Text = ex.Message;
        }
    }
}