using TelegramBot.Domain.Constants;
using Xunit;

namespace TelegramBot.Domain.Tests.Constants;

public class IconsStringsTests
{
    private static readonly string[] ExpectedNames = ["Robot", "Spanner", "Ok", "Ko", "CashBag", "System"];

    [Fact]
    public void Names_ContainsAllConstantFields()
    {
        Assert.Equal(ExpectedNames.Order(), IconsStrings.Names.Order());
    }

    [Fact]
    public void Names_HasExpectedCount()
    {
        Assert.Equal(ExpectedNames.Length, IconsStrings.Names.Length);
    }

    [Fact]
    public void Names_ExcludesNonConstMembers()
    {
        Assert.DoesNotContain("Names", IconsStrings.Names);
        Assert.DoesNotContain("FieldInfos", IconsStrings.Names);
    }

    [Theory]
    [InlineData("Robot", "🤖")]
    [InlineData("Spanner", "🔧")]
    [InlineData("Ok", "✅")]
    [InlineData("Ko", "❌")]
    [InlineData("CashBag", "💰")]
    [InlineData("System", "🟢")]
    public void Indexer_ReturnsCorrectEmoji(string name, string expected)
    {
        var icons = new IconsStrings();

        Assert.Equal(expected, icons[name]);
    }

    [Fact]
    public void Indexer_UnknownName_ThrowsInvalidOperationException()
    {
        var icons = new IconsStrings();

        Assert.Throws<InvalidOperationException>(() => _ = icons["NonExistent"]);
    }
}
