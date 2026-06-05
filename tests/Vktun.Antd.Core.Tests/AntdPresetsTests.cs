using FluentAssertions;
using Vktun.Antd;

namespace Vktun.Antd.Core.Tests;

[TestClass]
public sealed class AntdPresetsTests
{
    [TestMethod]
    public void Colors_AntdBlue_MatchesAntDesignSpec()
    {
        AntdPresets.Colors.AntdBlue.Should().Be(AntdColor.Parse("#1677FF"));
    }

    [TestMethod]
    public void Colors_CachedOnRepeatedAccess()
    {
        var first = AntdPresets.Colors.AntdBlue;
        var second = AntdPresets.Colors.AntdBlue;

        first.Should().Be(second);
    }

    [TestMethod]
    public void Colors_AllPresetsAreOpaque()
    {
        AntdPresets.Colors.AntdBlue.A.Should().Be(255);
        AntdPresets.Colors.AliyunOrange.A.Should().Be(255);
        AntdPresets.Colors.WeChatGreen.A.Should().Be(255);
        AntdPresets.Colors.GeekBlue.A.Should().Be(255);
        AntdPresets.Colors.Success.A.Should().Be(255);
        AntdPresets.Colors.Warning.A.Should().Be(255);
        AntdPresets.Colors.Error.A.Should().Be(255);
    }

    [TestMethod]
    public void Default_IsSameAsSeedTokenDefault()
    {
        AntdPresets.Default.Should().Be(AntdSeedToken.Default);
    }

    [TestMethod]
    public void Compact_HasSmallerHeights()
    {
        AntdPresets.Compact.ControlHeightSmall.Should().BeLessThan(AntdPresets.Default.ControlHeightSmall);
        AntdPresets.Compact.ControlHeightMiddle.Should().BeLessThan(AntdPresets.Default.ControlHeightMiddle);
        AntdPresets.Compact.ControlHeightLarge.Should().BeLessThan(AntdPresets.Default.ControlHeightLarge);
    }

    [TestMethod]
    public void Round_HasLargerBorderRadius()
    {
        AntdPresets.Round.BorderRadius.Should().BeGreaterThan(AntdPresets.Default.BorderRadius);
    }

    [TestMethod]
    public void Sharp_HasSmallerBorderRadius()
    {
        AntdPresets.Sharp.BorderRadius.Should().BeLessThan(AntdPresets.Default.BorderRadius);
    }

    [TestMethod]
    public void ThemePresets_LightDefault_UsesLightMode()
    {
        AntdThemePresets.LightDefault.Mode.Should().Be(AntdThemeMode.Light);
    }

    [TestMethod]
    public void ThemePresets_DarkDefault_UsesDarkMode()
    {
        AntdThemePresets.DarkDefault.Mode.Should().Be(AntdThemeMode.Dark);
    }
}
