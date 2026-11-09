using System.Text;

namespace ArrayOperations;

internal enum MenuCommand
{
    Exit = 0,
    Demo = 1,
    Input = 2,
}

internal static class Program
{
    private const char ElementSeparator = ' ';

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== Часть 1. Одномерный массив IntArray ===");
            Console.WriteLine($"{(int)MenuCommand.Demo} - демонстрация на тестовых данных");
            Console.WriteLine($"{(int)MenuCommand.Input} - ввести два массива с клавиатуры");
            Console.WriteLine($"{(int)MenuCommand.Exit} - выход");
            Console.Write("Ваш выбор: ");

            string? line = Console.ReadLine();
            if (line is null)
            {
                return;
            }

            if (!int.TryParse(line, out int choice) || !Enum.IsDefined(typeof(MenuCommand), choice))
            {
                Console.WriteLine("Ошибка: такого пункта меню нет.");
                continue;
            }

            switch ((MenuCommand)choice)
            {
                case MenuCommand.Demo:
                    RunDemo();
                    break;
                case MenuCommand.Input:
                    try
                    {
                        RunInput();
                    }
                    catch (EndOfStreamException)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Ввод прерван.");
                        return;
                    }

                    break;
                case MenuCommand.Exit:
                    Console.WriteLine("Работа завершена.");
                    return;
            }
        }
    }

    private static void RunDemo()
    {
        var a = new IntArray(new[] { 1, 2, 3, 4 });
        var b = new IntArray(new[] { 2, 0, -1, 5 });
        var c = new IntArray(new[] { 1, 2, 3, 4 });
        var d = new IntArray(new[] { 1, 2, 4 });
        var e = new IntArray(new[] { 1, 2 });
        var f = new IntArray(new[] { -3, 7, -1, 0, 8 });

        Console.WriteLine();
        Console.WriteLine("=== Демонстрация на тестовых данных ===");
        Console.WriteLine($"a = {a}   b = {b}   c = {c}");
        Console.WriteLine($"d = {d}   e = {e}   f = {f}");

        Console.WriteLine("--- Конструкторы и индексатор ---");
        Console.WriteLine($"new IntArray(3) = {new IntArray(3)}");
        Console.WriteLine($"a[0] = {a[0]}, a[3] = {a[3]}");
        var copy = new IntArray(a);
        copy[1] = 10;
        Console.WriteLine($"copy = new IntArray(a); copy[1] = 10 -> copy = {copy}, a = {a}");
        TryShow("a[4]", () => a[4].ToString());

        Console.WriteLine("--- Операция * (поэлементное умножение) ---");
        Console.WriteLine($"a * b = {a * b}");
        TryShow("a * d", () => (a * d).ToString());

        Console.WriteLine("--- Операции true / false ---");
        Console.WriteLine($"a: {DescribeTruth(a)}");
        Console.WriteLine($"b: {DescribeTruth(b)}");

        Console.WriteLine("--- Явное приведение (int) ---");
        Console.WriteLine($"(int)a = {(int)a}, (int)d = {(int)d}");

        Console.WriteLine("--- Операции == и != ---");
        Console.WriteLine($"a == c: {a == c}   a == b: {a == b}   a != b: {a != b}");
        Console.WriteLine($"ReferenceEquals(a, c): {ReferenceEquals(a, c)} (разные объекты, равные значения)");

        Console.WriteLine("--- Операции < и > (лексикографическое сравнение) ---");
        Console.WriteLine($"a < d: {a < d}   d > a: {d > a}   b > a: {b > a}");
        Console.WriteLine($"a < c: {a < c}   a > c: {a > c}   e < a: {e < a}");

        Console.WriteLine("--- Методы расширения ---");
        const string sampleText = "Перегрузка операций";
        Console.WriteLine($"\"{sampleText}\".ContainsChar('з'): {sampleText.ContainsChar('з')}");
        Console.WriteLine($"\"{sampleText}\".ContainsChar('ы'): {sampleText.ContainsChar('ы')}");
        Console.WriteLine($"b.RemoveNegatives() = {b.RemoveNegatives()}, b = {b}");
        Console.WriteLine($"f.RemoveNegatives() = {f.RemoveNegatives()}");
    }

    private static void RunInput()
    {
        IntArray first = ReadArray("A");
        IntArray second = ReadArray("B");

        Console.WriteLine();
        Console.WriteLine($"A = {first}   B = {second}");
        TryShow("A * B", () => (first * second).ToString());
        Console.WriteLine($"A: {DescribeTruth(first)}");
        Console.WriteLine($"B: {DescribeTruth(second)}");
        Console.WriteLine($"(int)A = {(int)first}, (int)B = {(int)second}");
        Console.WriteLine($"A == B: {first == second}   A != B: {first != second}");
        Console.WriteLine($"A < B: {first < second}   A > B: {first > second}");
        Console.WriteLine($"A.RemoveNegatives() = {first.RemoveNegatives()}");
        Console.WriteLine($"B.RemoveNegatives() = {second.RemoveNegatives()}");

        Console.Write("Введите строку: ");
        string text = ReadLineOrThrow();
        char symbol = ReadChar("Введите символ для поиска: ");
        Console.WriteLine($"Строка содержит символ '{symbol}': {text.ContainsChar(symbol)}");
    }

    private static string DescribeTruth(IntArray array)
    {
        // здесь срабатывает перегруженная операция true
        if (array)
        {
            return "true - отрицательных элементов нет";
        }

        return "false - есть отрицательные элементы";
    }

    private static void TryShow(string expression, Func<string> evaluate)
    {
        try
        {
            Console.WriteLine($"{expression} = {evaluate()}");
        }
        catch (Exception exception) when (exception is ArgumentException or IndexOutOfRangeException)
        {
            Console.WriteLine($"{expression}: исключение {exception.GetType().Name}: {exception.Message}");
        }
    }

    // ввод повторяется, пока не будут введены только целые числа
    private static IntArray ReadArray(string name)
    {
        while (true)
        {
            Console.Write($"Введите элементы массива {name} через пробел: ");
            string line = ReadLineOrThrow();
            string[] parts = line.Split(ElementSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                Console.WriteLine("Ошибка: нужно ввести хотя бы одно число.");
                continue;
            }

            var values = new int[parts.Length];
            bool isValid = true;
            for (int i = 0; i < parts.Length && isValid; i++)
            {
                isValid = int.TryParse(parts[i], out values[i]);
            }

            if (isValid)
            {
                return new IntArray(values);
            }

            Console.WriteLine("Ошибка: допускаются только целые числа.");
        }
    }

    private static char ReadChar(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string line = ReadLineOrThrow();
            if (line.Length == 1)
            {
                return line[0];
            }

            Console.WriteLine("Ошибка: введите ровно один символ.");
        }
    }

    private static string ReadLineOrThrow() =>
        Console.ReadLine() ?? throw new EndOfStreamException();
}
