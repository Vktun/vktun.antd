using FluentAssertions;
using Vktun.Antd;

namespace Vktun.Antd.Core.Tests;

[TestClass]
public sealed class AntdColorTests
{
    [TestMethod]
    public void Parse_ValidRgb_ReturnsCorrectColor()
    {
        var color = AntdColor.Parse("#1677FF");

        color.R.Should().Be(0x16);
        color.G.Should().Be(0x77);
        color.B.Should().Be(0xFF);
        color.A.Should().Be(255);
    }

    [TestMethod]
    public void Parse_ValidArgb_ReturnsCorrectColor()
    {
        var color = AntdColor.Parse("#801677FF");

        color.A.Should().Be(0x80);
        color.R.Should().Be(0x16);
        color.G.Should().Be(0x77);
        color.B.Should().Be(0xFF);
    }

    [TestMethod]
    public void Parse_WithoutHash_ReturnsCorrectColor()
    {
        var color = AntdColor.Parse("1677FF");

        color.R.Should().Be(0x16);
        color.G.Should().Be(0x77);
        color.B.Should().Be(0xFF);
    }

    [TestMethod]
    public void Parse_InvalidLength_Throws()
    {
        var act = () => AntdColor.Parse("#FFF");
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void Parse_NullOrEmpty_Throws()
    {
        var actNull = () => AntdColor.Parse(null!);
        var actEmpty = () => AntdColor.Parse("");

        actNull.Should().Throw<ArgumentException>();
        actEmpty.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void ToHex_RoundTrip_PreservesColor()
    {
        var original = AntdColor.FromRgb(100, 150, 200);
        var hex = original.ToHex();
        var parsed = AntdColor.Parse(hex);

        parsed.Should().Be(original);
    }

    [TestMethod]
    public void ToHex_WithAlpha_IncludesAlphaChannel()
    {
        var color = AntdColor.FromArgb(128, 100, 150, 200);
        var hex = color.ToHex(includeAlpha: true);

        hex.Should().StartWith("#");
        hex.Length.Should().Be(9);
    }

    [TestMethod]
    public void ToString_OpaqueColor_ExcludesAlpha()
    {
        var color = AntdColor.FromRgb(255, 0, 0);
        color.ToString().Length.Should().Be(7);
    }

    [TestMethod]
    public void ToString_TransparentColor_IncludesAlpha()
    {
        var color = AntdColor.FromArgb(128, 255, 0, 0);
        color.ToString().Length.Should().Be(9);
    }

    [TestMethod]
    public void FromRgb_SetsAlphaTo255()
    {
        var color = AntdColor.FromRgb(0, 0, 0);
        color.A.Should().Be(255);
    }

    [TestMethod]
    public void Equality_SameValues_AreEqual()
    {
        var a = AntdColor.FromRgb(10, 20, 30);
        var b = AntdColor.FromRgb(10, 20, 30);

        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }

    [TestMethod]
    public void Equality_DifferentValues_AreNotEqual()
    {
        var a = AntdColor.FromRgb(10, 20, 30);
        var b = AntdColor.FromRgb(10, 20, 31);

        a.Should().NotBe(b);
    }
}
