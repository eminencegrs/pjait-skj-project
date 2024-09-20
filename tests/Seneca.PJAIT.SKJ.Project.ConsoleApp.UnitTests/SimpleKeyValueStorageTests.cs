using FluentAssertions;
using FluentAssertions.Execution;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;
using Xunit;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.UnitTests;

public class KeyValueStorageTests
{
    [Fact]
    public void GivenKeyValue_WhenTestSimpleKeyValueStorage_ThenResultAsExpected()
    {
        var storage = new KeyValueStorage();
        var expectedResult = new Pair(1, 1);

        Pair? actualResult = null;
        Action action = () =>
        {
            storage.CreatePair(expectedResult);
            actualResult = storage.GetPair();
        };

        using (new AssertionScope())
        {
            action.Should().NotThrow();
            expectedResult.Should().BeEquivalentTo(actualResult);
        }
    }
}
