namespace GoodsHierarchy;

// sealed - от планшета наследовать нельзя
public sealed class Tablet : Computer, IPortable, IEquatable<Tablet>
{
    private const double MinScreenSizeInch = 5.0;
    private const int MinBatteryLifeHours = 1;

    private static int finalizedCount;

    private double screenSizeInch;
    private int batteryLifeHours;

    public Tablet(string name, Manufacturer manufacturer, decimal price, double weightKg,
                  int powerWatts, string cpuModel, int cpuCores, double cpuFrequencyGHz,
                  int ramGB, int storageGB, string operatingSystem,
                  double screenSizeInch, int batteryLifeHours, bool hasCellular)
        : base(name, manufacturer, price, weightKg, powerWatts,
               cpuModel, cpuCores, cpuFrequencyGHz, ramGB, storageGB, operatingSystem)
    {
        ScreenSizeInch = screenSizeInch;
        BatteryLifeHours = batteryLifeHours;
        HasCellular = hasCellular;
    }

    // деструктор - это переопределение Object.Finalize()
    ~Tablet()
    {
        Interlocked.Increment(ref finalizedCount);
    }

    public static int FinalizedCount => Volatile.Read(ref finalizedCount);

    public double ScreenSizeInch
    {
        get => screenSizeInch;
        set
        {
            Validation.AtLeast(value, MinScreenSizeInch, nameof(ScreenSizeInch));
            screenSizeInch = value;
        }
    }

    public int BatteryLifeHours
    {
        get => batteryLifeHours;
        set
        {
            Validation.AtLeast(value, MinBatteryLifeHours, nameof(BatteryLifeHours));
            batteryLifeHours = value;
        }
    }

    public bool HasCellular { get; set; }

    public override string Category => "Планшет";

    public override decimal CalculateShippingCost() => BaseShippingCost + FragilePackagingCost;

    public override string PerformMainFunction() =>
        $"работает автономно до {BatteryLifeHours} ч и {base.PerformMainFunction()}";

    // переопределение методов, унаследованных от Object
    public bool Equals(Tablet? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Name == other.Name
            && Manufacturer.Name == other.Manufacturer.Name
            && Price == other.Price
            && WeightKg.Equals(other.WeightKg)
            && Processor.Model == other.Processor.Model
            && RamGB == other.RamGB
            && StorageGB == other.StorageGB
            && ScreenSizeInch.Equals(other.ScreenSizeInch)
            && BatteryLifeHours == other.BatteryLifeHours
            && HasCellular == other.HasCellular;
    }

    public override bool Equals(object? obj) => Equals(obj as Tablet);

    public override int GetHashCode() =>
        HashCode.Combine(Name, Manufacturer.Name, Price, Processor.Model,
                         RamGB, StorageGB, ScreenSizeInch, HasCellular);

    public override string ToString() =>
        base.ToString() +
        $"{Indent}Экран: {Validation.Number(ScreenSizeInch)}\", автономность: {BatteryLifeHours} ч, " +
        $"сотовая связь: {(HasCellular ? "да" : "нет")}";
}
