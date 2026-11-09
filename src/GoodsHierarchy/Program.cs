using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace GoodsHierarchy;

internal static class Program
{
    private const string TestDocument = "Отчёт.docx";
    private const int TestCopies = 10;

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        Product[] goods = CreateCatalog();

        PrintHeader("1. Массив разнотипных объектов: вызов ToString()");
        foreach (Product product in goods)
        {
            // вызывается ToString() фактического типа объекта
            Console.WriteLine(product);
            Console.WriteLine();
        }

        PrintHeader("2. Виртуальные члены Category, WarrantyMonths, CalculateShippingCost()");
        foreach (Product product in goods)
        {
            Console.WriteLine($"{product.Name,-20} | {product.Category,-21} | гарантия {product.WarrantyMonths,2} мес." +
                              $" | доставка {product.CalculateShippingCost(),7:0.00} руб.");
        }

        PrintHeader("3. Интерфейс IPowered и абстрактный метод PerformMainFunction()");
        foreach (Product product in goods)
        {
            if (product is Equipment equipment)
            {
                Console.WriteLine(equipment.TurnOn());
                Console.WriteLine($"  -> {equipment.PerformMainFunction()}");
            }
        }

        PrintHeader("4. Работа через интерфейсы IPrinter, IScanner, IPortable");
        foreach (Product product in goods)
        {
            switch (product)
            {
                case IPrinter printer:
                    Console.WriteLine(printer.Print(TestDocument, TestCopies));
                    break;
                case IScanner scanner:
                    Console.WriteLine(scanner.Scan(TestDocument));
                    break;
                case IPortable portable:
                    Console.WriteLine($"{product.Name}: переносное устройство, экран " +
                                      $"{portable.ScreenSizeInch}\", до {portable.BatteryLifeHours} ч без подзарядки");
                    break;
                default:
                    Console.WriteLine($"{product.Name}: не печатает, не сканирует и не переносное");
                    break;
            }
        }

        PrintHeader("5. Обработка ошибок");
        var firstPrinter = (PrintingDevice)goods[0];
        Console.WriteLine(firstPrinter.TurnOff());
        TryRun(() => firstPrinter.Print(TestDocument, TestCopies));
        TryRun(() => goods[2].Price = -100m);
        TryRun(() => new Scanner("Mini", goods[2].Manufacturer, 1000m, 0.5, 2, 50, PaperFormat.A4));

        PrintHeader("6. Сортировка массива по цене (IComparable<Product>)");
        Array.Sort(goods);
        foreach (Product product in goods)
        {
            Console.WriteLine($"{product.Price,10:0.00} руб. - {product.Name} ({product.GetType().Name})");
        }

        DemonstrateObjectMethods();

        PrintHeader("8. Бесплодный (sealed) класс и абстрактные классы");
        foreach (Type type in new[] { typeof(Product), typeof(Equipment), typeof(Computer), typeof(Tablet) })
        {
            Console.WriteLine($"{type.Name,-10} abstract: {type.IsAbstract,-5}  sealed: {type.IsSealed,-5}  " +
                              $"базовый класс: {type.BaseType?.Name}");
        }

        Console.WriteLine("Объявление class MiniTablet : Tablet { } вызывает ошибку компиляции CS0509.");
    }

    public static Product[] CreateCatalog()
    {
        var hp = new Manufacturer("HP", "США");
        var epson = new Manufacturer("Epson", "Япония");
        var dell = new Manufacturer("Dell", "США");
        var samsung = new Manufacturer("Samsung", "Южная Корея");

        return
        [
            new PrintingDevice("LaserJet Pro M404dn", hp, 32990m, 9.5, 550,
                               PrintTechnology.Laser, 38, isColor: false),
            new PrintingDevice("EcoTank L8050", epson, 41990m, 5.0, 15,
                               PrintTechnology.Inkjet, 22, isColor: true),
            new Scanner("Perfection V39 II", epson, 12490m, 1.5, 3, 4800, PaperFormat.A4),
            new Computer("OptiPlex 7010", dell, 74990m, 5.2, 180,
                         "Intel Core i5-13500", 14, 2.5, 16, 512, "Windows 11 Pro"),
            new Tablet("Galaxy Tab S9", samsung, 69990m, 0.5, 45,
                       "Snapdragon 8 Gen 2", 8, 3.36, 8, 128, "Android 14",
                       11.0, 15, hasCellular: false),
        ];
    }

    private static void DemonstrateObjectMethods()
    {
        PrintHeader("7. Методы System.Object, переопределённые в классе Tablet");
        var samsung = new Manufacturer("Samsung", "Южная Корея");
        Tablet first = CreateTablet(samsung, 128);
        Tablet second = CreateTablet(samsung, 128);
        Tablet other = CreateTablet(samsung, 256);

        Console.WriteLine($"first.Equals(second): {first.Equals(second)}   first.Equals(other): {first.Equals(other)}");
        Console.WriteLine($"first == second (сравнение ссылок): {first == second}");
        Console.WriteLine($"GetHashCode: first = {first.GetHashCode()}, second = {second.GetHashCode()}");
        Console.WriteLine($"first.Equals(\"строка\"): {first.Equals("строка")}");
        Console.WriteLine($"other.ToString(): {other.ToString().Split(Environment.NewLine)[0]} ...");

        int before = Tablet.FinalizedCount;
        CreateGarbageTablets(samsung, 3);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine($"Finalize (~Tablet): сборщик мусора уничтожил {Tablet.FinalizedCount - before} " +
                          "временных объекта");
    }

    private static Tablet CreateTablet(Manufacturer manufacturer, int storageGB) =>
        new("Galaxy Tab S9", manufacturer, 69990m, 0.5, 45,
            "Snapdragon 8 Gen 2", 8, 3.36, 8, storageGB, "Android 14", 11.0, 15, false);

    // отдельный метод, чтобы созданные объекты сразу стали недоступны
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CreateGarbageTablets(Manufacturer manufacturer, int count)
    {
        for (int i = 0; i < count; i++)
        {
            _ = CreateTablet(manufacturer, 64);
        }
    }

    private static void TryRun(Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            Console.WriteLine($"Исключение {exception.GetType().Name}: {exception.Message.Split(" (Parameter")[0]}");
        }
    }

    private static void PrintHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"=== {title} ===");
    }
}
