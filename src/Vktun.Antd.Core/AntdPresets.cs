namespace Vktun.Antd;

/// <summary>
/// Provides predefined theme presets and colors for Vktun.Antd.
/// </summary>
public static class AntdPresets
{
    /// <summary>
    /// Provides predefined color presets.
    /// </summary>
    public static class Colors
    {
        public static AntdColor AntdBlue { get; } = AntdColor.Parse("#1677FF");
        public static AntdColor AliyunOrange { get; } = AntdColor.Parse("#FF6A00");
        public static AntdColor WeChatGreen { get; } = AntdColor.Parse("#07C160");
        public static AntdColor GeekBlue { get; } = AntdColor.Parse("#2F54EB");
        public static AntdColor Golden { get; } = AntdColor.Parse("#D48806");
        public static AntdColor Magenta { get; } = AntdColor.Parse("#EB2F96");
        public static AntdColor Purple { get; } = AntdColor.Parse("#722ED1");
        public static AntdColor Volcano { get; } = AntdColor.Parse("#FA541C");
        public static AntdColor Cyan { get; } = AntdColor.Parse("#13C2C2");
        public static AntdColor Lime { get; } = AntdColor.Parse("#A0D911");
        public static AntdColor Success { get; } = AntdColor.Parse("#52C41A");
        public static AntdColor Warning { get; } = AntdColor.Parse("#FAAD14");
        public static AntdColor Error { get; } = AntdColor.Parse("#FF4D4F");
    }

    /// <summary>
    /// Gets the default seed token preset.
    /// </summary>
    public static AntdSeedToken Default { get; } = AntdSeedToken.Default;

    /// <summary>
    /// Gets a compact seed token preset with smaller control heights.
    /// </summary>
    public static AntdSeedToken Compact { get; } = new AntdSeedToken
    {
        PrimaryColor = Colors.AntdBlue,
        SuccessColor = Colors.Success,
        WarningColor = Colors.Warning,
        ErrorColor = Colors.Error,
        FontSizeBase = 14d,
        ControlHeightSmall = 24d,
        ControlHeightMiddle = 32d,
        ControlHeightLarge = 40d,
        BorderRadius = 6d
    };

    /// <summary>
    /// Gets a round seed token preset with larger border radius.
    /// </summary>
    public static AntdSeedToken Round { get; } = new AntdSeedToken
    {
        PrimaryColor = Colors.AntdBlue,
        SuccessColor = Colors.Success,
        WarningColor = Colors.Warning,
        ErrorColor = Colors.Error,
        FontSizeBase = 14d,
        ControlHeightSmall = 32d,
        ControlHeightMiddle = 40d,
        ControlHeightLarge = 48d,
        BorderRadius = 12d
    };

    /// <summary>
    /// Gets a sharp seed token preset with minimal border radius.
    /// </summary>
    public static AntdSeedToken Sharp { get; } = new AntdSeedToken
    {
        PrimaryColor = Colors.AntdBlue,
        SuccessColor = Colors.Success,
        WarningColor = Colors.Warning,
        ErrorColor = Colors.Error,
        FontSizeBase = 14d,
        ControlHeightSmall = 32d,
        ControlHeightMiddle = 40d,
        ControlHeightLarge = 48d,
        BorderRadius = 2d
    };

    /// <summary>
    /// Gets Aliyun theme preset with orange primary color.
    /// </summary>
    public static AntdSeedToken Aliyun { get; } = new AntdSeedToken
    {
        PrimaryColor = Colors.AliyunOrange,
        SuccessColor = Colors.Success,
        WarningColor = Colors.Warning,
        ErrorColor = Colors.Error,
        FontSizeBase = 14d,
        ControlHeightSmall = 32d,
        ControlHeightMiddle = 40d,
        ControlHeightLarge = 48d,
        BorderRadius = 8d
    };

    /// <summary>
    /// Gets WeChat theme preset with green primary color.
    /// </summary>
    public static AntdSeedToken WeChat { get; } = new AntdSeedToken
    {
        PrimaryColor = Colors.WeChatGreen,
        SuccessColor = Colors.Success,
        WarningColor = Colors.Warning,
        ErrorColor = Colors.Error,
        FontSizeBase = 14d,
        ControlHeightSmall = 32d,
        ControlHeightMiddle = 40d,
        ControlHeightLarge = 48d,
        BorderRadius = 8d
    };

