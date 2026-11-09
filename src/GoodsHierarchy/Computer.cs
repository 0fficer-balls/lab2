namespace GoodsHierarchy;

public class Computer : Equipment
{
    protected const int ComputerWarrantyMonths = 36;
    private const int MinRamGB = 1;
    private const int MinStorageGB = 1;

    private int ramGB;
    private int storageGB;
    private string operatingSystem = string.Empty;

    public Computer(string name, Manufacturer manufacturer, decimal price, double weightKg,
                    int powerWatts, string cpuModel, int cpuCores, double cpuFrequencyGHz,
                    int ramGB, int storageGB, string operatingSystem)
        : base(name, manufacturer, price, weightKg, powerWatts)
    {
        // композиция: процессор создаётся вместе с компьютером
        Processor = new Processor(cpuModel, cpuCores, cpuFrequencyGHz);
        RamGB = ramGB;
        StorageGB = storageGB;
        OperatingSystem = operatingSystem;
    }

    public Processor Processor { get; }

    public int RamGB
    {
        get => ramGB;
        set
        {
            Validation.AtLeast(value, MinRamGB, nameof(RamGB));
            ramGB = value;
        }
    }

    public int StorageGB
    {
        get => storageGB;
        set
        {
            Validation.AtLeast(value, MinStorageGB, nameof(StorageGB));
            storageGB = value;
        }
    }

    public string OperatingSystem
    {
        get => operatingSystem;
        set
        {
            Validation.NotEmpty(value, nameof(OperatingSystem));
            operatingSystem = value;
        }
    }

    public override string Category => "Компьютер";

    public override int WarrantyMonths => ComputerWarrantyMonths;

    public override string PerformMainFunction() =>
        $"выполняет программы: {Processor.Cores} ядер × {Validation.Number(Processor.FrequencyGHz)} ГГц, ОЗУ {RamGB} ГБ";

    // при включении компьютера дополнительно загружается ОС
    public override string TurnOn()
    {
        bool wasOn = IsOn;
        string message = base.TurnOn();
        return wasOn ? message : $"{message}, загружена ОС {OperatingSystem}";
    }

    public override string ToString() =>
        base.ToString() +
        $"{Indent}Процессор: {Processor}" +
        $"{Indent}ОЗУ: {RamGB} ГБ, накопитель: {StorageGB} ГБ, ОС: {OperatingSystem}";
}
