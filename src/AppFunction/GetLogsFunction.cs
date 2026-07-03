using System.Net;
using AppFunction.Models;
using AppFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AppFunction;

public class GetLogsFunction
{
    private readonly ILogger<GetLogsFunction> _logger;
    private readonly ITableStorageService _tableStorageService;

    public GetLogsFunction(
        ILogger<GetLogsFunction> logger,
        ITableStorageService tableStorageService)
    {
        _logger = logger;
        _tableStorageService = tableStorageService;
    }

    [Function("GetLogs")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "logs")] HttpRequestData req)
    {
        _logger.LogInformation("Processing request to fetch logs from Table Storage.");

        try
        {
            var logs = await _tableStorageService.GetAllAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(logs);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching records from Table Storage.");
            
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("An error occurred while retrieving data.");
            
            return errorResponse;
        }
    }
}

