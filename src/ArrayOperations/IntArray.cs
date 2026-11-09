using System.Collections;
using System.Text;

namespace ArrayOperations;

// Одномерный массив целых чисел (вариант 1)
public class IntArray : IEquatable<IntArray>, IComparable<IntArray>, IEnumerable<int>
{
    private const string SeparatorText = ", ";

    private readonly int[] items;

    public IntArray(int size)
    {
        if (size < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), size,
                "Размер массива не может быть отрицательным.");
        }

        items = new int[size];
    }

    public IntArray(int[] values)
    {
        ArgumentNullException.ThrowIfNull(values);
        items = (int[])values.Clone();
    }

    public IntArray(IntArray other)
        : this(other?.items ?? throw new ArgumentNullException(nameof(other)))
    {
    }

    public int Length => items.Length;

    public int this[int index]
    {
        get
        {
            CheckIndex(index);
            return items[index];
        }
        set
        {
            CheckIndex(index);
            items[index] = value;
        }
    }

    public bool HasNegative()
    {
        foreach (int item in items)
        {
            if (item < 0)
            {
                return true;
            }
        }

        return false;
    }

    public long Sum()
    {
        long sum = 0;
        foreach (int item in items)
        {
            sum += item;
        }

        return sum;
    }

    public int[] ToArray() => (int[])items.Clone();

    // поэлементное умножение, размеры массивов должны совпадать
    public static IntArray operator *(IntArray left, IntArray right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        if (left.Length != right.Length)
        {
            throw new ArgumentException(
                $"Умножать можно только массивы одного размера ({left.Length} и {right.Length}).");
        }

        var result = new IntArray(left.Length);
        for (int i = 0; i < left.Length; i++)
        {
            result.items[i] = left.items[i] * right.items[i];
        }

        return result;
    }

    // true - в массиве нет отрицательных элементов; false объявляется в паре с true
    public static bool operator true(IntArray? array) => array is not null && !array.HasNegative();

    public static bool operator false(IntArray? array) => array is null || array.HasNegative();

    // приведение к int возвращает размер массива
    public static explicit operator int(IntArray array)
    {
        ArgumentNullException.ThrowIfNull(array);
        return array.Length;
    }

    // равенство: одинаковый размер и все элементы совпадают
    public static bool operator ==(IntArray? left, IntArray? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(IntArray? left, IntArray? right) => !(left == right);

    // сравнение в лексикографическом порядке, как у строк
    public static bool operator <(IntArray? left, IntArray? right) => Compare(left, right) < 0;

    public static bool operator >(IntArray? left, IntArray? right) => Compare(left, right) > 0;

    public int CompareTo(IntArray? other)
    {
        if (other is null)
        {
            return 1;
        }

        int commonLength = Math.Min(Length, other.Length);
        for (int i = 0; i < commonLength; i++)
        {
            int result = items[i].CompareTo(other.items[i]);
            if (result != 0)
            {
                return result;
            }
        }

        return Length.CompareTo(other.Length);
    }

    public bool Equals(IntArray? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return items.AsSpan().SequenceEqual(other.items);
    }

    public override bool Equals(object? obj) => Equals(obj as IntArray);

    // при перегрузке == нужно переопределить Equals и GetHashCode
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (int item in items)
        {
            hash.Add(item);
        }

        return hash.ToHashCode();
    }

    public override string ToString()
    {
        var builder = new StringBuilder("[");
        for (int i = 0; i < items.Length; i++)
        {
            if (i > 0)
            {
                builder.Append(SeparatorText);
            }

            builder.Append(items[i]);
        }

        return builder.Append(']').ToString();
    }

    public IEnumerator<int> GetEnumerator() => ((IEnumerable<int>)items).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static int Compare(IntArray? left, IntArray? right)
    {
        if (left is null)
        {
            return right is null ? 0 : -1;
        }

        return left.CompareTo(right);
    }

    private void CheckIndex(int index)
    {
        if (index < 0 || index >= items.Length)
        {
            throw new IndexOutOfRangeException(
                $"Индекс {index} вне диапазона 0..{items.Length - 1}.");
        }
    }
}
