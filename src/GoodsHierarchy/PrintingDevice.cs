namespace GoodsHierarchy;

public enum PrintTechnology
{
    Laser,
    Inkjet,
}

public class PrintingDevice : Equipment, IPrinter
{
    private const int MinPagesPerMinute = 1;
    private const int MinCopies = 1;
    private const double SecondsPerMinute = 60.0;

    private int pagesPerMinute;

    public PrintingDevice(string name, Manufacturer manufacturer, decimal price, double weightKg,
                          int powerWatts, PrintTechnology technology, int pagesPerMinute, bool isColor)
        : base(name, manufacturer, price, weightKg, powerWatts)
    {
        Technology = technology;
        PagesPerMinute = pagesPerMinute;
        IsColor = isColor;
    }

    public PrintTechnology Technology { get; set; }

    public int PagesPerMinute
    {
        get => pagesPerMinute;
        set
        {
            Validation.AtLeast(value, MinPagesPerMinute, nameof(PagesPerMinute));
            pagesPerMinute = value;
        }
    }

    public bool IsColor { get; set; }

    public override string Category => "Печатающее устройство";

    public override string PerformMainFunction() =>
        $"печатает {PagesPerMinute} стр./мин, {TechnologyName(Technology)} " +
        $"{(IsColor ? "цветная" : "монохромная")} печать";

    // время печати округляется вверх до целых секунд
    public string Print(string document, int copies)
    {
        Validation.NotEmpty(document, nameof(document));
        Validation.AtLeast(copies, MinCopies, nameof(copies));
        EnsureIsOn();

        int seconds = (int)Math.Ceiling(copies * SecondsPerMinute / PagesPerMinute);
        return $"{Name}: «{document}» напечатан, {copies} экз. за {seconds} с";
    }

    public override string ToString() =>
        base.ToString() +
        $"{Indent}Технология: {TechnologyName(Technology)}, скорость: {PagesPerMinute} стр./мин, " +
        $"цветная печать: {(IsColor ? "да" : "нет")}";

    private static string TechnologyName(PrintTechnology technology) => technology switch
    {
        PrintTechnology.Laser => "лазерная",
        PrintTechnology.Inkjet => "струйная",
        _ => technology.ToString(),
    };
}
