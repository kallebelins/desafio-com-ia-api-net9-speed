namespace Lab06.Application.DTOs.Responses;

/// <summary>
/// Response com lista de clientes
/// </summary>
public record ClienteListResponse(
    IList<ClienteResponse> Items,
    int TotalCount
);
