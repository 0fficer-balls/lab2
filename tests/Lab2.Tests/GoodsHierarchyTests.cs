using System.Runtime.CompilerServices;
using GoodsHierarchy;
using Lab2.Tests.TestFramework;

namespace Lab2.Tests;

// тесты иерархии товаров: наследование, полиморфизм, интерфейсы
public class GoodsHierarchyTests
{
    private readonly Manufacturer hp = new("HP", "США");
    private readonly Manufacturer epson = new("Epson", "Япония");
    private readonly Manufacturer dell = new("Dell", "США");
    private readonly Manufacturer samsung = new("Samsung", "Южная Корея");

    [Test]
    public void Category_IsOverriddenInEveryClass()
    {
        Product[] goods = CreateCatalog();

        Assert.Equal("Печатающее устройство", goods[0].Category);
        Assert.Equal("Сканер", goods[1].Category);
        Assert.Equal("Компьютер", goods[2].Category);
        Assert.Equal("Планшет", goods[3].Category);
    }

    [Test]
    public void WarrantyMonths_DependsOnActualType()
    {
        Product[] goods = CreateCatalog();

        Assert.Equal(24, goods[0].WarrantyMonths);
        Assert.Equal(24, goods[1].WarrantyMonths);
        Assert.Equal(36, goods[2].WarrantyMonths);
        Assert.Equal(36, goods[3].WarrantyMonths);
    }

    [Test]
    public void CalculateShippingCost_CallsOverriddenVersion()
    {
        Product[] goods = CreateCatalog();

        Assert.Equal(1275.00m, goods[0].CalculateShippingCost());
        Assert.Equal(875.00m, goods[1].CalculateShippingCost());
        Assert.Equal(1060.00m, goods[2].CalculateShippingCost());
        Assert.Equal(800m, goods[3].CalculateShippingCost());
    }

    [Test]
    public void PerformMainFunction_ThroughBaseReference_UsesDerivedImplementation()
    {
        Equipment printer = CreatePrinter();
        Equipment tablet = CreateTablet(128);

        Assert.Contains("38 стр./мин", printer.PerformMainFunction());
        Assert.Contains("автономно до 15 ч", tablet.PerformMainFunction());
        Assert.Contains("8 ядер", tablet.PerformMainFunction());
    }

    [Test]
    public void TurnOn_Computer_AlsoLoadsOperatingSystem()
    {
        IPowered computer = CreateComputer();

        string message = computer.TurnOn();

        Assert.True(computer.IsOn);
        Assert.Contains("загружена ОС Windows 11 Pro", message);
    }

    [Test]
    public void TurnOff_SwitchesPowerOff()
    {
        IPowered scanner = CreateScanner();
        scanner.TurnOn();
        scanner.TurnOff();

        Assert.False(scanner.IsOn);
    }

    [Test]
    public void Print_DeviceIsOff_ThrowsInvalidOperationException()
    {
        PrintingDevice printer = CreatePrinter();

        Assert.Throws<InvalidOperationException>(() => printer.Print("Отчёт.docx", 1));
    }

    [Test]
    public void Print_DeviceIsOn_ReturnsPrintingTime()
    {
        PrintingDevice printer = CreatePrinter();
        printer.TurnOn();

        Assert.Contains("10 экз. за 16 с", printer.Print("Отчёт.docx", 10));
    }

    [Test]
    public void Scan_DeviceIsOn_ReturnsResult()
    {
        Scanner scanner = CreateScanner();
        scanner.TurnOn();

        Assert.Contains("отсканирован (4800 dpi)", scanner.Scan("Паспорт.pdf"));
    }

    [Test]
    public void Interfaces_AreImplementedByExpectedClasses()
    {
        Assert.True(CreatePrinter() is IPrinter and IPowered);
        Assert.True(CreateScanner() is IScanner and IPowered);
        Assert.False(CreateComputer() is IPortable);
        Assert.True(CreateTablet(128) is IPortable and IPowered);
    }

    [Test]
    public void Computer_CreatesItsProcessor_Composition()
    {
        Computer computer = CreateComputer();

        Assert.Equal("Intel Core i5-13500", computer.Processor.Model);
        Assert.Equal(14, computer.Processor.Cores);
        Assert.NotSame(computer.Processor, CreateComputer().Processor);
    }

    [Test]
    public void Manufacturer_CanBeSharedBetweenProducts_Aggregation()
    {
        Product[] goods = CreateCatalog();

        Assert.True(ReferenceEquals(goods[0].Manufacturer, hp));
        Assert.True(ReferenceEquals(goods[1].Manufacturer, epson));
    }

