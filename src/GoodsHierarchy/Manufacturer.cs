namespace GoodsHierarchy;

// один производитель может быть у нескольких товаров (агрегация)
public class Manufacturer
{
    public Manufacturer(string name, string country)
    {
        Validation.NotEmpty(name, nameof(name));
        Validation.NotEmpty(country, nameof(country));
        Name = name;
        Country = country;
    }

    public string Name { get; }

    public string Country { get; }

    public override string ToString() => $"{Name} ({Country})";
}
