using Lab08.Application.DTOs;

namespace Lab08.Application.UseCases.Clientes.CreateCliente;

/// <summary>
/// Output da criação de cliente
/// </summary>
public record CreateClienteOutput
{
    public bool Success { get; init; }
    public ClienteDto? Cliente { get; init; }
    public string? ErrorMessage { get; init; }

    public static CreateClienteOutput Ok(ClienteDto cliente)
        => new() { Success = true, Cliente = cliente };

    public static CreateClienteOutput Error(string message)
        => new() { Success = false, ErrorMessage = message };
}
