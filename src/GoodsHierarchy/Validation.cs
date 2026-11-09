using System.Globalization;

namespace GoodsHierarchy;

internal static class Validation
{
    public static void NotEmpty(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Значение {name} не может быть пустым.", name);
        }
    }

    public static void AtLeast(int value, int minValue, string name)
    {
        if (value < minValue)
        {
            throw new ArgumentOutOfRangeException(name, value,
                $"Значение {name} должно быть не меньше {minValue}.");
        }
    }

    public static void AtLeast(double value, double minValue, string name)
    {
        if (value < minValue)
        {
            throw new ArgumentOutOfRangeException(name, value,
                $"Значение {name} должно быть не меньше {minValue}.");
        }
    }

    public static void Positive(decimal value, string name)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(name, value,
                $"Значение {name} должно быть больше нуля.");
        }
    }

    public static string Money(decimal value) =>
        value.ToString("0.00", CultureInfo.InvariantCulture) + " руб.";

    public static string Number(double value) =>
        value.ToString("0.##", CultureInfo.InvariantCulture);
}
