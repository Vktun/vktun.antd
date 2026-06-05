using FluentAssertions;
using Vktun.Antd;

namespace Vktun.Antd.Core.Tests;

[TestClass]
public sealed class AntdSeedTokenTests
{
    [TestMethod]
    public void Default_HasExpectedValues()
    {
        var token = AntdSeedToken.Default;

        token.PrimaryColor.Should().Be(AntdColor.Parse("#1677FF"));
        token.SuccessColor.Should().Be(AntdColor.Parse("#52C41A"));
        token.WarningColor.Should().Be(AntdColor.Parse("#FAAD14"));
        token.ErrorColor.Should().Be(AntdColor.Parse("#FF4D4F"));
        token.FontSizeBase.Should().Be(14d);
        token.ControlHeightSmall.Should().Be(32d);
        token.ControlHeightMiddle.Should().Be(40d);
        token.ControlHeightLarge.Should().Be(48d);
        token.BorderRadius.Should().Be(8d);
    }

    [TestMethod]
    public void Equals_SameValues_ReturnsTrue()
    {
        var a = new AntdSeedToken { PrimaryColor = AntdColor.Parse("#FF0000") };
        var b = new AntdSeedToken { PrimaryColor = AntdColor.Parse("#FF0000") };

        a.Equals(b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [TestMethod]
    public void Equals_DifferentPrimaryColor_ReturnsFalse()
    {
        var a = new AntdSeedToken { PrimaryColor = AntdColor.Parse("#FF0000") };
        var b = new AntdSeedToken { PrimaryColor = AntdColor.Parse("#00FF00") };

        a.Equals(b).Should().BeFalse();
    }

    [TestMethod]
    public void Equals_DifferentFontSize_ReturnsFalse()
    {
        var a = new AntdSeedToken { FontSizeBase = 14d };
        var b = new AntdSeedToken { FontSizeBase = 16d };

        a.Equals(b).Should().BeFalse();
    }

    [TestMethod]
    public void Equals_Null_ReturnsFalse()
    {
        var token = AntdSeedToken.Default;
        token.Equals(null).Should().BeFalse();
    }

    [TestMethod]
    public void Equals_SameReference_ReturnsTrue()
    {
        var token = AntdSeedToken.Default;
        token.Equals(token).Should().BeTrue();
    }

    [TestMethod]
    public void Equals_ObjectOverload_WorksCorrectly()
    {
        var a = AntdSeedToken.Default;
        object b = new AntdSeedToken();

        a.Equals(b).Should().BeTrue();
    }

    [TestMethod]
    public void GetHashCode_ConsistentForSameValues()
    {
        var a = new AntdSeedToken { BorderRadius = 12d };
        var b = new AntdSeedToken { BorderRadius = 12d };

        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_DiffersForDifferentValues()
    {
        var a = new AntdSeedToken { BorderRadius = 12d };
        var b = new AntdSeedToken { BorderRadius = 8d };

        a.GetHashCode().Should().NotBe(b.GetHashCode());
    }
}
