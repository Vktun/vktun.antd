using System;
using System.Collections.Concurrent;

namespace Vktun.Antd;

/// <summary>
/// Creates resolved theme tokens from a seed token and theme mode.
/// </summary>
public static class AntdTokenFactory
{
    private static readonly AntdColor Black = AntdColor.FromRgb(0, 0, 0);
    private static readonly AntdColor White = AntdColor.FromRgb(255, 255, 255);
    private static readonly AntdColor Transparent = AntdColor.FromArgb(0, 255, 255, 255);

    private const double DarkHoverBlendRatio = 0.12d;
    private const double LightHoverBlendRatio = 0.2d;
    private const double DarkActiveBlendRatio = 0.18d;
    private const double LightActiveBlendRatio = 0.12d;
    private const double DarkSpotlightBlendRatio = 0.45d;
    private const double LightSpotlightBlendRatio = 0.9d;
    private const double DarkFocusOutlineBlendRatio = 0.18d;
    private const double LightFocusOutlineBlendRatio = 0.4d;
    private const double DarkMaskOpacity = 0.55d;
    private const double LightMaskOpacity = 0.35d;
    private const double DarkTagBgBlendRatio = 0.08d;
    private const double LightTagBgBlendRatio = 0.92d;
    private const double CompactHeightOffset = 4d;
    private const double CompactPaddingOffset = 2d;

    private static readonly ConcurrentDictionary<(AntdThemeMode, AntdSeedToken), AntdTokenSet> _cache = new();

    /// <summary>
    /// Creates a resolved token set. Results are cached by mode and seed token.
    /// </summary>
    /// <param name="mode">The theme mode.</param>
    /// <param name="seed">The seed token.</param>
    /// <returns>The resolved token set.</returns>
    public static AntdTokenSet Create(AntdThemeMode mode, AntdSeedToken seed)
    {
        ArgumentNullException.ThrowIfNull(seed);
        ValidateSeed(seed);
        return _cache.GetOrAdd((mode, seed), static key => CreateCore(key.Item1, key.Item2));
    }

    private static void ValidateSeed(AntdSeedToken seed)
    {
        if (seed.FontSizeBase <= 0)
            throw new ArgumentOutOfRangeException(nameof(seed), "FontSizeBase must be positive.");
        if (seed.ControlHeightSmall <= 0 || seed.ControlHeightMiddle <= 0 || seed.ControlHeightLarge <= 0)
            throw new ArgumentOutOfRangeException(nameof(seed), "Control heights must be positive.");
        if (seed.ControlHeightSmall >= seed.ControlHeightMiddle || seed.ControlHeightMiddle >= seed.ControlHeightLarge)
            throw new ArgumentOutOfRangeException(nameof(seed), "Control heights must satisfy Small < Middle < Large.");
        if (seed.BorderRadius < 0)
            throw new ArgumentOutOfRangeException(nameof(seed), "BorderRadius must be non-negative.");
    }

    /// <summary>
    /// Clears the token cache.
    /// </summary>
    public static void ClearCache() => _cache.Clear();

    private static AntdTokenSet CreateCore(AntdThemeMode mode, AntdSeedToken seed)
    {
        var alias = CreateAlias(mode, seed);
        var component = CreateComponent(seed, mode);

        return new AntdTokenSet
        {
            ColorPrimary = seed.PrimaryColor,
            ColorPrimaryHover = AntdColorMath.Blend(seed.PrimaryColor, White, mode == AntdThemeMode.Dark ? DarkHoverBlendRatio : LightHoverBlendRatio),
            ColorPrimaryActive = AntdColorMath.Blend(seed.PrimaryColor, Black, mode == AntdThemeMode.Dark ? DarkActiveBlendRatio : LightActiveBlendRatio),
            ColorSuccess = seed.SuccessColor,
            ColorWarning = seed.WarningColor,
            ColorError = seed.ErrorColor,
            ColorText = alias.ColorText,
            ColorTextSecondary = alias.ColorTextSecondary,
            ColorTextTertiary = alias.ColorTextTertiary,
            ColorLink = seed.PrimaryColor,
            ColorBgBase = alias.ColorBgBase,
            ColorBgLayout = alias.ColorBgLayout,
            ColorBgContainer = alias.ColorBgContainer,
            ColorBgElevated = alias.ColorBgElevated,
            ColorBgSpotlight = alias.ColorBgSpotlight,
            ColorFillSecondary = alias.ColorFillSecondary,
            ColorFillTertiary = alias.ColorFillTertiary,
            ColorFillQuaternary = alias.ColorFillQuaternary,
            ColorBorder = alias.ColorBorder,
            ColorBorderSecondary = alias.ColorBorderSecondary,
            ColorFocusOutline = alias.ColorFocusOutline,
            ColorMask = alias.ColorMask,
            ColorTagDefaultBackground = mode == AntdThemeMode.Dark
                ? AntdColorMath.Blend(alias.ColorBgContainer, White, DarkTagBgBlendRatio)
                : AntdColorMath.Blend(seed.PrimaryColor, White, LightTagBgBlendRatio),
            ColorTagDefaultForeground = mode == AntdThemeMode.Dark ? alias.ColorText : seed.PrimaryColor,
            ColorBadgeBackground = seed.ErrorColor,
            ColorWhite = White,
            ColorTransparent = Transparent,
            FontSizeBase = seed.FontSizeBase,
            FontSizeSmall = component.FontSizeSmall,
            FontSizeLarge = component.FontSizeLarge,
            ControlHeightSmall = mode == AntdThemeMode.Compact ? seed.ControlHeightSmall - CompactHeightOffset : seed.ControlHeightSmall,
            ControlHeightMiddle = mode == AntdThemeMode.Compact ? seed.ControlHeightMiddle - CompactHeightOffset : seed.ControlHeightMiddle,
            ControlHeightLarge = mode == AntdThemeMode.Compact ? seed.ControlHeightLarge - CompactHeightOffset : seed.ControlHeightLarge,
            BorderRadiusSmall = component.BorderRadiusSmall,
            BorderRadiusMiddle = component.BorderRadiusMiddle,
            BorderRadiusLarge = component.BorderRadiusLarge,
            PaddingXs = component.PaddingXs,
            PaddingSm = component.PaddingSm,
            PaddingMd = component.PaddingMd,
            PaddingLg = component.PaddingLg,
        };
    }

