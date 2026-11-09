namespace GoodsHierarchy;

// Техника - товар с электропитанием
public abstract class Equipment : Product, IPowered
{
    protected const decimal FragilePackagingCost = 500m;
    protected const int EquipmentWarrantyMonths = 24;
    private const int MinPowerWatts = 1;

    private int powerWatts;

    protected Equipment(string name, Manufacturer manufacturer, decimal price,
                        double weightKg, int powerWatts)
        : base(name, manufacturer, price, weightKg)
    {
        PowerWatts = powerWatts;
    }

    public int PowerWatts
    {
        get => powerWatts;
        set
        {
            Validation.AtLeast(value, MinPowerWatts, nameof(PowerWatts));
            powerWatts = value;
        }
    }

    public bool IsOn { get; private set; }

    public override string Category => "Техника";

    public override int WarrantyMonths => EquipmentWarrantyMonths;

    // технику перевозят в защитной упаковке
    public override decimal CalculateShippingCost() =>
        base.CalculateShippingCost() + FragilePackagingCost;

    // основная функция устройства, у каждого класса своя
    public abstract string PerformMainFunction();

    public virtual string TurnOn()
    {
        if (IsOn)
        {
            return $"{Name}: устройство уже включено";
        }

        IsOn = true;
        return $"{Name}: питание включено ({PowerWatts} Вт)";
    }

    public virtual string TurnOff()
    {
        IsOn = false;
        return $"{Name}: питание выключено";
    }

    public override string ToString() =>
        base.ToString() +
        $"{Indent}Мощность: {PowerWatts} Вт, состояние: {(IsOn ? "включено" : "выключено")}";

    protected void EnsureIsOn()
    {
        if (!IsOn)
        {
            throw new InvalidOperationException($"{Name}: устройство выключено.");
        }
    }
}
