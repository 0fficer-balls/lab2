namespace Lab2.Tests.TestFramework;

public sealed class AssertionException : Exception
{
    public AssertionException(string message) : base(message)
    {
    }
}

public static class Assert
{
    public static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new AssertionException($"ожидалось <{expected}>, получено <{actual}>");
        }
    }

    public static void SequenceEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
    {
        if (!expected.SequenceEqual(actual))
        {
            throw new AssertionException(
                $"ожидалось [{string.Join(", ", expected)}], получено [{string.Join(", ", actual)}]");
        }
    }

    public static void True(bool condition, string message = "условие ложно")
    {
        if (!condition)
        {
            throw new AssertionException(message);
        }
    }

    public static void False(bool condition, string message = "условие истинно")
    {
        if (condition)
        {
            throw new AssertionException(message);
        }
    }

    public static void NotSame(object? notExpected, object? actual)
    {
        if (ReferenceEquals(notExpected, actual))
        {
            throw new AssertionException("ссылки указывают на один и тот же объект");
        }
    }

    public static void Contains(string expectedSubstring, string actual)
    {
        if (!actual.Contains(expectedSubstring, StringComparison.Ordinal))
        {
            throw new AssertionException($"строка не содержит «{expectedSubstring}»: «{actual}»");
        }
    }

    public static void IsType<T>(object? actual)
    {
        if (actual is null || actual.GetType() != typeof(T))
        {
            throw new AssertionException($"ожидался тип {typeof(T).Name}, получен {actual?.GetType().Name ?? "null"}");
        }
    }

    // проверка проходит, только если выброшено исключение нужного типа
    public static TException Throws<TException>(Action action) where TException : Exception
    {
        try
        {
            action();
        }
        catch (Exception exception) when (exception.GetType() == typeof(TException))
        {
            return (TException)exception;
        }
        catch (Exception exception)
        {
            throw new AssertionException(
                $"ожидалось исключение {typeof(TException).Name}, получено {exception.GetType().Name}");
        }

        throw new AssertionException($"ожидалось исключение {typeof(TException).Name}, но его не было");
    }
}
