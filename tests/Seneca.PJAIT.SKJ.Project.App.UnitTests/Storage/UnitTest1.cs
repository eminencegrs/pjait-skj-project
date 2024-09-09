using FluentAssertions;
using FluentAssertions.Execution;
using Seneca.PJAIT.SKJ.Project.Storage;
using Xunit;

namespace Seneca.PJAIT.SKJ.Project.App.UnitTests.Storage;

public class SimpleKeyValueStorageTests
{
    [Fact]
    public void GivenKeyValue_WhenTestSimpleKeyValueStorage_ThenResultAsExpected()
    {
        var storage = new SimpleKeyValueStorage();
        var expectedResult = new Pair(1, 1);

        Pair? actualResult = null;
        Action action = () =>
        {
            storage.NewPair(expectedResult);
            actualResult = storage.GetPair();
        };

        using (new AssertionScope())
        {
            action.Should().NotThrow();
            expectedResult.Should().BeEquivalentTo(actualResult);
        }
    }
}
