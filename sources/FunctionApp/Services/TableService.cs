using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace FunctionApp.Services;

using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;

/// <summary>
/// https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-table?tabs=in-process%2Ctable-api%2Cextensionv3&amp;pivots=programming-language-csharp
/// </summary>
internal class TableService : ITableService
{
    private readonly ISettingsService _settingsService;
    private readonly CloudTable _table;
    private const string _tableName = "HydroNotifier";
    private const string _partitionKey = "Data";

    public TableService(ISettingsService settingsService)
    {
        _settingsService = settingsService;

        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_settingsService.TableStorageConnectionString);
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
        _table = tableClient.GetTableReference(_tableName);
    }

    public async Task<FlowDataEntity> InsertOrMergeAsync(FlowDataEntity entity)
    {
        if (string.IsNullOrWhiteSpace(entity.RowKey))
        {
            var invertedTimeKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
            entity.RowKey = invertedTimeKey;
        }

        entity.PartitionKey = _partitionKey;

        TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
        TableResult result = await _table.ExecuteAsync(insertOrMergeOperation);

        return result.Result as FlowDataEntity;
    }

    public List<FlowDataEntity> GetAll()
    {
        var tableServiseEndpoint = @"https://gasreadingappstorage.table.core.windows.net/";
        var serviceClient = new TableServiceClient(new Uri(tableServiseEndpoint), 
                                                   new TableSharedKeyCredential("gasreadingappstorage", "84RiPJyG3Fs5uXMWFwJcmJ1lzJcckjHKK1uQDMQ+EQNno/rw/hX2f2jvrka+tg3jzhSSSRZajem0+AStbYRZFg=="));

        TableItem table = serviceClient.CreateTableIfNotExists("Test");

        // Construct the query operation for all customer entities where PartitionKey="Smith".
        TableQuery<FlowDataEntity> query = new TableQuery<FlowDataEntity>()
            .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, _partitionKey));

        return _table.ExecuteQuery(query).ToList();
    }

    public FlowDataEntity GetLastOrDefault()
    {
        // Construct the query operation for all customer entities where PartitionKey="Smith".
        TableQuery<FlowDataEntity> query = new TableQuery<FlowDataEntity>()
            .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, _partitionKey))
            //.OrderByDesc("Timestamp") // works for Cosmos DB only
            .Take(1);

        var result = _table.ExecuteQuery(query).ToList();

        if (!result.Any())
        {
            return null;
        }

        return result[0];
    }
}