    /// <summary>
    /// Gets Geek theme preset with deep blue primary color.
    /// </summary>
    public static AntdSeedToken Geek { get; } = new AntdSeedToken
    {
        PrimaryColor = Colors.GeekBlue,
        SuccessColor = Colors.Success,
        WarningColor = Colors.Warning,
        ErrorColor = Colors.Error,
        FontSizeBase = 14d,
        ControlHeightSmall = 32d,
        ControlHeightMiddle = 40d,
        ControlHeightLarge = 48d,
        BorderRadius = 8d
    };

    /// <summary>
    /// Gets Cyan theme preset with cyan primary color.
    /// </summary>
    public static AntdSeedToken Cyan { get; } = new AntdSeedToken
    {
        PrimaryColor = Colors.Cyan,
        SuccessColor = Colors.Success,
        WarningColor = Colors.Warning,
        ErrorColor = Colors.Error,
        FontSizeBase = 14d,
        ControlHeightSmall = 32d,
        ControlHeightMiddle = 40d,
        ControlHeightLarge = 48d,
        BorderRadius = 8d
    };

    /// <summary>
    /// Gets Magenta theme preset with magenta primary color.
    /// </summary>
    public static AntdSeedToken Magenta { get; } = new AntdSeedToken
    {
        PrimaryColor = Colors.Magenta,
        SuccessColor = Colors.Success,
        WarningColor = Colors.Warning,
        ErrorColor = Colors.Error,
        FontSizeBase = 14d,
        ControlHeightSmall = 32d,
        ControlHeightMiddle = 40d,
        ControlHeightLarge = 48d,
        BorderRadius = 8d
    };

    /// <summary>
    /// Gets Purple theme preset with purple primary color.
    /// </summary>
    public static AntdSeedToken Purple { get; } = new AntdSeedToken
    {
        PrimaryColor = Colors.Purple,
        SuccessColor = Colors.Success,
        WarningColor = Colors.Warning,
        ErrorColor = Colors.Error,
        FontSizeBase = 14d,
        ControlHeightSmall = 32d,
        ControlHeightMiddle = 40d,
        ControlHeightLarge = 48d,
        BorderRadius = 8d
    };
}

/// <summary>
/// Represents a complete theme configuration with mode and seed token.
/// </summary>
/// <param name="Mode">The theme mode.</param>
/// <param name="Seed">The seed token.</param>
public record AntdThemeConfiguration(AntdThemeMode Mode, AntdSeedToken Seed);

/// <summary>
/// Provides predefined complete theme configurations.
/// </summary>
public static class AntdThemePresets
{
    /// <summary>
    /// Gets the default light theme configuration.
    /// </summary>
    public static AntdThemeConfiguration LightDefault { get; } = new(AntdThemeMode.Light, AntdPresets.Default);

    /// <summary>
    /// Gets the default dark theme configuration.
    /// </summary>
    public static AntdThemeConfiguration DarkDefault { get; } = new(AntdThemeMode.Dark, AntdPresets.Default);

    /// <summary>
    /// Gets the compact light theme configuration.
    /// </summary>
    public static AntdThemeConfiguration CompactLight { get; } = new(AntdThemeMode.Compact, AntdPresets.Compact);

    /// <summary>
    /// Gets the Aliyun light theme configuration.
    /// </summary>
    public static AntdThemeConfiguration AliyunLight { get; } = new(AntdThemeMode.Light, AntdPresets.Aliyun);

    /// <summary>
    /// Gets the WeChat light theme configuration.
    /// </summary>
    public static AntdThemeConfiguration WeChatLight { get; } = new(AntdThemeMode.Light, AntdPresets.WeChat);

    /// <summary>
    /// Gets the Geek light theme configuration.
    /// </summary>
    public static AntdThemeConfiguration GeekLight { get; } = new(AntdThemeMode.Light, AntdPresets.Geek);

    /// <summary>
    /// Gets the Cyan light theme configuration.
    /// </summary>
    public static AntdThemeConfiguration CyanLight { get; } = new(AntdThemeMode.Light, AntdPresets.Cyan);
}
