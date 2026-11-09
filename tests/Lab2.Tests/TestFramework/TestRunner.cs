using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Lab2.Tests.TestFramework;

public static class TestRunner
{
    private const string PassedMark = "[ OK ]";
    private const string FailedMark = "[FAIL]";

    // тесты ищутся через отражение по атрибутам [Test] и [TestCase]
    public static int RunAll(Assembly assembly)
    {
        int passed = 0;
        int failed = 0;
        var stopwatch = Stopwatch.StartNew();

        IEnumerable<Type> testClasses = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && GetTestMethods(type).Any())
            .OrderBy(type => type.Name);

        foreach (Type testClass in testClasses)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {testClass.Name} ---");
            foreach (MethodInfo method in GetTestMethods(testClass))
            {
                foreach (object[] arguments in GetArgumentSets(method))
                {
                    string title = FormatTitle(method, arguments);
                    string? error = RunSingle(testClass, method, arguments);
                    if (error is null)
                    {
                        passed++;
                        Console.WriteLine($"{PassedMark} {title}");
                    }
                    else
                    {
                        failed++;
                        Console.WriteLine($"{FailedMark} {title}: {error}");
                    }
                }
            }
        }

        stopwatch.Stop();
        Console.WriteLine();
        Console.WriteLine($"Итого тестов: {passed + failed}, пройдено: {passed}, не пройдено: {failed}, " +
                          $"время: {stopwatch.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture)} с");
        return failed;
    }

    private static IEnumerable<MethodInfo> GetTestMethods(Type type) =>
        type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(method => method.IsDefined(typeof(TestAttribute)) ||
                             method.IsDefined(typeof(TestCaseAttribute)))
            .OrderBy(method => method.MetadataToken);

    private static IEnumerable<object[]> GetArgumentSets(MethodInfo method)
    {
        TestCaseAttribute[] cases = method.GetCustomAttributes<TestCaseAttribute>().ToArray();
        return cases.Length == 0
            ? [Array.Empty<object>()]
            : cases.Select(testCase => testCase.Arguments);
    }

    private static string? RunSingle(Type testClass, MethodInfo method, object[] arguments)
    {
        try
        {
            object instance = Activator.CreateInstance(testClass)!;
            method.Invoke(instance, arguments.Length == 0 ? null : arguments);
            return null;
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            Exception inner = exception.InnerException;
            return inner is AssertionException
                ? inner.Message
                : $"необработанное исключение {inner.GetType().Name}: {inner.Message}";
        }
    }

    private static string FormatTitle(MethodInfo method, object[] arguments)
    {
        if (arguments.Length == 0)
        {
            return method.Name;
        }

        IEnumerable<string> values = arguments.Select(argument => argument switch
        {
            string text => $"\"{text}\"",
            char symbol => $"'{symbol}'",
            int[] array => $"[{string.Join(", ", array)}]",
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => argument?.ToString() ?? "null",
        });
        return $"{method.Name}({string.Join(", ", values)})";
    }
}
