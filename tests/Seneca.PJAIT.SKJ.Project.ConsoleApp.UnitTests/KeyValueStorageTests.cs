using FluentAssertions;
using FluentAssertions.Execution;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;
using Xunit;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.UnitTests;

public class KeyValueStorageTests
{
    [Theory]
    [MemberData(nameof(PairCreationTestCases))]
    public void GivenPair_WhenSetKeyValue_ThenResultAsExpected(Pair input, Pair expectedResult)
    {
        var storage = new KeyValueStorage();

        Pair? actualResult = null;
        Action action = () =>
        {
            storage.SetKeyValue(input);
            actualResult = storage.GetPair();
        };

        using (new AssertionScope())
        {
            action.Should().NotThrow();
            actualResult.Should().NotBeNull();
            actualResult.Should().Be(expectedResult);
        }
    }

    [Theory]
    [MemberData(nameof(TestCases))]
    public void GivenKey_WhenGetValue_ThenResultAsExpected(
        KeyValueStorage storage, int key, Pair? expectedResult)
    {
        Pair? actualResult = null;
        Action action = () => actualResult = storage.GetValue(key);

        using (new AssertionScope())
        {
            action.Should().NotThrow();
            actualResult.Should().Be(expectedResult);
        }
    }

    public static IEnumerable<object[]> PairCreationTestCases()
    {
        yield return [new Pair(1, 123), new Pair(1, 123)];
    }

    public static IEnumerable<object[]> TestCases()
    {
        var pair = new Pair(1, 123);
        var storage = new KeyValueStorage();
        storage.SetKeyValue(pair);

        yield return [storage, 1, pair];
        yield return [storage, 2, null];
    }
}
