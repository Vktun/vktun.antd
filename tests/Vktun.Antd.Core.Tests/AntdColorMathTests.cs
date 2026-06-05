using FluentAssertions;
using Vktun.Antd;

namespace Vktun.Antd.Core.Tests;

[TestClass]
public sealed class AntdColorMathTests
{
    [TestMethod]
    public void Blend_Ratio0_ReturnsFromColor()
    {
        var from = AntdColor.FromRgb(100, 150, 200);
        var to = AntdColor.FromRgb(50, 75, 100);

        var result = AntdColorMath.Blend(from, to, 0.0);

        result.Should().Be(from);
    }

    [TestMethod]
    public void Blend_Ratio1_ReturnsToColor()
    {
        var from = AntdColor.FromRgb(100, 150, 200);
        var to = AntdColor.FromRgb(50, 75, 100);

        var result = AntdColorMath.Blend(from, to, 1.0);

        result.Should().Be(to);
    }

    [TestMethod]
    public void Blend_Ratio05_ReturnsMidpoint()
    {
        var from = AntdColor.FromRgb(100, 100, 100);
        var to = AntdColor.FromRgb(200, 200, 200);

        var result = AntdColorMath.Blend(from, to, 0.5);

        result.R.Should().Be(150);
        result.G.Should().Be(150);
        result.B.Should().Be(150);
    }

    [TestMethod]
    public void Blend_RatioAbove1_ClampedTo1()
    {
        var from = AntdColor.FromRgb(100, 100, 100);
        var to = AntdColor.FromRgb(200, 200, 200);

        var result = AntdColorMath.Blend(from, to, 1.5);

        result.Should().Be(to);
    }

    [TestMethod]
    public void Blend_RatioBelow0_ClampedTo0()
    {
        var from = AntdColor.FromRgb(100, 100, 100);
        var to = AntdColor.FromRgb(200, 200, 200);

        var result = AntdColorMath.Blend(from, to, -0.5);

        result.Should().Be(from);
    }

    [TestMethod]
    public void Blend_BlackToWhite_ProducesGray()
    {
        var black = AntdColor.FromRgb(0, 0, 0);
        var white = AntdColor.FromRgb(255, 255, 255);

        var result = AntdColorMath.Blend(black, white, 0.5);

        result.R.Should().Be(128);
        result.G.Should().Be(128);
        result.B.Should().Be(128);
    }

    [TestMethod]
    public void Blend_PreservesAlphaChannel()
    {
        var from = AntdColor.FromArgb(100, 0, 0, 0);
        var to = AntdColor.FromArgb(200, 255, 255, 255);

        var result = AntdColorMath.Blend(from, to, 0.5);

        result.A.Should().Be(150);
    }

    [TestMethod]
    public void WithOpacity_Zero_ReturnsFullyTransparent()
    {
        var color = AntdColor.FromRgb(100, 150, 200);

        var result = AntdColorMath.WithOpacity(color, 0.0);

        result.A.Should().Be(0);
        result.R.Should().Be(100);
        result.G.Should().Be(150);
        result.B.Should().Be(200);
    }

    [TestMethod]
    public void WithOpacity_One_ReturnsFullyOpaque()
    {
        var color = AntdColor.FromRgb(100, 150, 200);

        var result = AntdColorMath.WithOpacity(color, 1.0);

        result.A.Should().Be(255);
    }

    [TestMethod]
    public void WithOpacity_Half_ReturnsHalfAlpha()
    {
        var color = AntdColor.FromRgb(100, 150, 200);

        var result = AntdColorMath.WithOpacity(color, 0.5);

        result.A.Should().Be(128);
    }

    [TestMethod]
    public void WithOpacity_ClampsAbove1()
    {
        var color = AntdColor.FromRgb(100, 150, 200);

        var result = AntdColorMath.WithOpacity(color, 2.0);

        result.A.Should().Be(255);
    }

    [TestMethod]
    public void WithOpacity_ClampsBelow0()
    {
        var color = AntdColor.FromRgb(100, 150, 200);

        var result = AntdColorMath.WithOpacity(color, -1.0);

        result.A.Should().Be(0);
    }
}
