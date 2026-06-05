using FluentAssertions;
using Vktun.Antd;

namespace Vktun.Antd.Core.Tests;

[TestClass]
public sealed class AntdTokenFactoryTests
{
    [TestInitialize]
    public void Setup() => AntdTokenFactory.ClearCache();

    [TestMethod]
    public void Create_ShouldResolveLightThemeTokens()
    {
        var tokens = AntdTokenFactory.Create(AntdThemeMode.Light, AntdSeedToken.Default);

        tokens.ColorPrimary.Should().Be(AntdColor.Parse("#1677FF"));
        tokens.ColorBgBase.Should().Be(AntdColor.Parse("#FFFFFF"));
        tokens.ControlHeightMiddle.Should().Be(40d);
        tokens.BorderRadiusMiddle.Should().Be(8d);
    }

    [TestMethod]
    public void Create_ShouldApplyCompactControlHeights()
    {
        var tokens = AntdTokenFactory.Create(AntdThemeMode.Compact, AntdSeedToken.Default);

        tokens.ControlHeightSmall.Should().Be(28d);
        tokens.ControlHeightMiddle.Should().Be(36d);
        tokens.ControlHeightLarge.Should().Be(44d);
    }

    [TestMethod]
    public void Create_DarkTheme_UsesDarkBackground()
    {
        var tokens = AntdTokenFactory.Create(AntdThemeMode.Dark, AntdSeedToken.Default);

        tokens.ColorBgBase.R.Should().BeLessThan(50);
        tokens.ColorBgBase.G.Should().BeLessThan(50);
        tokens.ColorBgBase.B.Should().BeLessThan(50);
    }

    [TestMethod]
    public void Create_DarkTheme_UsesLightText()
    {
        var tokens = AntdTokenFactory.Create(AntdThemeMode.Dark, AntdSeedToken.Default);

        tokens.ColorText.R.Should().BeGreaterThan(200);
    }

    [TestMethod]
    public void Create_LightTheme_HoverIsLighterThanPrimary()
    {
        var tokens = AntdTokenFactory.Create(AntdThemeMode.Light, AntdSeedToken.Default);

        var primaryBrightness = tokens.ColorPrimary.R + tokens.ColorPrimary.G + tokens.ColorPrimary.B;
        var hoverBrightness = tokens.ColorPrimaryHover.R + tokens.ColorPrimaryHover.G + tokens.ColorPrimaryHover.B;

        hoverBrightness.Should().BeGreaterThan(primaryBrightness);
    }

    [TestMethod]
    public void Create_LightTheme_ActiveIsDarkerThanPrimary()
    {
        var tokens = AntdTokenFactory.Create(AntdThemeMode.Light, AntdSeedToken.Default);

        var primaryBrightness = tokens.ColorPrimary.R + tokens.ColorPrimary.G + tokens.ColorPrimary.B;
        var activeBrightness = tokens.ColorPrimaryActive.R + tokens.ColorPrimaryActive.G + tokens.ColorPrimaryActive.B;

        activeBrightness.Should().BeLessThan(primaryBrightness);
    }

    [TestMethod]
    public void Create_CompactTheme_UsesLightColorScheme()
    {
        var lightTokens = AntdTokenFactory.Create(AntdThemeMode.Light, AntdSeedToken.Default);
        var compactTokens = AntdTokenFactory.Create(AntdThemeMode.Compact, AntdSeedToken.Default);

        compactTokens.ColorBgBase.Should().Be(lightTokens.ColorBgBase);
        compactTokens.ColorText.Should().Be(lightTokens.ColorText);
    }

    [TestMethod]
    public void Create_CompactTheme_HasSmallerPadding()
    {
        var lightTokens = AntdTokenFactory.Create(AntdThemeMode.Light, AntdSeedToken.Default);
        var compactTokens = AntdTokenFactory.Create(AntdThemeMode.Compact, AntdSeedToken.Default);

        compactTokens.PaddingSm.Should().BeLessThan(lightTokens.PaddingSm);
        compactTokens.PaddingMd.Should().BeLessThan(lightTokens.PaddingMd);
        compactTokens.PaddingLg.Should().BeLessThan(lightTokens.PaddingLg);
    }