    [Test]
    public void Tablet_IsSealed_AndBaseClassesAreAbstract()
    {
        Assert.True(typeof(Tablet).IsSealed);
        Assert.False(typeof(Computer).IsSealed);
        Assert.True(typeof(Product).IsAbstract);
        Assert.True(typeof(Equipment).IsAbstract);
    }

    [Test]
    public void Tablet_Equals_SameCharacteristics_IsTrue()
    {
        Tablet first = CreateTablet(128);
        Tablet second = CreateTablet(128);

        Assert.True(first.Equals(second));
        Assert.True(first.Equals((object)second));
        Assert.False(ReferenceEquals(first, second));
    }

    [Test]
    public void Tablet_Equals_DifferentObjects_IsFalse()
    {
        Tablet tablet = CreateTablet(128);

        Tablet? nothing = null;
        object text = "Galaxy Tab S9";

        Assert.False(tablet.Equals(CreateTablet(256)));
        Assert.False(tablet.Equals(nothing));
        Assert.False(tablet.Equals(text));
    }

    [Test]
    public void Tablet_GetHashCode_EqualObjects_HaveEqualHashCodes()
    {
        Assert.Equal(CreateTablet(128).GetHashCode(), CreateTablet(128).GetHashCode());
    }

    [Test]
    public void Tablet_Finalize_IsCalledByGarbageCollector()
    {
        int before = Tablet.FinalizedCount;

        CreateUnreachableTablets(5);
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Assert.True(Tablet.FinalizedCount - before >= 5, "деструктор ~Tablet должен быть вызван");
    }

    [Test]
    public void ToString_ContainsTypeAndCurrentValues()
    {
        foreach (Product product in CreateCatalog())
        {
            string text = product.ToString();

            Assert.Contains($"Тип: {product.Category} ({product.GetType().Name})", text);
            Assert.Contains(product.Name, text);
            Assert.Contains(product.Manufacturer.Name, text);
        }

        Assert.Contains("Экран: 11\"", CreateTablet(128).ToString());
        Assert.Contains("ОЗУ: 16 ГБ", CreateComputer().ToString());
    }

    [Test]
    public void ToString_ReflectsChangedValues()
    {
        Computer computer = CreateComputer();
        computer.RamGB = 32;
        computer.TurnOn();

        Assert.Contains("ОЗУ: 32 ГБ", computer.ToString());
        Assert.Contains("состояние: включено", computer.ToString());
    }

    [TestCase(0)]
    [TestCase(-100)]
    public void Price_NotPositive_ThrowsAndKeepsPreviousValue(int newPrice)
    {
        Product scanner = CreateScanner();

        Assert.Throws<ArgumentOutOfRangeException>(() => scanner.Price = newPrice);
        Assert.Equal(12490m, scanner.Price);
    }

    [Test]
    public void Name_Empty_ThrowsArgumentException()
    {
        Product printer = CreatePrinter();

        Assert.Throws<ArgumentException>(() => printer.Name = "  ");
    }

    [Test]
    public void ArraySort_OrdersProductsByPrice()
    {
        Product[] goods = CreateCatalog();
        Array.Sort(goods);

        Assert.IsType<Scanner>(goods[0]);
        Assert.IsType<PrintingDevice>(goods[1]);
        Assert.IsType<Tablet>(goods[2]);
        Assert.IsType<Computer>(goods[3]);
    }

    // тестовые данные
    private Product[] CreateCatalog() =>
        [CreatePrinter(), CreateScanner(), CreateComputer(), CreateTablet(128)];

    private PrintingDevice CreatePrinter() =>
        new("LaserJet Pro M404dn", hp, 32990m, 9.5, 550, PrintTechnology.Laser, 38, false);

    private Scanner CreateScanner() =>
        new("Perfection V39 II", epson, 12490m, 1.5, 3, 4800, PaperFormat.A4);

    private Computer CreateComputer() =>
        new("OptiPlex 7010", dell, 74990m, 5.2, 180,
            "Intel Core i5-13500", 14, 2.5, 16, 512, "Windows 11 Pro");

    private Tablet CreateTablet(int storageGB) =>
        new("Galaxy Tab S9", samsung, 69990m, 0.5, 45,
            "Snapdragon 8 Gen 2", 8, 3.36, 8, storageGB, "Android 14", 11.0, 15, false);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void CreateUnreachableTablets(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _ = CreateTablet(64);
        }
    }
}
