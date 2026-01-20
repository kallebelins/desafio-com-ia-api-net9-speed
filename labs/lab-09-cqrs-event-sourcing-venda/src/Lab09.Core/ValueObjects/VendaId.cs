namespace Lab09.Core.ValueObjects;

/// <summary>
/// Value Object para identificador de Venda (Strongly-Typed ID)
/// </summary>
public readonly record struct VendaId
{
    public Guid Value { get; }

    public VendaId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("VendaId não pode ser vazio", nameof(value));
        Value = value;
    }

    public static VendaId New() => new(Guid.NewGuid());
    
    public static VendaId From(Guid value) => new(value);
    
    public static implicit operator Guid(VendaId id) => id.Value;
    
    public static explicit operator VendaId(Guid value) => new(value);
    
    public override string ToString() => Value.ToString();
}
