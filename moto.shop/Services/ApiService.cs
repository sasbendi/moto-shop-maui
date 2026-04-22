using System.Net.Http.Headers;
using System.Text.Json;

namespace moto.shop.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string _token;

        private const string BaseUrl = "http://localhost:8000/api/";

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        private void SetAuthHeader()
        {
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _token);
            }
        }

        // ================= LOGIN =================
        public async Task<bool> Login(string email, string password)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("email", email),
                new KeyValuePair<string,string>("password", password)
            });

            var response = await _httpClient.PostAsync("login", content);

            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            // ✅ YOUR API FORMAT: [ user, token ]
            if (doc.RootElement.ValueKind == JsonValueKind.Array &&
                doc.RootElement.GetArrayLength() > 1)
            {
                _token = doc.RootElement[1].GetString();
            }

            return !string.IsNullOrEmpty(_token);
        }

        // ================= GET CURRENT USER =================
        public async Task<string> GetCurrentUser()
        {
            SetAuthHeader();
            var res = await _httpClient.GetAsync("user");
            return await res.Content.ReadAsStringAsync();
        }

        // ================= GET ALL USERS =================
        public async Task<string> GetUsers()
        {
            SetAuthHeader();
            var res = await _httpClient.GetAsync("users");
            return await res.Content.ReadAsStringAsync();
        }

        // ================= CREATE USER =================
        public async Task<string> CreateUser(string name, string email, string password)
        {
            SetAuthHeader();

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("name", name),
                new KeyValuePair<string,string>("email", email),
                new KeyValuePair<string,string>("password", password),
                new KeyValuePair<string,string>("password_confirmation", password)
            });

            var res = await _httpClient.PostAsync("users", content);
            return await res.Content.ReadAsStringAsync();
        }

        // ================= UPDATE USER =================
        public async Task<string> UpdateUser(int id, string name)
        {
            SetAuthHeader();

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("name", name)
            });

            var res = await _httpClient.PutAsync($"users/{id}", content);
            return await res.Content.ReadAsStringAsync();
        }

        // ================= DELETE USER =================
        public async Task<string> DeleteUser(int id)
        {
            SetAuthHeader();
            var res = await _httpClient.DeleteAsync($"users/{id}");
            return await res.Content.ReadAsStringAsync();
        }
    }
}