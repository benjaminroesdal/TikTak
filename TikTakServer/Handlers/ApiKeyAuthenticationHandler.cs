using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace TikTakServer.Handlers
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private const string ApiKeyHeaderName = "X-Api-Key";
        private readonly string _apiKey;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration) : base(options, logger, encoder)
        {
            _apiKey = configuration["ApiKey"]; // Load the API key from configuration
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                return AuthenticateResult.NoResult();
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
            {
                return AuthenticateResult.NoResult();
            }

            if (providedApiKey == _apiKey)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "ApiKeyUser") };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.Fail("Invalid API Key provided.");
        }
    }
}
