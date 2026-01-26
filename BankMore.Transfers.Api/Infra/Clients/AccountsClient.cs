using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BankMore.Transfers.Api.Infra.Clients
{

    public interface IAccountsClient
    {
        Task DebitAsync(string token, string requisitionId, decimal valor);
        Task CreditAsync(string token, string requisitionId, int numeroContaDestino, decimal valor);
        Task CreditSelfAsync(string token, string requisitionId, decimal valor);
    }

    public sealed class AccountsClient : IAccountsClient
    {
        private readonly HttpClient _http;

        public AccountsClient(HttpClient http)
        {
            _http = http;
        }

        public Task DebitAsync(string token, string requisitionId, decimal valor)
            => PostAsync(token, new { requisitionId, valor, tipoMovimento = "D" });

        public Task CreditAsync(string token, string requisitionId, int numeroContaDestino, decimal valor)
            => PostAsync(token, new { requisitionId, numeroConta = numeroContaDestino, valor, tipoMovimento = "C" });

        public Task CreditSelfAsync(string token, string requisitionId, decimal valor)
            => PostAsync(token, new { requisitionId, valor, tipoMovimento = "C" });

        private async Task PostAsync(string token, object payload)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, "/api/accounts/movements");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var res = await _http.SendAsync(req);
            if (!res.IsSuccessStatusCode)
            {
                var body = await res.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Accounts.Api erro {(int)res.StatusCode}: {body}");
            }
        }
    }
}
