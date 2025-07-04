using AestusDemoAPI.EndpointHandlers;

namespace AestusDemoAPI.Extensions
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void RegisterEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/transactions", TransactionHandler.GetTransactionsAsync);
            endpoints.MapPost("/transactions", TransactionHandler.PostTransactionAsync);
            endpoints.MapGet("/transactions/{id}/anomalies", TransactionHandler.GetAnomaliesAsync);
        }
    }
}
