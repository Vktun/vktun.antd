using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Vktun.Antd.Wpf;

/// <summary>
/// Represents a row container in the Ant Design grid system.
/// </summary>
public class Row : Panel
{
    static Row()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Row), new FrameworkPropertyMetadata(typeof(Row)));
    }

    /// <summary>
    /// Gets or sets the gutter spacing between columns.
    /// </summary>
    public double Gutter
    {
        get => (double)GetValue(GutterProperty);
        set => SetValue(GutterProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Gutter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GutterProperty =
        DependencyProperty.Register(nameof(Gutter), typeof(double), typeof(Row),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure, null, CoerceNonNegative));

    /// <summary>
    /// Gets or sets whether the row should wrap to next line.
    /// </summary>
    public bool Wrap
    {
        get => (bool)GetValue(WrapProperty);
        set => SetValue(WrapProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Wrap"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WrapProperty =
        DependencyProperty.Register(nameof(Wrap), typeof(bool), typeof(Row),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// Gets or sets the horizontal alignment of columns.
    /// </summary>
    public HorizontalAlignment Justify
    {
        get => (HorizontalAlignment)GetValue(JustifyProperty);
        set => SetValue(JustifyProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Justify"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty JustifyProperty =
        DependencyProperty.Register(nameof(Justify), typeof(HorizontalAlignment), typeof(Row),
            new FrameworkPropertyMetadata(HorizontalAlignment.Left, FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>
    /// Gets or sets the vertical alignment of columns.
    /// </summary>
    public VerticalAlignment Align
    {
        get => (VerticalAlignment)GetValue(AlignProperty);
        set => SetValue(AlignProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Align"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlignProperty =
        DependencyProperty.Register(nameof(Align), typeof(VerticalAlignment), typeof(Row),
            new FrameworkPropertyMetadata(VerticalAlignment.Top, FrameworkPropertyMetadataOptions.AffectsArrange));

    protected override Size MeasureOverride(Size availableSize)
    {
        var lines = BuildLines(availableSize, measureChildren: true);
        var desiredHeight = 0d;
        foreach (var line in lines)
        {
            desiredHeight += line.Height;
        }

        return new Size(double.IsInfinity(availableSize.Width) ? GetMaxLineWidth(lines) : availableSize.Width, desiredHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var lines = BuildLines(finalSize, measureChildren: false);
        var y = 0d;
        foreach (var line in lines)
        {
            var gap = Gutter;
            var availableExtra = Math.Max(0d, finalSize.Width - line.Width);
            var x = GetLineStart(availableExtra);

            if (Justify == HorizontalAlignment.Stretch && line.Items.Count > 1)
            {
                gap += availableExtra / (line.Items.Count - 1);
                x = 0d;
            }

            foreach (var item in line.Items)
            {
                var childHeight = Align == VerticalAlignment.Stretch ? line.Height : item.Child.DesiredSize.Height;
                var childY = y + GetChildTop(line.Height, childHeight);
                item.Child.Arrange(new Rect(x + item.OffsetWidth, childY, item.Width, childHeight));
                x += item.OffsetWidth + item.Width + gap;
            }

            y += line.Height;
        }

        return finalSize;
    }

    private List<RowLine> BuildLines(Size availableSize, bool measureChildren)
    {
        var lines = new List<RowLine>();
        var containerWidth = double.IsInfinity(availableSize.Width) ? 0d : Math.Max(0d, availableSize.Width);
        var unitWidth = containerWidth / 24d;
        var gutter = Gutter;
        var current = new RowLine();

        foreach (UIElement child in InternalChildren)
        {
            if (child is not Col col)
            {
                continue;
            }

            var width = unitWidth * GetEffectiveSpan(col, containerWidth);
            var offsetWidth = unitWidth * GetEffectiveOffset(col, containerWidth);
            if (measureChildren)
            {
                child.Measure(new Size(width, availableSize.Height));
            }

            var itemWidth = offsetWidth + width;
            var nextWidth = current.Items.Count == 0 ? itemWidth : current.Width + gutter + itemWidth;
            if (Wrap && containerWidth > 0d && current.Items.Count > 0 && nextWidth > containerWidth)
            {
                lines.Add(current);
                current = new RowLine();
                nextWidth = itemWidth;
            }

            current.Items.Add(new RowItem(child, offsetWidth, width));
            current.Width = nextWidth;
            current.Height = Math.Max(current.Height, child.DesiredSize.Height);
        }

        if (current.Items.Count > 0)
        {
            lines.Add(current);
        }

        return lines;
    }

    private double GetLineStart(double availableExtra)
    {
        return Justify switch
        {
            HorizontalAlignment.Center => availableExtra / 2d,
            HorizontalAlignment.Right => availableExtra,
            _ => 0d,
        };
    }

    private double GetChildTop(double lineHeight, double childHeight)
    {
        var extra = Math.Max(0d, lineHeight - childHeight);
        return Align switch
        {
            VerticalAlignment.Center => extra / 2d,
            VerticalAlignment.Bottom => extra,
            _ => 0d,
        };
    }

    private static double GetMaxLineWidth(IReadOnlyList<RowLine> lines)
    {
        var maxWidth = 0d;
        foreach (var line in lines)
        {
            maxWidth = Math.Max(maxWidth, line.Width);
        }

        return maxWidth;
    }

    private int GetEffectiveSpan(Col col, double containerWidth)
    {
        if (containerWidth >= 1600 && col.XxlSpan > 0) return col.XxlSpan;
        if (containerWidth >= 1200 && col.XlSpan > 0) return col.XlSpan;
        if (containerWidth < 576 && col.XsSpan > 0) return col.XsSpan;
        if (containerWidth < 768 && col.SmSpan > 0) return col.SmSpan;
        if (containerWidth < 992 && col.MdSpan > 0) return col.MdSpan;
        if (containerWidth < 1200 && col.LgSpan > 0) return col.LgSpan;

        return col.Span;
    }

    private int GetEffectiveOffset(Col col, double containerWidth)
    {
        if (containerWidth >= 1600 && col.XxlOffset > 0) return col.XxlOffset;
        if (containerWidth >= 1200 && col.XlOffset > 0) return col.XlOffset;
        if (containerWidth < 576 && col.XsOffset > 0) return col.XsOffset;
        if (containerWidth < 768 && col.SmOffset > 0) return col.SmOffset;
        if (containerWidth < 992 && col.MdOffset > 0) return col.MdOffset;
        if (containerWidth < 1200 && col.LgOffset > 0) return col.LgOffset;

        return col.Offset;
    }

    private static object CoerceNonNegative(DependencyObject dependencyObject, object baseValue)
    {
        return baseValue is double value && value > 0d ? value : 0d;
    }

    private sealed class RowLine
    {
        public List<RowItem> Items { get; } = [];

        public double Width { get; set; }

        public double Height { get; set; }
    }

    private sealed record RowItem(UIElement Child, double OffsetWidth, double Width);
}

/// <summary>
/// Represents a column in the Ant Design grid system.
/// </summary>
public class Col : ContentControl
{
    static Col()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Col), new FrameworkPropertyMetadata(typeof(Col)));
    }

    /// <summary>
    /// Gets or sets the number of columns to span (1-24).
    /// </summary>
    public int Span
    {
        get => (int)GetValue(SpanProperty);
        set => SetValue(SpanProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Span"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SpanProperty =
        DependencyProperty.Register(nameof(Span), typeof(int), typeof(Col),
            new FrameworkPropertyMetadata(24, FrameworkPropertyMetadataOptions.AffectsParentMeasure, null, CoerceSpan));

    /// <summary>
    /// Gets or sets the number of columns to offset.
    /// </summary>
    public int Offset
    {
        get => (int)GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Offset"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OffsetProperty =
        DependencyProperty.Register(nameof(Offset), typeof(int), typeof(Col),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure, null, CoerceOffset));

    /// <summary>
    /// Gets or sets the span for extra small screens (&lt;576px).
    /// </summary>
    public int XsSpan
    {
        get => (int)GetValue(XsSpanProperty);
        set => SetValue(XsSpanProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="XsSpan"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty XsSpanProperty =
        DependencyProperty.Register(nameof(XsSpan), typeof(int), typeof(Col),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure, null, CoerceOptionalSpan));

    /// <summary>
    /// Gets or sets the offset for extra small screens (&lt;576px).
    /// </summary>
    public int XsOffset
    {
        get => (int)GetValue(XsOffsetProperty);
        set => SetValue(XsOffsetProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="XsOffset"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty XsOffsetProperty =
        DependencyProperty.Register(nameof(XsOffset), typeof(int), typeof(Col),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure, null, CoerceOffset));

    /// <summary>
    /// Gets or sets the span for small screens (≥576px).
    /// </summary>
    public int SmSpan
    {
        get => (int)GetValue(SmSpanProperty);
        set => SetValue(SmSpanProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="SmSpan"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SmSpanProperty =
        DependencyProperty.Register(nameof(SmSpan), typeof(int), typeof(Col),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure, null, CoerceOptionalSpan));

    /// <summary>
    /// Gets or sets the offset for small screens (≥576px).
    /// </summary>
    public int SmOffset
    {
        get => (int)GetValue(SmOffsetProperty);
        set => SetValue(SmOffsetProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="SmOffset"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SmOffsetProperty =
        DependencyProperty.Register(nameof(SmOffset), typeof(int), typeof(Col),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure, null, CoerceOffset));

    /// <summary>
    /// Gets or sets the span for medium screens (≥768px).
    /// </summary>
    public int MdSpan
    {
        get => (int)GetValue(MdSpanProperty);
        set => SetValue(MdSpanProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MdSpan"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MdSpanProperty =
        DependencyProperty.Register(nameof(MdSpan), typeof(int), typeof(Col),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure, null, CoerceOptionalSpan));

    /// <summary>
    /// Gets or sets the offset for medium screens (≥768px).
    /// </summary>
    public int MdOffset
    {
        get => (int)GetValue(MdOffsetProperty);
        set => SetValue(MdOffsetProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MdOffset"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MdOffsetProperty =
        DependencyProperty.Register(nameof(MdOffset), typeof(int), typeof(Col),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure, null, CoerceOffset));

    /// <summary>
    /// Gets or sets the span for large screens (≥992px).
    /// </summary>
    public int LgSpan
    {
        get => (int)GetValue(LgSpanProperty);
        set => SetValue(LgSpanProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="LgSpan"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LgSpanProperty =
        DependencyProperty.Register(nameof(LgSpan), typeof(int), typeof(Col),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure, null, CoerceOptionalSpan));

    /// <summary>
    /// Gets or sets the offset for large screens (≥992px).
    /// </summary>
    public int LgOffset
    {
        get => (int)GetValue(LgOffsetProperty);
        set => SetValue(LgOffsetProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="LgOffset"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LgOffsetProperty =
        DependencyProperty.Register(nameof(LgOffset), typeof(int), typeof(Col),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure, null, CoerceOffset));

    /// <summary>
    /// Gets or sets the span for extra large screens (≥1200px).
    /// </summary>
    public int XlSpan
    {
        get => (int)GetValue(XlSpanProperty);
        set => SetValue(XlSpanProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="XlSpan"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty XlSpanProperty =
        DependencyProperty.Register(nameof(XlSpan), typeof(int), typeof(Col),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure, null, CoerceOptionalSpan));

    /// <summary>
    /// Gets or sets the offset for extra large screens (≥1200px).
    /// </summary>
    public int XlOffset
    {
        get => (int)GetValue(XlOffsetProperty);
        set => SetValue(XlOffsetProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="XlOffset"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty XlOffsetProperty =
        DependencyProperty.Register(nameof(XlOffset), typeof(int), typeof(Col),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure, null, CoerceOffset));

    /// <summary>
    /// Gets or sets the span for extra extra large screens (≥1600px).
    /// </summary>
    public int XxlSpan
    {
        get => (int)GetValue(XxlSpanProperty);
        set => SetValue(XxlSpanProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="XxlSpan"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty XxlSpanProperty =
        DependencyProperty.Register(nameof(XxlSpan), typeof(int), typeof(Col),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure, null, CoerceOptionalSpan));

    /// <summary>
    /// Gets or sets the offset for extra extra large screens (≥1600px).
    /// </summary>
    public int XxlOffset
    {
        get => (int)GetValue(XxlOffsetProperty);
        set => SetValue(XxlOffsetProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="XxlOffset"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty XxlOffsetProperty =
        DependencyProperty.Register(nameof(XxlOffset), typeof(int), typeof(Col),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure, null, CoerceOffset));

    private static object CoerceSpan(DependencyObject dependencyObject, object baseValue)
    {
        return baseValue is int value ? Math.Clamp(value, 1, 24) : 24;
    }

    private static object CoerceOptionalSpan(DependencyObject dependencyObject, object baseValue)
    {
        return baseValue is int value ? Math.Clamp(value, 0, 24) : 0;
    }

    private static object CoerceOffset(DependencyObject dependencyObject, object baseValue)
    {
        return baseValue is int value ? Math.Clamp(value, 0, 24) : 0;
    }
}
