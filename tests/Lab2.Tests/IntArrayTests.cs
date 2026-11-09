using ArrayOperations;
using Lab2.Tests.TestFramework;

namespace Lab2.Tests;

// тесты конструкторов, индексатора и перегруженных операций
public class IntArrayTests
{
    [Test]
    public void Constructor_WithSize_CreatesZeroFilledArray()
    {
        var array = new IntArray(3);

        Assert.Equal(3, array.Length);
        Assert.SequenceEqual(new[] { 0, 0, 0 }, array);
    }

    [Test]
    public void Constructor_NegativeSize_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new IntArray(-1));
    }

    [Test]
    public void Constructor_FromArray_CopiesValues()
    {
        int[] source = [5, -6, 7];
        var array = new IntArray(source);
        source[0] = 100;

        Assert.SequenceEqual(new[] { 5, -6, 7 }, array);
    }

    [Test]
    public void CopyConstructor_CreatesIndependentCopy()
    {
        var original = new IntArray(new[] { 1, 2, 3 });
        var copy = new IntArray(original);
        copy[0] = 99;

        Assert.Equal(1, original[0]);
        Assert.Equal(99, copy[0]);
        Assert.NotSame(original, copy);
    }

    [Test]
    public void Indexer_ReadsAndWritesElements()
    {
        var array = new IntArray(new[] { 10, 20, 30 });
        array[1] = -5;

        Assert.Equal(10, array[0]);
        Assert.Equal(-5, array[1]);
        Assert.Equal(30, array[2]);
    }

    [TestCase(-1)]
    [TestCase(3)]
    public void Indexer_IndexOutOfRange_ThrowsIndexOutOfRangeException(int index)
    {
        var array = new IntArray(new[] { 1, 2, 3 });

        Assert.Throws<IndexOutOfRangeException>(() => _ = array[index]);
        Assert.Throws<IndexOutOfRangeException>(() => array[index] = 0);
    }

    [TestCase(new[] { 1, 2, 3, 4 }, new[] { 2, 0, -1, 5 }, new[] { 2, 0, -3, 20 })]
    [TestCase(new[] { -2, -3 }, new[] { -4, 5 }, new[] { 8, -15 })]
    public void Multiply_SameSize_MultipliesElementwise(int[] left, int[] right, int[] expected)
    {
        IntArray result = new IntArray(left) * new IntArray(right);

        Assert.SequenceEqual(expected, result);
    }

    [Test]
    public void Multiply_DifferentSize_ThrowsArgumentException()
    {
        var left = new IntArray(new[] { 1, 2, 3 });
        var right = new IntArray(new[] { 1, 2 });

        Assert.Throws<ArgumentException>(() => _ = left * right);
    }

    [Test]
    public void Multiply_DoesNotChangeOperands()
    {
        var left = new IntArray(new[] { 1, 2 });
        var right = new IntArray(new[] { 3, 4 });
        _ = left * right;

        Assert.SequenceEqual(new[] { 1, 2 }, left);
        Assert.SequenceEqual(new[] { 3, 4 }, right);
    }

    [TestCase(new[] { 1, 2, 3 })]
    [TestCase(new[] { 0, 0 })]
    [TestCase(new int[0])]
    public void OperatorTrue_NoNegativeElements_IsTrue(int[] values)
    {
        var array = new IntArray(values);
        bool result = false;
        if (array)
        {
            result = true;
        }

        Assert.True(result);
    }

    [TestCase(new[] { 1, -2, 3 })]
    [TestCase(new[] { -5 })]
    public void OperatorTrue_HasNegativeElements_IsFalse(int[] values)
    {
        var array = new IntArray(values);
        bool result = true;
        if (array)
        {
            result = false;
        }

        Assert.True(result, "массив с отрицательными элементами должен считаться ложным");
    }

    [TestCase(new[] { 7, 8, 9, 10 }, 4)]
    [TestCase(new[] { 1 }, 1)]
    [TestCase(new int[0], 0)]
    public void ExplicitCastToInt_ReturnsSize(int[] values, int expectedSize)
    {
        Assert.Equal(expectedSize, (int)new IntArray(values));
    }

    [Test]
    public void Equality_SameElements_IsTrue()
    {
        var first = new IntArray(new[] { 1, 2, 3 });
        var second = new IntArray(new[] { 1, 2, 3 });

        Assert.True(first == second);
        Assert.False(first != second);
        Assert.True(first.Equals(second));
    }

    [TestCase(new[] { 1, 2, 3 }, new[] { 1, 2, 4 })]
    [TestCase(new[] { 1, 2, 3 }, new[] { 1, 2 })]
    [TestCase(new[] { 1, 2, 3 }, new[] { 3, 2, 1 })]
    public void Equality_DifferentElementsOrSize_IsFalse(int[] left, int[] right)
    {
        var first = new IntArray(left);
        var second = new IntArray(right);

        Assert.False(first == second);
        Assert.True(first != second);
    }

    [Test]
    public void Equality_WithNull_HandledCorrectly()
    {
        var array = new IntArray(new[] { 1 });
        IntArray? nothing = null;

        Assert.False(array == nothing);
        Assert.True(nothing == null);
        Assert.False(array.Equals(null));
    }

    [Test]
    public void GetHashCode_EqualArrays_HaveEqualHashCodes()
    {
        Assert.Equal(new IntArray(new[] { 4, 5, 6 }).GetHashCode(), new IntArray(new[] { 4, 5, 6 }).GetHashCode());
    }

    [TestCase(new[] { 1, 2, 3 }, new[] { 1, 2, 4 })]
    [TestCase(new[] { 1, 2 }, new[] { 1, 2, 0 })]
    [TestCase(new[] { -1, 9, 9 }, new[] { 0 })]
    public void LessAndGreater_ComparesLexicographically(int[] smaller, int[] larger)
    {
        var first = new IntArray(smaller);
        var second = new IntArray(larger);

        Assert.True(first < second);
        Assert.True(second > first);
        Assert.False(first > second);
        Assert.False(second < first);
    }

    [Test]
    public void LessAndGreater_EqualArrays_AreFalse()
    {
        var first = new IntArray(new[] { 1, 2, 3 });
        var second = new IntArray(new[] { 1, 2, 3 });

        Assert.False(first < second);
        Assert.False(first > second);
    }

    [Test]
    public void ToString_FormatsElementsInBrackets()
    {
        Assert.Equal("[1, -2, 3]", new IntArray(new[] { 1, -2, 3 }).ToString());
        Assert.Equal("[]", new IntArray(0).ToString());
    }
}
