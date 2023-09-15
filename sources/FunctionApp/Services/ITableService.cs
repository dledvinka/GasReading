namespace FunctionApp.Services;

using FunctionApp.Entities;

internal interface ITableService
{
    Task<List<GasMeterReading>> GetAll();
}