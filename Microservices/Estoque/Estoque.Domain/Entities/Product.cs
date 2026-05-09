namespace Estoque.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string TechnicalDescription { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public string Location { get; private set; } = string.Empty;

    private Product() { }

    public Product(string name, string technicalDescription, int initialQuantity, string location)
    {
        Id = Guid.NewGuid();
        Update(name, technicalDescription, location);
        AddStock(initialQuantity);
    }

    public void Update(string name, string technicalDescription, string location)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome é obrigatório.");
        Name = name.Trim();
        TechnicalDescription = technicalDescription?.Trim() ?? string.Empty;
        Location = location?.Trim() ?? string.Empty;
    }

    public void AddStock(int quantity)
    {
        if (quantity < 0) throw new ArgumentException("Quantidade não pode ser negativa.");
        Quantity += quantity;
    }

    public void RemoveStock(int quantity)
    {
        if (quantity < 0) throw new ArgumentException("Quantidade não pode ser negativa.");
        if (Quantity < quantity) throw new InvalidOperationException("Estoque insuficiente.");
        Quantity -= quantity;
    }
}
