using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.UnitTests;

public class NodeTests
{
    [Theory]
    [MemberData(nameof(NodeCreationTestCases))]
    public void GivenHostAndPort_WhenCreateNode_ThenResultAsExpected(
        string localhost, int port, Node expectedResult)
    {
        Node? actualResult = null;

        Action action = () => actualResult = new Node(localhost, port);

        using (new AssertionScope())
        {
            action.Should().NotThrow();
            actualResult.Should().NotBeNull();
            actualResult.Should().BeEquivalentTo(expectedResult);
        }
    }

    [Theory]
    [MemberData(nameof(NodesComparisonTestCases))]
    public void GivenNodes_WhenCompareNodes_ThenResultAsExpected(
        Node first, Node second, bool expectedResult)
    {
        bool? actualResult = null;
        bool? areHashCodesEqual = null;

        Action action = () =>
        {
            actualResult = first.Equals(second);
            areHashCodesEqual = first.GetHashCode() == second.GetHashCode();
        };

        using (new AssertionScope())
        {
            action.Should().NotThrow();
            actualResult.Should().NotBeNull();
            actualResult.Should().Be(expectedResult);
            areHashCodesEqual.Should().Be(expectedResult);
        }
    }

    [Theory]
    [MemberData(nameof(StringParsingTestCases))]
    public void GivenString_WhenParse_ThenResultAsExpected(string input, Node expectedResult)
    {
        Node? actualResult = null;

        Action action = () => actualResult = Node.Parse(input);

        using (new AssertionScope())
        {
            action.Should().NotThrow();
            actualResult.Should().NotBeNull();
            actualResult.Should().Be(expectedResult);
        }
    }

    public static IEnumerable<object[]> NodeCreationTestCases()
    {
        yield return ["localhost", 5000, new Node("localhost", 5000)];
    }

    public static IEnumerable<object[]> NodesComparisonTestCases()
    {
        yield return [new Node("localhost", 5000), new Node("localhost", 5000), true];
        yield return [new Node("192.168.0.1", 5000), new Node("192.168.0.2", 5000), false];
    }

    public static IEnumerable<object[]> StringParsingTestCases()
    {
        yield return ["0.0.0.0:5000", new Node("0.0.0.0", 5000)];
    }
}
