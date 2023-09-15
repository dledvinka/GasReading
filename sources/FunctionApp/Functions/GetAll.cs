using System.Collections.Generic;
using System.Net;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp.Functions
{

    public class GetAll
    {
        private readonly ILogger _logger;

        public GetAll(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetAll>();
        }

        [Function("GetAll")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
                                    [Table("GasReadings")] TableClient tableClient)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }
}
