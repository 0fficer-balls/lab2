namespace GoodsHierarchy;

public enum PaperFormat
{
    A4,
    A3,
}

public class Scanner : Equipment, IScanner
{
    private const int MinResolutionDpi = 75;

    private int resolutionDpi;

    public Scanner(string name, Manufacturer manufacturer, decimal price, double weightKg,
                   int powerWatts, int resolutionDpi, PaperFormat maxFormat)
        : base(name, manufacturer, price, weightKg, powerWatts)
    {
        ResolutionDpi = resolutionDpi;
        MaxFormat = maxFormat;
    }

    public int ResolutionDpi
    {
        get => resolutionDpi;
        set
        {
            Validation.AtLeast(value, MinResolutionDpi, nameof(ResolutionDpi));
            resolutionDpi = value;
        }
    }

    public PaperFormat MaxFormat { get; set; }

    public override string Category => "Сканер";

    public override string PerformMainFunction() =>
        $"сканирует оригиналы до формата {MaxFormat} с разрешением {ResolutionDpi} dpi";

    public string Scan(string document)
    {
        Validation.NotEmpty(document, nameof(document));
        EnsureIsOn();
        return $"{Name}: «{document}» отсканирован ({ResolutionDpi} dpi)";
    }

    public override string ToString() =>
        base.ToString() +
        $"{Indent}Разрешение: {ResolutionDpi} dpi, максимальный формат: {MaxFormat}";
}
