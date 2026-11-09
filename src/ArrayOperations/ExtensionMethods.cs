namespace ArrayOperations;

// методы расширения: первый параметр помечен словом this
public static class ExtensionMethods
{
    public static bool ContainsChar(this string text, char symbol)
    {
        ArgumentNullException.ThrowIfNull(text);
        foreach (char current in text)
        {
            if (current == symbol)
            {
                return true;
            }
        }

        return false;
    }

    // возвращает новый массив, исходный не изменяется
    public static IntArray RemoveNegatives(this IntArray array)
    {
        ArgumentNullException.ThrowIfNull(array);

        int count = 0;
        foreach (int item in array)
        {
            if (item >= 0)
            {
                count++;
            }
        }

        var result = new IntArray(count);
        int position = 0;
        foreach (int item in array)
        {
            if (item >= 0)
            {
                result[position++] = item;
            }
        }

        return result;
    }
}
