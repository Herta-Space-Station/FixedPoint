using Herta;

namespace Test
{
    [FPGenerator]
    public static partial class TestGenerator
    {
        private const float Sb = 9999f;
        private const float Sb2 = 0.9999f;
    }

    [FPGenerator]
    public static partial class TestGenerator2
    {
        private const float Sb = 9999f;
        private const float Sb2 = 0.9999f;
    }
}

[FPGenerator]
public static partial class TestGenerator
{
    private const float Sb = 0.1f;
    private const float Sb2 = 0.1f;
}

[FPGenerator]
public static partial class TestGenerator2
{
    private const float Sb = 0.1f;
    private const float Sb2 = 0.1f;
}