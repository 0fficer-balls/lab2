using ArrayOperations;
using Lab2.Tests.TestFramework;

namespace Lab2.Tests;

public class ExtensionMethodsTests
{
    [TestCase("Перегрузка операций", 'з')]
    [TestCase("abc", 'c')]
    [TestCase("a b", ' ')]
    public void ContainsChar_SymbolPresent_ReturnsTrue(string text, char symbol)
    {
        Assert.True(text.ContainsChar(symbol));
    }

    [TestCase("Перегрузка операций", 'ы')]
    [TestCase("", 'a')]
    [TestCase("Abc", 'a')]
    public void ContainsChar_SymbolAbsent_ReturnsFalse(string text, char symbol)
    {
        Assert.False(text.ContainsChar(symbol));
    }

    [Test]
    public void ContainsChar_NullString_ThrowsArgumentNullException()
    {
        string? text = null;

        Assert.Throws<ArgumentNullException>(() => text!.ContainsChar('a'));
    }

    [TestCase(new[] { 2, 0, -1, 5 }, new[] { 2, 0, 5 })]
    [TestCase(new[] { -3, 7, -1, 0, 8 }, new[] { 7, 0, 8 })]
    [TestCase(new[] { 1, 2, 3 }, new[] { 1, 2, 3 })]
    [TestCase(new[] { -1, -2 }, new int[0])]
    public void RemoveNegatives_ReturnsOnlyNonNegativeElements(int[] source, int[] expected)
    {
        IntArray result = new IntArray(source).RemoveNegatives();

        Assert.SequenceEqual(expected, result);
    }

    [Test]
    public void RemoveNegatives_DoesNotChangeSourceArray()
    {
        var array = new IntArray(new[] { -1, 2, -3 });
        IntArray result = array.RemoveNegatives();

        Assert.SequenceEqual(new[] { -1, 2, -3 }, array);
        Assert.NotSame(array, result);
    }

    [Test]
    public void RemoveNegatives_ResultIsTrueByOperatorTrue()
    {
        IntArray result = new IntArray(new[] { -5, 4, -3, 2 }).RemoveNegatives();
        bool isTrue = false;
        if (result)
        {
            isTrue = true;
        }

        Assert.True(isTrue, "после удаления отрицательных элементов operator true должен вернуть true");
    }
}
