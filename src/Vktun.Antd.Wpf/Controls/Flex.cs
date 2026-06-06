using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Vktun.Antd.Wpf;

/// <summary>
/// Represents a lightweight flex-style panel with wrapping, gap, alignment and justification support.
/// </summary>
public class Flex : Panel
{
    /// <summary>
    /// Identifies the <see cref="Orientation"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(Flex),
            new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// Identifies the <see cref="Gap"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GapProperty =
        DependencyProperty.Register(nameof(Gap), typeof(double), typeof(Flex),
            new FrameworkPropertyMetadata(12d, FrameworkPropertyMetadataOptions.AffectsMeasure, null, CoerceNonNegative));

    /// <summary>
    /// Identifies the <see cref="Wrap"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WrapProperty =
        DependencyProperty.Register(nameof(Wrap), typeof(bool), typeof(Flex),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// Identifies the <see cref="Justify"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty JustifyProperty =
        DependencyProperty.Register(nameof(Justify), typeof(HorizontalAlignment), typeof(Flex),
            new FrameworkPropertyMetadata(HorizontalAlignment.Left, FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>
    /// Identifies the <see cref="Align"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlignProperty =
        DependencyProperty.Register(nameof(Align), typeof(VerticalAlignment), typeof(Flex),
            new FrameworkPropertyMetadata(VerticalAlignment.Top, FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>
    /// Gets or sets the primary flow direction.
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the gap between items.
    /// </summary>
    public double Gap
    {
        get => (double)GetValue(GapProperty);
        set => SetValue(GapProperty, value);
    }

    /// <summary>
    /// Gets or sets whether items should wrap when space is insufficient.
    /// </summary>
    public bool Wrap
    {
        get => (bool)GetValue(WrapProperty);
        set => SetValue(WrapProperty, value);
    }

    /// <summary>
    /// Gets or sets how items are distributed along the main axis.
    /// </summary>
    public HorizontalAlignment Justify
    {
        get => (HorizontalAlignment)GetValue(JustifyProperty);
        set => SetValue(JustifyProperty, value);
    }

    /// <summary>
    /// Gets or sets how items align along the cross axis.
    /// </summary>
    public VerticalAlignment Align
    {
        get => (VerticalAlignment)GetValue(AlignProperty);
        set => SetValue(AlignProperty, value);
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
    {
        var lines = MeasureLines(availableSize);
        return ComposeDesiredSize(lines);
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        var lines = BuildLines(finalSize);
        var gap = Math.Max(0d, Gap);
        var crossCursor = 0d;
        var isHorizontal = Orientation == Orientation.Horizontal;

        foreach (var line in lines)
        {
            var availableFlow = isHorizontal ? finalSize.Width : finalSize.Height;
            var extraFlow = double.IsInfinity(availableFlow) ? 0d : Math.Max(0d, availableFlow - line.FlowLength);
            var startFlow = GetFlowStartOffset(extraFlow);
            var flowGap = gap;

            if (Justify == HorizontalAlignment.Stretch && line.Children.Count > 1 && !double.IsInfinity(availableFlow))
            {
                flowGap += extraFlow / (line.Children.Count - 1);
                startFlow = 0d;
            }

            var flowCursor = startFlow;
            foreach (var child in line.Children)
            {
                var desired = child.DesiredSize;
                var childFlow = isHorizontal ? desired.Width : desired.Height;
                var childCross = isHorizontal ? desired.Height : desired.Width;
                if (Align == VerticalAlignment.Stretch)
                {
                    childCross = line.CrossLength;
                }

                var crossOffset = GetCrossOffset(line.CrossLength, childCross);
                var rect = isHorizontal
                    ? new Rect(flowCursor, crossCursor + crossOffset, childFlow, childCross)
                    : new Rect(crossCursor + crossOffset, flowCursor, childCross, childFlow);

                child.Arrange(rect);
                flowCursor += childFlow + flowGap;
            }

            crossCursor += line.CrossLength + gap;
        }

        return finalSize;
    }

    private List<FlexLine> MeasureLines(Size availableSize)
    {
        var isHorizontal = Orientation == Orientation.Horizontal;
        foreach (UIElement child in InternalChildren)
        {
            child.Measure(isHorizontal
                ? new Size(double.PositiveInfinity, availableSize.Height)
                : new Size(availableSize.Width, double.PositiveInfinity));
        }

        return BuildLines(availableSize);
    }

    private List<FlexLine> BuildLines(Size availableSize)
    {
        var lines = new List<FlexLine>();
        var gap = Math.Max(0d, Gap);
        var isHorizontal = Orientation == Orientation.Horizontal;
        var limit = isHorizontal ? availableSize.Width : availableSize.Height;
        var canWrap = Wrap && !double.IsInfinity(limit);

        var current = new FlexLine();
        foreach (UIElement child in InternalChildren)
        {
            var desired = child.DesiredSize;
            var childFlow = isHorizontal ? desired.Width : desired.Height;
            var childCross = isHorizontal ? desired.Height : desired.Width;
            var nextFlow = current.Children.Count == 0 ? childFlow : current.FlowLength + gap + childFlow;

            if (canWrap && current.Children.Count > 0 && nextFlow > limit)
            {
                lines.Add(current);
                current = new FlexLine();
                nextFlow = childFlow;
            }

            current.Children.Add(child);
            current.FlowLength = nextFlow;
            current.CrossLength = Math.Max(current.CrossLength, childCross);
        }

        if (current.Children.Count > 0)
        {
            lines.Add(current);
        }

        return lines;
    }

    private Size ComposeDesiredSize(IReadOnlyList<FlexLine> lines)
    {
        var gap = Math.Max(0d, Gap);
        if (lines.Count == 0)
        {
            return new Size();
        }

        double maxFlow = 0d;
        double totalCross = 0d;
        for (var index = 0; index < lines.Count; index++)
        {
            maxFlow = Math.Max(maxFlow, lines[index].FlowLength);
            totalCross += lines[index].CrossLength;
            if (index < lines.Count - 1)
            {
                totalCross += gap;
            }
        }

        return Orientation == Orientation.Horizontal
            ? new Size(maxFlow, totalCross)
            : new Size(totalCross, maxFlow);
    }

    private static object CoerceNonNegative(DependencyObject dependencyObject, object baseValue)
    {
        return baseValue is double value && value > 0d ? value : 0d;
    }

    private double GetFlowStartOffset(double extraFlow)
    {
        return Justify switch
        {
            HorizontalAlignment.Center => extraFlow / 2d,
            HorizontalAlignment.Right => extraFlow,
            _ => 0d,
        };
    }

    private double GetCrossOffset(double availableCross, double childCross)
    {
        var extraCross = Math.Max(0d, availableCross - childCross);
        return Align switch
        {
            VerticalAlignment.Center => extraCross / 2d,
            VerticalAlignment.Bottom => extraCross,
            _ => 0d,
        };
    }

    private sealed class FlexLine
    {
        public List<UIElement> Children { get; } = [];

        public double FlowLength { get; set; }

        public double CrossLength { get; set; }
    }
}
