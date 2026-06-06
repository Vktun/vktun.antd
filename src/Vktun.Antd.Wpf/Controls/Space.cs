using System.Windows;
using System.Windows.Controls;

namespace Vktun.Antd.Wpf;

/// <summary>
/// Represents an Ant Design styled space layout component for consistent spacing between elements.
/// </summary>
public class Space : StackPanel
{
    static Space()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Space),
            new FrameworkPropertyMetadata(typeof(Space)));
    }

    /// <summary>
    /// Identifies the <see cref="Gap"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GapProperty =
        DependencyProperty.Register(nameof(Gap), typeof(double), typeof(Space),
            new PropertyMetadata(8d, OnGapChanged, CoerceNonNegative));

    /// <summary>
    /// Identifies the <see cref="Wrap"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WrapProperty =
        DependencyProperty.Register(nameof(Wrap), typeof(bool), typeof(Space),
            new PropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="Align"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlignProperty =
        DependencyProperty.Register(nameof(Align), typeof(HorizontalAlignment), typeof(Space),
            new PropertyMetadata(HorizontalAlignment.Left));

    /// <summary>
    /// Gets or sets the gap between items.
    /// </summary>
    public double Gap
    {
        get => (double)GetValue(GapProperty);
        set => SetValue(GapProperty, value);
    }

    /// <summary>
    /// Gets or sets whether items should wrap when space is limited. Use <see cref="Flex" /> for wrapping layouts.
    /// </summary>
    public bool Wrap
    {
        get => (bool)GetValue(WrapProperty);
        set => SetValue(WrapProperty, value);
    }

    /// <summary>
    /// Gets or sets the alignment of items. Use <see cref="Flex" /> for cross-axis alignment.
    /// </summary>
    public HorizontalAlignment Align
    {
        get => (HorizontalAlignment)GetValue(AlignProperty);
        set => SetValue(AlignProperty, value);
    }

    private static void OnGapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Space space)
        {
            space.UpdateSpacing();
        }
    }

    private static object CoerceNonNegative(DependencyObject dependencyObject, object baseValue)
    {
        return baseValue is double value && value > 0d ? value : 0d;
    }

    /// <summary>
    /// Updates the spacing for all child elements.
    /// </summary>
    public void UpdateSpacing()
    {
        var gap = Gap;
        for (var index = 0; index < Children.Count; index++)
        {
            if (Children[index] is not FrameworkElement child)
            {
                continue;
            }

            child.Margin = Orientation == Orientation.Horizontal
                ? new Thickness(0d, 0d, index == Children.Count - 1 ? 0d : gap, 0d)
                : new Thickness(0d, 0d, 0d, index == Children.Count - 1 ? 0d : gap);
        }
    }

    protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
    {
        base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        UpdateSpacing();
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == OrientationProperty)
        {
            UpdateSpacing();
        }
    }
}
