using System;

namespace Vktun.Antd;

/// <summary>
/// Defines the seed values used to derive semantic theme resources.
/// </summary>
public sealed class AntdSeedToken : IEquatable<AntdSeedToken>
{
    /// <summary>
    /// Gets or sets the primary brand color.
    /// </summary>
    public AntdColor PrimaryColor { get; init; } = AntdColor.Parse("#1677FF");

    /// <summary>
    /// Gets or sets the success color.
    /// </summary>
    public AntdColor SuccessColor { get; init; } = AntdColor.Parse("#52C41A");

    /// <summary>
    /// Gets or sets the warning color.
    /// </summary>
    public AntdColor WarningColor { get; init; } = AntdColor.Parse("#FAAD14");

    /// <summary>
    /// Gets or sets the error color.
    /// </summary>
    public AntdColor ErrorColor { get; init; } = AntdColor.Parse("#FF4D4F");

    /// <summary>
    /// Gets or sets the base font size.
    /// </summary>
    public double FontSizeBase { get; init; } = 14d;

    /// <summary>
    /// Gets or sets the small control height.
    /// </summary>
    public double ControlHeightSmall { get; init; } = 32d;

    /// <summary>
    /// Gets or sets the default control height.
    /// </summary>
    public double ControlHeightMiddle { get; init; } = 40d;

    /// <summary>
    /// Gets or sets the large control height.
    /// </summary>
    public double ControlHeightLarge { get; init; } = 48d;

    /// <summary>
    /// Gets or sets the default radius.
    /// </summary>
    public double BorderRadius { get; init; } = 8d;

    /// <summary>
    /// Gets a default token set.
    /// </summary>
    public static AntdSeedToken Default { get; } = new();

    /// <inheritdoc />
    public bool Equals(AntdSeedToken? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return PrimaryColor == other.PrimaryColor
            && SuccessColor == other.SuccessColor
            && WarningColor == other.WarningColor
            && ErrorColor == other.ErrorColor
            && FontSizeBase.Equals(other.FontSizeBase)
            && ControlHeightSmall.Equals(other.ControlHeightSmall)
            && ControlHeightMiddle.Equals(other.ControlHeightMiddle)
            && ControlHeightLarge.Equals(other.ControlHeightLarge)
            && BorderRadius.Equals(other.BorderRadius);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as AntdSeedToken);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(PrimaryColor);
        hash.Add(SuccessColor);
        hash.Add(WarningColor);
        hash.Add(ErrorColor);
        hash.Add(FontSizeBase);
        hash.Add(ControlHeightSmall);
        hash.Add(ControlHeightMiddle);
        hash.Add(ControlHeightLarge);
        hash.Add(BorderRadius);
        return hash.ToHashCode();
    }
}
