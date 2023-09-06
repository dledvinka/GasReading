// See https://aka.ms/new-console-template for more information

// based on: https://centraluseuap.dev.cognitive.microsoft.com/docs/services/unified-vision-apis-public-preview-2023-02-01-preview/operations/61d65934cd35050c20f73ab6
// test app: https://centraluseuap.dev.cognitive.microsoft.com/docs/services/unified-vision-apis-public-preview-2023-02-01-preview/operations/61d65934cd35050c20f73ab6/console



using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Drawing.Imaging;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

Console.WriteLine("Hello, World!");

await MakeRequest();
Console.WriteLine("Hit ENTER to exit...");
Console.ReadLine();

static async Task MakeRequest()
{
    var client = new HttpClient();
    var queryString = HttpUtility.ParseQueryString(string.Empty);
    
    var resourceName = "xxx";
    var apiKey = "xxx";

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
