// See https://aka.ms/new-console-template for more information

// based on: https://centraluseuap.dev.cognitive.microsoft.com/docs/services/unified-vision-apis-public-preview-2023-02-01-preview/operations/61d65934cd35050c20f73ab6
// test app: https://centraluseuap.dev.cognitive.microsoft.com/docs/services/unified-vision-apis-public-preview-2023-02-01-preview/operations/61d65934cd35050c20f73ab6/console
// TODO: credentials : https://stackoverflow.com/questions/2397822/what-is-the-best-practice-for-dealing-with-passwords-in-git-repositories


using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Azure.Data.Tables;
using ConsoleApp;
using Core.Entities;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");

var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .AddEnvironmentVariables()
                            .Build();

var settings = config.GetRequiredSection("Settings").Get<Settings>();

if (settings is null)
    return;

await TestTableAccess();



//await MakeRequest();
Console.WriteLine("Hit ENTER to exit...");
Console.ReadLine();

async Task TestTableAccess()
{
    //var tableServiceEndpoint = @"https://gasreadingappstorage.table.core.windows.net/";
    var tableName = "GasReadings";

    //var accountName = "gasreadingappstorage";
    //var storageAccountKey = "84RiPJyG3Fs5uXMWFwJcmJ1lzJcckjHKK1uQDMQ+EQNno/rw/hX2f2jvrka+tg3jzhSSSRZajem0+AStbYRZFg==";


    // Azure.Data.Tables
    // https://github.com/Azure/azure-sdk-for-net/blob/Azure.Data.Tables_12.8.1/sdk/tables/Azure.Data.Tables/README.md


    // Construct a new <see cref="TableClient" /> using a <see cref="TableSharedKeyCredential" />.
    var tableClient = new TableClient(new Uri(settings.AzureTableStorageServiceEndpoint),
                                      tableName,
                                      new TableSharedKeyCredential(settings.AzureTableStorageAccountName, settings.AzureTableStoragePassword));
    
    //var deleteResponse = await tableClient.DeleteAsync();

    // Create the table in the service.
    var createResponse = await tableClient.CreateIfNotExistsAsync();

    var lines = await File.ReadAllLinesAsync("InitialValues.csv");

    foreach (var line in lines)
    {
        var splitValues = line.Split(',');

        var date = DateTime.SpecifyKind(DateTime.Parse(splitValues[0]), DateTimeKind.Utc);
        var meterValue = double.Parse(splitValues[1]);

        var newEntry = new GasMeterReading()
        {
            PartitionKey = Guid.NewGuid().ToString(),
            RowKey = Guid.NewGuid().ToString(),

            ReadingDateUtc = date,
            MeterValue = meterValue
        };

        var response = await tableClient.AddEntityAsync(newEntry);
    }

    //var initialMeterValue = 5000.0;

    //for (var i = 0; i < 20; i++)
    //{
    //    var newEntry = new GasMeterReading()
    //    {
    //        PartitionKey = Guid.NewGuid().ToString(),
    //        RowKey = Guid.NewGuid().ToString(),

    //        ReadingDateUtc = DateTime.UtcNow.AddMonths(-i),
    //        MeterValue = initialMeterValue - i * 200.0
    //    };

    //    var response = await tableClient.AddEntityAsync(newEntry);
    //}

    var queryResultsFilter = tableClient.Query<GasMeterReading>();

    // Iterate the <see cref="Pageable"> to access all queried entities.
    foreach (var qEntity in queryResultsFilter.OrderBy(reading => reading.ReadingDateUtc))
    {
        Console.WriteLine($"{qEntity.ReadingDateUtc.Date.ToShortDateString()}: {qEntity.MeterValue}");
    }

    Console.WriteLine($"The query returned {queryResultsFilter.Count()} entities.");
}

async Task MakeRequest()
{
    var client = new HttpClient();
    var queryString = HttpUtility.ParseQueryString(string.Empty);
    
    var resourceName = settings.AzureCognitiveServicesResourceName;
    var apiKey = settings.AzureCognitiveServicesApiKey;

    // Request headers
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

    // Request parameters
    //The visual features requested: tags, objects, caption, denseCaptions, read, smartCrops, people. This parameter needs to be specified if the parameter "model-name" is not specified.
    queryString["features"] = "read";
    //queryString["model-name"] = "{string}";
    queryString["language"] = "en";
    //queryString["smartcrops-aspect-ratios"] = "{string}";
    queryString["gender-neutral-caption"] = "False";
    var uri = $"https://{resourceName}.cognitiveservices.azure.com/computervision/imageanalysis:analyze?api-version=2023-02-01-preview&" + queryString;

    var testImagePath1 = @"c:\Users\dledvinka\Downloads\PXL_20230905_165547509_scaled.jpg";
    var testImagePath2 = @"c:\Users\dledvinka\Downloads\Screenshot (152).png";
    var testImagePath3 = @"c:\Users\dledvinka\Downloads\googlelogo_color_92x30dp.png";


    var bytes = await File.ReadAllBytesAsync(testImagePath1);
    
    using var content = new ByteArrayContent(bytes);
    content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
    var response = await client.PostAsync(uri, content);
    var responseContent = await response.Content.ReadAsStringAsync();
    Console.WriteLine(responseContent);

    var dynamicObject = JsonConvert.DeserializeObject<dynamic>(responseContent)!;
    var parsedContent = dynamicObject.readResult.content;

    var gasReadingValue = GetGasReadingValue(parsedContent);

    decimal GetGasReadingValue(string parsedContent)
    {
        var indexOfMeter3 = parsedContent.IndexOf("m\n3");
        var numericPart = parsedContent.Substring(0, indexOfMeter3);
        var withoutWhitespaces = numericPart.Replace(" ", string.Empty);
        var integerPart = withoutWhitespaces.Substring(0, withoutWhitespaces.Length - 3);
        var decimalPart = withoutWhitespaces.Substring(integerPart.Length, 3);
        var decimalNumberAsString = $"{integerPart},{decimalPart}";

        var number = decimal.Parse(decimalNumberAsString);

        return number;
    }

    //dynamic stuff = JsonConvert.DeserializeObject("{ 'Name': 'Jon Smith', 'Address': { 'City': 'New York', 'State': 'NY' }, 'Age': 42 }");
}
