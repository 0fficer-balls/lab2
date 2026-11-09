namespace Lab2.Tests.TestFramework;

[AttributeUsage(AttributeTargets.Method)]
public sealed class TestAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class TestCaseAttribute : Attribute
{
    public TestCaseAttribute(params object[] arguments)
    {
        Arguments = arguments;
    }

    public object[] Arguments { get; }
}