    [TestMethod]
    public void Create_CustomSeed_UsesPrimaryColor()
    {
        var customSeed = new AntdSeedToken { PrimaryColor = AntdColor.Parse("#FF0000") };
        var tokens = AntdTokenFactory.Create(AntdThemeMode.Light, customSeed);

        tokens.ColorPrimary.Should().Be(AntdColor.Parse("#FF0000"));
        tokens.ColorLink.Should().Be(AntdColor.Parse("#FF0000"));
    }

    [TestMethod]
    public void Create_CustomSeed_DerivesBorderRadius()
    {
        var customSeed = new AntdSeedToken { BorderRadius = 12d };
        var tokens = AntdTokenFactory.Create(AntdThemeMode.Light, customSeed);

        tokens.BorderRadiusSmall.Should().Be(10d);
        tokens.BorderRadiusMiddle.Should().Be(12d);
        tokens.BorderRadiusLarge.Should().Be(16d);
    }

    [TestMethod]
    public void Create_CustomSeed_DerivesFontSizes()
    {
        var customSeed = new AntdSeedToken { FontSizeBase = 16d };
        var tokens = AntdTokenFactory.Create(AntdThemeMode.Light, customSeed);

        tokens.FontSizeBase.Should().Be(16d);
        tokens.FontSizeSmall.Should().Be(15d);
        tokens.FontSizeLarge.Should().Be(18d);
    }

    [TestMethod]
    public void Create_CachesResultForSameInput()
    {
        var result1 = AntdTokenFactory.Create(AntdThemeMode.Light, AntdSeedToken.Default);
        var result2 = AntdTokenFactory.Create(AntdThemeMode.Light, AntdSeedToken.Default);

        ReferenceEquals(result1, result2).Should().BeTrue();
    }

    [TestMethod]
    public void Create_DifferentModes_ReturnDifferentResults()
    {
        var light = AntdTokenFactory.Create(AntdThemeMode.Light, AntdSeedToken.Default);
        var dark = AntdTokenFactory.Create(AntdThemeMode.Dark, AntdSeedToken.Default);

        light.ColorBgBase.Should().NotBe(dark.ColorBgBase);
    }

    [TestMethod]
    public void Create_NullSeed_Throws()
    {
        var act = () => AntdTokenFactory.Create(AntdThemeMode.Light, null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Create_InvalidSeed_NegativeFontSize_Throws()
    {
        var invalidSeed = new AntdSeedToken { FontSizeBase = -10d };
        var act = () => AntdTokenFactory.Create(AntdThemeMode.Light, invalidSeed);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void Create_InvalidSeed_HeightOrderViolation_Throws()
    {
        var invalidSeed = new AntdSeedToken
        {
            ControlHeightSmall = 50d,
            ControlHeightMiddle = 40d,
            ControlHeightLarge = 48d
        };
        var act = () => AntdTokenFactory.Create(AntdThemeMode.Light, invalidSeed);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void Create_InvalidSeed_NegativeBorderRadius_Throws()
    {
        var invalidSeed = new AntdSeedToken { BorderRadius = -1d };
        var act = () => AntdTokenFactory.Create(AntdThemeMode.Light, invalidSeed);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void ClearCache_InvalidatesCachedResults()
    {
        var result1 = AntdTokenFactory.Create(AntdThemeMode.Light, AntdSeedToken.Default);
        AntdTokenFactory.ClearCache();
        var result2 = AntdTokenFactory.Create(AntdThemeMode.Light, AntdSeedToken.Default);

        ReferenceEquals(result1, result2).Should().BeFalse();
        result1.ColorPrimary.Should().Be(result2.ColorPrimary);
    }

    [TestMethod]
    public void Create_AllModes_ProduceValidTokens()
    {
        foreach (var mode in Enum.GetValues<AntdThemeMode>())
        {
            var tokens = AntdTokenFactory.Create(mode, AntdSeedToken.Default);

            tokens.FontSizeBase.Should().BeGreaterThan(0);
            tokens.ControlHeightSmall.Should().BeGreaterThan(0);
            tokens.ControlHeightMiddle.Should().BeGreaterThan(tokens.ControlHeightSmall);
            tokens.ControlHeightLarge.Should().BeGreaterThan(tokens.ControlHeightMiddle);
            tokens.PaddingXs.Should().BeGreaterThanOrEqualTo(0);
        }
    }
}
