using System.Globalization;
using System.Reflection;
using System.Text;
using Lab2.Tests.TestFramework;

namespace Lab2.Tests;

internal static class Program
{
    private static int Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        Console.WriteLine("Программа тестирования лабораторной работы №2");
        int failed = TestRunner.RunAll(Assembly.GetExecutingAssembly());

        // код возврата 1 означает, что есть непройденные тесты
        return failed == 0 ? 0 : 1;
    }
}
