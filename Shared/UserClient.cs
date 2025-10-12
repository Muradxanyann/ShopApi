using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Shared.Dto.UserDto;

namespace Shared;

public class UserClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserClient> _logger;

    public UserClient(HttpClient httpClient,  ILogger<UserClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UserDto?> GetUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting user {userId}", userId);
        var response = await _httpClient.GetAsync($"http://localhost:5001/api/Users/{userId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: cancellationToken);
    }
}