    private static AntdAliasToken CreateAlias(AntdThemeMode mode, AntdSeedToken seed)
    {
        // Compact mode uses Light color scheme with tighter spacing (handled in CreateComponent)
        return mode switch
        {
            AntdThemeMode.Dark => new AntdAliasToken(
                ColorText: AntdColor.Parse("#F3F5F7"),
                ColorTextSecondary: AntdColor.Parse("#C5CDD5"),
                ColorTextTertiary: AntdColor.Parse("#95A0AD"),
                ColorBgBase: AntdColor.Parse("#111A22"),
                ColorBgLayout: AntdColor.Parse("#0B1118"),
                ColorBgContainer: AntdColor.Parse("#16202B"),
                ColorBgElevated: AntdColor.Parse("#1C2936"),
                ColorBgSpotlight: AntdColorMath.Blend(seed.PrimaryColor, Black, DarkSpotlightBlendRatio),
                ColorFillSecondary: AntdColor.Parse("#243241"),
                ColorFillTertiary: AntdColor.Parse("#21303E"),
                ColorFillQuaternary: AntdColor.Parse("#1B2733"),
                ColorBorder: AntdColor.Parse("#304050"),
                ColorBorderSecondary: AntdColor.Parse("#22303D"),
                ColorMask: AntdColorMath.WithOpacity(Black, DarkMaskOpacity),
                ColorFocusOutline: AntdColorMath.Blend(seed.PrimaryColor, White, DarkFocusOutlineBlendRatio)),
            AntdThemeMode.Light or AntdThemeMode.Compact => new AntdAliasToken(
                ColorText: AntdColor.Parse("#1F2329"),
                ColorTextSecondary: AntdColor.Parse("#5B6470"),
                ColorTextTertiary: AntdColor.Parse("#88919D"),
                ColorBgBase: White,
                ColorBgLayout: AntdColor.Parse("#F5F7FA"),
                ColorBgContainer: White,
                ColorBgElevated: White,
                ColorBgSpotlight: AntdColorMath.Blend(seed.PrimaryColor, White, LightSpotlightBlendRatio),
                ColorFillSecondary: AntdColor.Parse("#F0F4F8"),
                ColorFillTertiary: AntdColor.Parse("#E8EDF3"),
                ColorFillQuaternary: AntdColor.Parse("#DEE5EE"),
                ColorBorder: AntdColor.Parse("#D0D7E2"),
                ColorBorderSecondary: AntdColor.Parse("#E2E8F0"),
                ColorMask: AntdColorMath.WithOpacity(Black, LightMaskOpacity),
                ColorFocusOutline: AntdColorMath.Blend(seed.PrimaryColor, White, LightFocusOutlineBlendRatio)),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unsupported theme mode."),
        };
    }

    private static AntdComponentToken CreateComponent(AntdSeedToken seed, AntdThemeMode mode)
    {
        var compactOffset = mode == AntdThemeMode.Compact ? CompactPaddingOffset : 0d;
        return new AntdComponentToken(
            BorderRadiusSmall: seed.BorderRadius - 2d,
            BorderRadiusMiddle: seed.BorderRadius,
            BorderRadiusLarge: seed.BorderRadius + 4d,
            PaddingXs: 4d,
            PaddingSm: 8d - compactOffset,
            PaddingMd: 12d - compactOffset,
            PaddingLg: 16d - compactOffset,
            FontSizeSmall: seed.FontSizeBase - 1d,
            FontSizeLarge: seed.FontSizeBase + 2d);
    }
}
