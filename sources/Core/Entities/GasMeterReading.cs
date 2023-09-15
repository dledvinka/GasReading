namespace Core.Entities;

using Azure;
using Azure.Data.Tables;

/// <summary>
/// https://github.com/Azure/azure-sdk-for-net/blob/Azure.Data.Tables_12.8.1/sdk/tables/Azure.Data.Tables/README.md
/// </summary>
public class GasMeterReading : ITableEntity
{
    public ETag ETag { get; set; }

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string Text { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public DateTime ReadingDateUtc { get; set; }
    public double MeterValue { get; set; }
}