using System.Diagnostics;

namespace Lab05.Infrastructure.Observability;

/// <summary>
/// ActivitySource customizado para tracing da aplicação
/// </summary>
public static class CustomActivitySource
{
    public const string SourceName = "Lab05.CQRS.Produtos";
    public const string ServiceVersion = "1.0.0";

    public static readonly ActivitySource Source = new(SourceName, ServiceVersion);

    /// <summary>
    /// Inicia uma nova Activity para uma operação
    /// </summary>
    public static Activity? StartActivity(string operationName, ActivityKind kind = ActivityKind.Internal)
    {
        return Source.StartActivity(operationName, kind);
    }

    /// <summary>
    /// Inicia uma Activity para operação de banco de dados
    /// </summary>
    public static Activity? StartDatabaseActivity(string operation, string? table = null)
    {
        var activity = Source.StartActivity($"DB: {operation}", ActivityKind.Client);
        
        if (activity != null)
        {
            activity.SetTag("db.system", "sqlserver");
            activity.SetTag("db.operation", operation);
            
            if (table != null)
            {
                activity.SetTag("db.table", table);
            }
        }

        return activity;
    }

    /// <summary>
    /// Inicia uma Activity para operação CQRS
    /// </summary>
    public static Activity? StartCqrsActivity(string requestType, string requestName)
    {
        var activity = Source.StartActivity($"CQRS: {requestType} - {requestName}", ActivityKind.Internal);

        if (activity != null)
        {
            activity.SetTag("cqrs.request_type", requestType);
            activity.SetTag("cqrs.request_name", requestName);
        }

        return activity;
    }
}
