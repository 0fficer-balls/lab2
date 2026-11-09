namespace GoodsHierarchy;

// Товар - абстрактный базовый класс иерархии
public abstract class Product : IComparable<Product>
{
    protected const decimal BaseShippingCost = 300m;
    protected const decimal ShippingCostPerKg = 50m;
    protected const int DefaultWarrantyMonths = 12;
    protected const double MinWeightKg = 0.01;

    protected static readonly string Indent = Environment.NewLine + "  ";

    private string name = string.Empty;
    private decimal price;
    private double weightKg;

    protected Product(string name, Manufacturer manufacturer, decimal price, double weightKg)
    {
        ArgumentNullException.ThrowIfNull(manufacturer);
        Name = name;
        Manufacturer = manufacturer;
        Price = price;
        WeightKg = weightKg;
    }

    public string Name
    {
        get => name;
        set
        {
            Validation.NotEmpty(value, nameof(Name));
            name = value;
        }
    }

    public Manufacturer Manufacturer { get; }

    public decimal Price
    {
        get => price;
        set
        {
            Validation.Positive(value, nameof(Price));
            price = value;
        }
    }

    public double WeightKg
    {
        get => weightKg;
        set
        {
            Validation.AtLeast(value, MinWeightKg, nameof(WeightKg));
            weightKg = value;
        }
    }

    // каждый потомок задаёт свою категорию
    public abstract string Category { get; }

    public virtual int WarrantyMonths => DefaultWarrantyMonths;

    // доставка: базовая ставка + плата за каждый килограмм
    public virtual decimal CalculateShippingCost() =>
        BaseShippingCost + ShippingCostPerKg * (decimal)WeightKg;

    public int CompareTo(Product? other) => other is null ? 1 : Price.CompareTo(other.Price);

    public override string ToString() =>
        $"Тип: {Category} ({GetType().Name})" +
        $"{Indent}Название: {Name}" +
        $"{Indent}Производитель: {Manufacturer}" +
        $"{Indent}Цена: {Validation.Money(Price)}" +
        $"{Indent}Масса: {Validation.Number(WeightKg)} кг, гарантия: {WarrantyMonths} мес.";
}
