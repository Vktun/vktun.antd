using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using FluentAssertions;
using Vktun.Antd.Wpf;

namespace Vktun.Antd.Wpf.Tests;

[TestClass]
public class AntdThemeTests
{
    [TestMethod]
    public void ThemeResources_ShouldExposeCoreKeys()
    {
        WpfTestHost.Run(() =>
        {
            var resources = new AntdThemeResources();

            resources.Contains(AntdResourceKeys.BrushPrimary).Should().BeTrue();
            resources.Contains(AntdResourceKeys.BrushText).Should().BeTrue();
            resources.Contains(AntdResourceKeys.ControlHeightMiddle).Should().BeTrue();
            resources.Contains(AntdResourceKeys.ShadowPopup).Should().BeTrue();
            resources[AntdResourceKeys.BorderRadiusMiddle].Should().BeOfType<CornerRadius>();
        });
    }

    [TestMethod]
    public void ThemeManager_ShouldUpdateExistingThemeResources()
    {
        WpfTestHost.Run(() =>
        {
            Application.Current!.Resources = new ResourceDictionary();
            var themeResources = new AntdThemeResources();
            Application.Current.Resources.MergedDictionaries.Add(themeResources);

            AntdThemeManager.Current.Apply(Application.Current, AntdThemeMode.Dark, new AntdSeedToken { PrimaryColor = AntdColor.FromRgb(255, 105, 180) });

            themeResources.Theme.Should().Be(AntdThemeMode.Dark);
            ((SolidColorBrush)themeResources[AntdResourceKeys.BrushPrimary]).Color.Should().Be(Colors.HotPink);
        });
    }

    [TestMethod]
    public void GeekPreset_ShouldKeepBodyTextDistinctFromPrimaryAccent()
    {
        WpfTestHost.Run(() =>
        {
            Application.Current!.Resources = new ResourceDictionary();
            Application.Current.Resources.MergedDictionaries.Add(new AntdThemeResources());
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("/Vktun.Antd.Wpf.Sample;component/Resources/CatalogResources.xaml", UriKind.Relative),
            });

            var window = new Vktun.Antd.Wpf.Sample.MainWindow();

            try
            {
                var themePresetsField = typeof(Vktun.Antd.Wpf.Sample.MainWindow).GetField("_themePresets", BindingFlags.Instance | BindingFlags.NonPublic);
                themePresetsField.Should().NotBeNull();

                var geekPreset = ((IEnumerable)themePresetsField!.GetValue(window)!)
                    .Cast<object>()
                    .First(static preset =>
                    {
                        var kind = preset.GetType().GetProperty("Kind", BindingFlags.Instance | BindingFlags.Public);
                        return string.Equals(kind?.GetValue(preset)?.ToString(), "Geek", StringComparison.Ordinal);
                    });

                var applyPresetMethod = typeof(Vktun.Antd.Wpf.Sample.MainWindow).GetMethod("ApplyPreset", BindingFlags.Instance | BindingFlags.NonPublic);
                applyPresetMethod.Should().NotBeNull();
                applyPresetMethod!.Invoke(window, [geekPreset]);

                var bodyText = ((SolidColorBrush)Application.Current.Resources[AntdResourceKeys.BrushText]).Color;
                var secondaryText = ((SolidColorBrush)Application.Current.Resources[AntdResourceKeys.BrushTextSecondary]).Color;
                var tertiaryText = ((SolidColorBrush)Application.Current.Resources[AntdResourceKeys.BrushTextTertiary]).Color;
                var primary = ((SolidColorBrush)Application.Current.Resources[AntdResourceKeys.BrushPrimary]).Color;
                var defaultTagText = ((SolidColorBrush)Application.Current.Resources[AntdResourceKeys.BrushTagDefaultForeground]).Color;

                bodyText.Should().Be(Color.FromRgb(0xE7, 0xF5, 0xE5));
                secondaryText.Should().Be(Color.FromRgb(0xA7, 0xB9, 0xA3));
                tertiaryText.Should().Be(Color.FromRgb(0x6D, 0x81, 0x6A));
                defaultTagText.Should().Be(Color.FromRgb(0xD8, 0xEF, 0xD4));
                bodyText.Should().NotBe(primary);
                secondaryText.Should().NotBe(primary);
                tertiaryText.Should().NotBe(primary);
            }
            finally
            {
                window.Close();
            }
        });
    }

    [TestMethod]
    public void ThemeResources_ShouldLoadFromXaml()
    {
        WpfTestHost.Run(() =>
        {
            const string xaml = """
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:antd="clr-namespace:Vktun.Antd.Wpf;assembly=Vktun.Antd.Wpf">
    <ResourceDictionary.MergedDictionaries>
        <antd:AntdThemeResources Theme="Light" />
    </ResourceDictionary.MergedDictionaries>
</ResourceDictionary>
""";

            var resources = XamlReader.Parse(xaml).Should().BeOfType<ResourceDictionary>().Subject;
            resources.MergedDictionaries.Should().ContainSingle();
            resources.MergedDictionaries[0].Should().BeOfType<AntdThemeResources>();
        });
    }

    #pragma warning disable CS0618
    [TestMethod]
    public void AttachedProperties_ShouldRoundTrip()
    {
        WpfTestHost.Run(() =>
        {
            var button = new Button();
            var textBox = new TextBox();

            ButtonAssist.SetType(button, AntdButtonType.Primary);
            ControlAssist.SetSize(button, AntdControlSize.Large);
            InputAssist.SetPrefix(textBox, "@");
            InputAssist.SetSuffix(textBox, ".com");
            StatusAssist.SetStatus(textBox, AntdStatus.Success);

            ButtonAssist.GetType(button).Should().Be(AntdButtonType.Primary);
            ControlAssist.GetSize(button).Should().Be(AntdControlSize.Large);
            InputAssist.GetPrefix(textBox).Should().Be("@");
            InputAssist.GetSuffix(textBox).Should().Be(".com");
            StatusAssist.GetStatus(textBox).Should().Be(AntdStatus.Success);
        });
    }
    #pragma warning restore CS0618

    [TestMethod]
    public void ComboBoxTemplate_ShouldExposePopupPartsAndReactToThemeSwitch()
    {
        WpfTestHost.Run(() =>
        {
            Application.Current!.Resources = new ResourceDictionary();
            Application.Current.Resources.MergedDictionaries.Add(new AntdThemeResources());

            var comboBox = new ComboBox
            {
                Width = 220,
                ItemsSource = new[] { "Ocean", "Aurora", "Nebula" },
                SelectedIndex = 0,
            };

            var window = new Window
            {
                Content = comboBox,
                Width = 420,
                Height = 240,
            };

            try
            {
                window.Show();
                comboBox.ApplyTemplate();
                comboBox.IsDropDownOpen = true;
                WpfTestHost.Pump();

                var popup = comboBox.Template.FindName("PART_Popup", comboBox).Should().BeOfType<Popup>().Subject;
                var popupBorder = comboBox.Template.FindName("PART_PopupBorder", comboBox).Should().BeOfType<Border>().Subject;
                var arrow = comboBox.Template.FindName("PART_Arrow", comboBox).Should().BeOfType<Path>().Subject;
                var border = comboBox.Template.FindName("Border", comboBox).Should().BeOfType<Border>().Subject;

                // HandyControl-inspired design: Popup positions relative to Border (not entire ComboBox)
                popup.PlacementTarget.Should().BeSameAs(border);
                ((SolidColorBrush)popupBorder.Background).Color.Should().Be(((SolidColorBrush)Application.Current.Resources[AntdResourceKeys.BrushBgElevated]).Color);
                ((SolidColorBrush)arrow.Stroke).Color.Should().Be(((SolidColorBrush)Application.Current.Resources[AntdResourceKeys.BrushPrimary]).Color);

                AntdThemeManager.Current.Apply(Application.Current, AntdThemeMode.Dark);
                comboBox.IsDropDownOpen = false;
                comboBox.ApplyTemplate();
                comboBox.IsDropDownOpen = true;
                WpfTestHost.Pump();

                popupBorder = comboBox.Template.FindName("PART_PopupBorder", comboBox).Should().BeOfType<Border>().Subject;
                popupBorder.Background.Should().BeOfType<SolidColorBrush>();
                popupBorder.BorderBrush.Should().BeOfType<SolidColorBrush>();
            }
            finally
            {
                window.Close();
            }
        });
    }

    [TestMethod]
    public void ComboBoxToggleSurface_ShouldProvideHitTestAreaAndToggleDropDown()
    {
        WpfTestHost.Run(() =>
        {
            Application.Current!.Resources = new ResourceDictionary();
            Application.Current.Resources.MergedDictionaries.Add(new AntdThemeResources());

            var comboBox = new ComboBox
            {
                Width = 220,
                ItemsSource = new[] { "Ocean", "Aurora", "Nebula" },
                SelectedIndex = 0,
            };

            var window = new Window
            {
                Content = comboBox,
                Width = 420,
                Height = 240,
            };

            try
            {
                window.Show();
                comboBox.ApplyTemplate();
                WpfTestHost.Pump();

                var toggle = comboBox.Template.FindName("PART_DropDownToggle", comboBox).Should().BeOfType<ToggleBlock>().Subject;
                toggle.ApplyTemplate();
                WpfTestHost.Pump();

                VisualTreeHelper.GetChild(toggle, 0).Should().BeOfType<Border>();
                comboBox.IsDropDownOpen.Should().BeFalse();

                InvokeToggleBlockMouseDown(toggle);
                WpfTestHost.Pump();

                comboBox.IsDropDownOpen.Should().BeTrue();
                toggle.IsChecked.Should().BeTrue();

                InvokeToggleBlockMouseDown(toggle);
                WpfTestHost.Pump();

                comboBox.IsDropDownOpen.Should().BeFalse();
                toggle.IsChecked.Should().BeFalse();
            }
            finally
            {
                window.Close();
            }
        });
    }

    [TestMethod]
    public void Popconfirm_ShouldOpenOnTriggerClick_AndRaiseConfirmWhenConfirmed()
    {
        WpfTestHost.Run(() =>
        {
            InitializeThemeResources();

            var popconfirm = new Popconfirm
            {
                Title = "Confirm delete?",
                Content = new Button { Content = "Delete" },
            };

            var window = CreateWindow(popconfirm);

            try
            {
                window.Show();
                popconfirm.ApplyTemplate();
                WpfTestHost.Pump();

                popconfirm.IsOpen.Should().BeFalse();

                InvokePopconfirmPreviewMouseDown(popconfirm);
                WpfTestHost.Pump();

                popconfirm.IsOpen.Should().BeTrue();

                var confirmed = false;
                popconfirm.Confirm += (_, _) => confirmed = true;

                var confirmButton = popconfirm.Template.FindName("PART_ConfirmButton", popconfirm)
                    .Should().BeOfType<Button>()
                    .Subject;
                confirmButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, confirmButton));
                WpfTestHost.Pump();

                confirmed.Should().BeTrue();
                popconfirm.IsOpen.Should().BeFalse();
            }
            finally
            {
                window.Close();
            }
        });
    }

    [TestMethod]
    public void DatePickerTemplate_ShouldExposePopupAndCalendarPartsAndReactToThemeSwitch()
    {
        WpfTestHost.Run(() =>
        {
            Application.Current!.Resources = new ResourceDictionary();
            Application.Current.Resources.MergedDictionaries.Add(new AntdThemeResources());

            var datePicker = new DatePicker
            {
                Width = 220,
                SelectedDate = new DateTime(2026, 4, 3),
                DisplayDate = new DateTime(2026, 4, 3),
            };

            var window = new Window
            {
                Content = datePicker,
                Width = 420,
                Height = 280,
            };

            try
            {
                window.Show();
                datePicker.ApplyTemplate();
                datePicker.IsDropDownOpen = true;
                WpfTestHost.Pump();

                var popup = datePicker.Template.FindName("PART_Popup", datePicker).Should().BeOfType<Popup>().Subject;
                var popupBorder = datePicker.Template.FindName("PART_PopupBorder", datePicker).Should().BeOfType<Border>().Subject;
                var calendar = datePicker.Template.FindName("PART_Calendar", datePicker).Should().BeOfType<Calendar>().Subject;

                popup.PlacementTarget.Should().BeSameAs(datePicker);
                datePicker.IsDropDownOpen.Should().BeTrue();
                calendar.ApplyTemplate();

                var calendarItem = calendar.Template.FindName("PART_CalendarItem", calendar).Should().BeOfType<CalendarItem>().Subject;
                calendarItem.ApplyTemplate();

                var headerButton = calendarItem.Template.FindName("PART_HeaderButton", calendarItem) as Button
                    ?? FindVisualChildren<Button>(calendarItem).FirstOrDefault(button => button.Name == "PART_HeaderButton");
                var dayButton = FindVisualChildren<CalendarDayButton>(calendarItem).FirstOrDefault();
                var selectedDay = FindVisualChildren<CalendarDayButton>(calendarItem).FirstOrDefault(button => button.IsSelected);

                popupBorder.Background.Should().BeOfType<SolidColorBrush>();

                if (headerButton is not null)
                {
                    headerButton.Background.Should().BeOfType<SolidColorBrush>();
                    ((SolidColorBrush)headerButton.Background).Color.Should().Be(((SolidColorBrush)Application.Current.Resources[AntdResourceKeys.BrushFillQuaternary]).Color);
                }

                dayButton.Should().NotBeNull();
                calendar.CalendarDayButtonStyle.Should().NotBeNull();
                dayButton!.Style.Should().BeSameAs(calendar.CalendarDayButtonStyle);
                dayButton.FontSize.Should().Be((double)Application.Current.Resources[AntdResourceKeys.FontSizeSmall]);

                if (selectedDay is not null)
                {
                    selectedDay.Style.Should().BeSameAs(calendar.CalendarDayButtonStyle);
                }

                AntdThemeManager.Current.Apply(Application.Current, AntdThemeMode.Dark);
                datePicker.IsDropDownOpen = false;
                datePicker.ApplyTemplate();
                datePicker.IsDropDownOpen = true;
                WpfTestHost.Pump();

                popupBorder = datePicker.Template.FindName("PART_PopupBorder", datePicker).Should().BeOfType<Border>().Subject;
                popupBorder.Background.Should().BeOfType<SolidColorBrush>();
                popupBorder.BorderBrush.Should().BeOfType<SolidColorBrush>();
            }
            finally
            {
                window.Close();
            }
        });
    }

    [TestMethod]
    public void Services_ShouldAttachOverlayHostToWindow()
    {
        WpfTestHost.Run(() =>
        {
            Application.Current!.Resources = new ResourceDictionary();
            Application.Current.Resources.MergedDictionaries.Add(new AntdThemeResources());

            var window = new Window
            {
                Content = new Grid(),
                Width = 600,
                Height = 400,
            };

            var messageService = new MessageService();
            var notificationService = new NotificationService();
            var modalService = new ModalService();

            messageService.Show(window, "test");
            notificationService.Show(window, new NotificationRequest { Title = "title", Description = "body" });
            var modalTask = modalService.ShowAsync(window, new ModalRequest { Title = "modal", Content = "body" });

            WpfTestHost.Pump();

            var root = window.Content.Should().BeOfType<Grid>().Subject;
            root.Children.Count.Should().BeGreaterThan(1);
            OverlayHost.Get(window).Should().NotBeNull();
            modalTask.IsCompleted.Should().BeFalse();
        });
    }

    [TestMethod]
    public void FloatButton_ShouldAttachToOverlayHostWhenGlobal()
    {
        WpfTestHost.Run(() =>
        {
            Application.Current!.Resources = new ResourceDictionary();
            Application.Current.Resources.MergedDictionaries.Add(new AntdThemeResources());

            var container = new StackPanel();
            var floatButton = new FloatButton
            {
                IsGlobal = true,
            };
            container.Children.Add(floatButton);

            var window = new Window
            {
                Content = new Grid
                {
                    Children =
                    {
                        container,
                    },
                },
                Width = 600,
                Height = 400,
            };

            window.Show();
            WpfTestHost.Pump();

            OverlayHost.Get(window).Should().NotBeNull();
            container.Children.Count.Should().Be(0);
            ((Grid)window.Content).Children.Count.Should().BeGreaterThan(1);

            floatButton.IsGlobal = false;
            WpfTestHost.Pump();

            container.Children.Count.Should().Be(1);
            container.Children[0].Should().BeSameAs(floatButton);
            window.Close();
        });
    }

    [TestMethod]
    public void FloatButton_ShouldAttachToOverlayHostWhenGlobalInsideContentPresenter()
    {
        WpfTestHost.Run(() =>
        {
            Application.Current!.Resources = new ResourceDictionary();
            Application.Current.Resources.MergedDictionaries.Add(new AntdThemeResources());

            var floatButton = new FloatButton
            {
                IsGlobal = true,
            };

            var presenter = new ContentPresenter
            {
                Content = floatButton,
            };

            var window = new Window
            {
                Content = new Grid
                {
                    Children =
                    {
                        presenter,
                    },
                },
                Width = 600,
                Height = 400,
            };

            window.Show();
            WpfTestHost.Pump();

            OverlayHost.Get(window).Should().NotBeNull();
            presenter.Content.Should().BeNull();
            ((Grid)window.Content).Children.Count.Should().BeGreaterThan(1);

            floatButton.IsGlobal = false;
            WpfTestHost.Pump();

            presenter.Content.Should().BeSameAs(floatButton);
            window.Close();
        });
    }

    [TestMethod]
    public void Space_ShouldClearSpacing_WhenGapSetToZero()
    {
        WpfTestHost.Run(() =>
        {
            var first = new Button();
            var second = new Button();
            var space = new Space
            {
                Orientation = Orientation.Horizontal,
                Gap = 12,
                Children =
                {
                    first,
                    second,
                },
            };

            first.Margin.Right.Should().Be(12);
            second.Margin.Should().Be(new Thickness(0));

            space.Gap = 0;

            first.Margin.Should().Be(new Thickness(0));
            second.Margin.Should().Be(new Thickness(0));
        });
    }

    [TestMethod]
    public void Flex_ShouldCoerceNegativeGapToZero()
    {
        WpfTestHost.Run(() =>
        {
            var flex = new Flex
            {
                Gap = -8,
            };

            flex.Gap.Should().Be(0);
        });
    }

    [TestMethod]
    public void Row_ShouldCoerceGutterSpanAndOffset()
    {
        WpfTestHost.Run(() =>
        {
            var row = new Row
            {
                Gutter = -16,
            };
            var col = new Col
            {
                Span = 40,
                Offset = -2,
                XsSpan = -1,
                MdSpan = 32,
                XxlOffset = 40,
            };

            row.Gutter.Should().Be(0);
            col.Span.Should().Be(24);
            col.Offset.Should().Be(0);
            col.XsSpan.Should().Be(0);
            col.MdSpan.Should().Be(24);
            col.XxlOffset.Should().Be(24);
        });
    }

    [TestMethod]
    public void Row_ShouldApplyJustifyAndAlign()
    {
        WpfTestHost.Run(() =>
        {
            var first = new Col
            {
                Span = 6,
                Content = new Border { Height = 40 },
            };
            var second = new Col
            {
                Span = 6,
                Content = new Border { Height = 20 },
            };
            var row = new Row
            {
                Width = 240,
                Justify = HorizontalAlignment.Center,
                Align = VerticalAlignment.Bottom,
                Children =
                {
                    first,
                    second,
                },
            };
            var window = CreateWindow(row, width: 320, height: 160);

            try
            {
                window.Show();
                row.UpdateLayout();
                WpfTestHost.Pump();

                var firstPosition = first.TranslatePoint(new Point(0, 0), row);
                var secondPosition = second.TranslatePoint(new Point(0, 0), row);

                firstPosition.X.Should().BeApproximately(60, 0.5);
                firstPosition.Y.Should().BeApproximately(0, 0.5);
                secondPosition.X.Should().BeApproximately(120, 0.5);
                secondPosition.Y.Should().BeApproximately(20, 0.5);
            }
            finally
            {
                window.Close();
            }
        });
    }

    [TestMethod]
    public void Row_ShouldApplyResponsiveSpanAndOffsetWithinBounds()
    {
        WpfTestHost.Run(() =>
        {
            var col = new Col
            {
                Span = 24,
                XxlSpan = 6,
                XxlOffset = 6,
                Content = new Border { Height = 20 },
            };
            var row = new Row
            {
                Width = 1680,
                Children =
                {
                    col,
                },
            };
            var window = CreateWindow(row, width: 1700, height: 120);

            try
            {
                window.Show();
                row.UpdateLayout();
                WpfTestHost.Pump();

                var position = col.TranslatePoint(new Point(0, 0), row);

                col.ActualWidth.Should().BeApproximately(420, 0.5);
                position.X.Should().BeApproximately(420, 0.5);
            }
            finally
            {
                window.Close();
            }
        });
    }

    [TestMethod]
    public void Pagination_ShouldClampCurrentPageToPageCount()
    {
        WpfTestHost.Run(() =>
        {
            var pagination = new Pagination
            {
                Total = 500,
                PageSize = 10,
                CurrentPage = 999,
            };

            pagination.CurrentPage.Should().Be(50);
        });
    }

    [TestMethod]
    public void InputNumber_ShouldClampStepPrecisionAndSynchronizeText()
    {
        WpfTestHost.Run(() =>
        {
            InitializeThemeResources();

            var inputNumber = new InputNumber
            {
                Width = 180,
                Minimum = 0,
                Maximum = 10,
                Step = 0.25,
                Precision = 2,
                Value = 12.345,
                Prefix = "RMB",
                Suffix = "CNY",
            };

            var window = CreateWindow(inputNumber);

            try
            {
                window.Show();
                inputNumber.ApplyTemplate();
                WpfTestHost.Pump();

                inputNumber.Value.Should().Be(10d);

                var textBox = inputNumber.Template.FindName("PART_TextBox", inputNumber).Should().BeOfType<TextBox>().Subject;
                textBox.Text.Should().Be("10.00");

                inputNumber.Decrement();
                inputNumber.Value.Should().Be(9.75d);
                textBox.Text.Should().Be("9.75");

                textBox.Text = "3.333";
                CommitInputNumberText(inputNumber);

                inputNumber.Value.Should().Be(3.33d);
                textBox.Text.Should().Be("3.33");

                inputNumber.Value = -2d;
                inputNumber.Value.Should().Be(0d);
            }
            finally
            {
                window.Close();
            }
        });
    }

    [TestMethod]
    public void InputNumber_SpinnerButtons_ShouldAdjustValue()
    {
        WpfTestHost.Run(() =>
        {
            InitializeThemeResources();

            var inputNumber = new InputNumber
            {
                Width = 180,
                Minimum = 0,
                Maximum = 10,
                Step = 0.5,
                Value = 1.5,
            };

            var window = CreateWindow(inputNumber);

            try
            {
                window.Show();
                inputNumber.ApplyTemplate();
                WpfTestHost.Pump();

                var textBox = inputNumber.Template.FindName("PART_TextBox", inputNumber).Should().BeOfType<TextBox>().Subject;
                var upButton = inputNumber.Template.FindName("PART_UpButton", inputNumber).Should().BeAssignableTo<ButtonBase>().Subject;
                var downButton = inputNumber.Template.FindName("PART_DownButton", inputNumber).Should().BeAssignableTo<ButtonBase>().Subject;

                upButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, upButton));
                inputNumber.Value.Should().Be(2d);

                downButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, downButton));
                inputNumber.Value.Should().Be(1.5d);

                textBox.Text = "4";
                upButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, upButton));
                inputNumber.Value.Should().Be(4.5d);
            }
            finally
            {
                window.Close();
            }
        });
    }

    [TestMethod]
    public void Segmented_ShouldSwitchSelectionAndPreserveDisabledItems()
    {
        WpfTestHost.Run(() =>
        {
            InitializeThemeResources();

            var segmented = new Segmented
            {
                Block = true,
                SelectedIndex = 0,
            };
            segmented.Items.Add(new ListBoxItem { Content = "Daily" });
            segmented.Items.Add(new ListBoxItem { Content = "Weekly", IsEnabled = false });
            segmented.Items.Add(new ListBoxItem { Content = "Monthly" });

            var window = CreateWindow(segmented);

            try
            {
                window.Show();
                segmented.ApplyTemplate();
                WpfTestHost.Pump();

                segmented.Block.Should().BeTrue();
                segmented.ItemContainerStyle.Should().NotBeNull();
                ((ListBoxItem)segmented.Items[1]).IsEnabled.Should().BeFalse();

                segmented.SelectedIndex = 2;

                segmented.SelectedIndex.Should().Be(2);
                segmented.SelectedItem.Should().BeSameAs(segmented.Items[2]);
            }
            finally
            {
                window.Close();
            }
        });
    }

    [TestMethod]
    public void Statistic_ShouldFormatValueAndRenderPrefixSuffix()
    {
        WpfTestHost.Run(() =>
        {
            InitializeThemeResources();

            var statistic = new Statistic
            {
                Title = "Conversion",
                Value = 0.2734,
                ValueFormat = "{0:P1}",
                Prefix = "+",
                Suffix = "pts",
            };

            var window = CreateWindow(statistic);

            try
            {
                window.Show();
                statistic.ApplyTemplate();
                WpfTestHost.Pump();

                statistic.FormattedValue.Should().Be(string.Format(CultureInfo.InvariantCulture, "{0:P1}", 0.2734));

                var texts = FindVisualChildren<TextBlock>(statistic).Select(static textBlock => textBlock.Text).ToList();
                texts.Should().Contain("Conversion");
                texts.Should().Contain(statistic.FormattedValue);
                texts.Should().Contain("+");
                texts.Should().Contain("pts");
            }
            finally
            {
                window.Close();
            }
        });
    }

    [TestMethod]
    public void Watermark_ShouldLoadTemplateAndCreateBrush()
    {
        WpfTestHost.Run(() =>
        {
            InitializeThemeResources();

            var watermark = new Watermark
            {
                Text = "Ant Design WPF",
                Content = new Border
                {
                    Width = 200,
                    Height = 80,
                    Background = Brushes.White,
                },
            };

            var window = CreateWindow(watermark);

            try
            {
                window.Show();
                watermark.ApplyTemplate();
                WpfTestHost.Pump();

                watermark.WatermarkBrush.Should().NotBeSameAs(Brushes.Transparent);

                var overlay = FindVisualChildren<Rectangle>(watermark).FirstOrDefault();
                overlay.Should().NotBeNull();
                overlay!.Fill.Should().BeSameAs(watermark.WatermarkBrush);
            }
            finally
            {
                window.Close();
            }
        });
    }

    [TestMethod]
    public void Calendar_ShouldApplyStandaloneCalendarStyles()
    {
        WpfTestHost.Run(() =>
        {
            InitializeThemeResources();

            var calendar = new Vktun.Antd.Wpf.Calendar
            {
                SelectedDate = new DateTime(2026, 4, 9),
                DisplayDate = new DateTime(2026, 4, 9),
            };

            var window = CreateWindow(calendar, height: 360);

            try
            {
                window.Show();
                calendar.ApplyTemplate();
                WpfTestHost.Pump();

                calendar.CalendarItemStyle.Should().NotBeNull();
                calendar.CalendarButtonStyle.Should().NotBeNull();
                calendar.CalendarDayButtonStyle.Should().NotBeNull();
                calendar.Template.FindName("PART_CalendarItem", calendar).Should().BeOfType<CalendarItem>();
            }
            finally
            {
                window.Close();
            }
        });
    }

    [TestMethod]
    public void Slider_ShouldExposeTrackFromTemplate()
    {
        WpfTestHost.Run(() =>
        {
            InitializeThemeResources();

            var slider = new Vktun.Antd.Wpf.Slider
            {
                Minimum = 0,
                Maximum = 100,
                Value = 64,
            };

            var window = CreateWindow(slider);

            try
            {
                window.Show();
                slider.ApplyTemplate();
                WpfTestHost.Pump();

                slider.Template.FindName("PART_Track", slider).Should().BeOfType<Track>();
            }
            finally
            {
                window.Close();
            }
        });
    }

    [TestMethod]
    public void Splitter_ShouldApplyTemplateAndRenderGrip()
    {
        WpfTestHost.Run(() =>
        {
            InitializeThemeResources();

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            var splitter = new Splitter
            {
                ResizeDirection = GridResizeDirection.Columns,
                ResizeBehavior = GridResizeBehavior.PreviousAndNext,
            };
            Grid.SetColumn(splitter, 1);
            grid.Children.Add(new Border());
            grid.Children.Add(splitter);
            grid.Children.Add(new Border { });
            Grid.SetColumn(grid.Children[2], 2);

            var window = CreateWindow(grid);

            try
            {
                window.Show();
                splitter.ApplyTemplate();
                WpfTestHost.Pump();

                splitter.Template.Should().NotBeNull();
                VisualTreeHelper.GetChildrenCount(splitter).Should().BeGreaterThan(0);
            }
            finally
            {
                window.Close();
            }
        });
    }

    [TestMethod]
    public void Tabs_ShouldSelectFirstEnabledPane_WhenItemsAssigned()
    {
        WpfTestHost.Run(() =>
        {
            var designPane = new TabPane { Key = "design", Header = "Design" };
            var codePane = new TabPane { Key = "code", Header = "Code" };

            var tabs = new Tabs
            {
                Items = new ObservableCollection<TabPane> { designPane, codePane },
            };

            tabs.SelectedIndex.Should().Be(0);
            tabs.SelectedKey.Should().Be("design");
            tabs.SelectedItem.Should().BeSameAs(designPane);
            designPane.IsSelected.Should().BeTrue();
            codePane.IsSelected.Should().BeFalse();
        });
    }

    [TestMethod]
    public void Tabs_SelectTabCommand_ShouldSwitchSelection()
    {
        WpfTestHost.Run(() =>
        {
            var designPane = new TabPane { Key = "design", Header = "Design" };
            var codePane = new TabPane { Key = "code", Header = "Code" };

            var tabs = new Tabs
            {
                Items = new ObservableCollection<TabPane> { designPane, codePane },
            };

            tabs.SelectTabCommand.CanExecute(codePane).Should().BeTrue();
            tabs.SelectTabCommand.Execute(codePane);

            tabs.SelectedIndex.Should().Be(1);
            tabs.SelectedKey.Should().Be("code");
            tabs.SelectedItem.Should().BeSameAs(codePane);
            designPane.IsSelected.Should().BeFalse();
            codePane.IsSelected.Should().BeTrue();
        });
    }

    [TestMethod]
    public void Tabs_ShouldRenderSelectedPaneContent_WhenPaneContentIsVisual()
    {
        WpfTestHost.Run(() =>
        {
            InitializeThemeResources();

            var designContent = new TextBlock { Text = "Design content" };
            var codeContent = new TextBlock { Text = "Code content" };
            var designPane = new TabPane { Key = "design", Header = "Design", Content = designContent };
            var codePane = new TabPane { Key = "code", Header = "Code", Content = codeContent };

            var tabs = new Tabs
            {
                Items = new ObservableCollection<TabPane> { designPane, codePane },
                SelectedIndex = 0,
                Width = 360,
                Height = 220,
            };

            var window = CreateWindow(tabs, width: 420, height: 280);

            try
            {
                window.Show();
                tabs.ApplyTemplate();
                WpfTestHost.Pump();

                tabs.SelectedItem.Should().BeSameAs(designPane);
                FindVisualChildren<TextBlock>(tabs).Any(textBlock => textBlock.Text == "Design content").Should().BeTrue();
            }
            finally
            {
                window.Close();
            }
        });
    }

    [TestMethod]
    public void Tabs_ResetActiveIndicator_ShouldReplaceFrozenTranslateTransform()
    {
        WpfTestHost.Run(() =>
        {
            var frozenTransform = new TranslateTransform(12, 0);
            frozenTransform.Freeze();
            var indicator = new Border
            {
                Width = 20,
                Opacity = 0.95,
                RenderTransform = frozenTransform,
            };
            var tabs = new Tabs();
            var activeIndicatorField = typeof(Tabs).GetField("_activeIndicator", BindingFlags.Instance | BindingFlags.NonPublic);
            activeIndicatorField.Should().NotBeNull();
            activeIndicatorField!.SetValue(tabs, indicator);
            var activeIndicatorTransformField = typeof(Tabs).GetField("_activeIndicatorTransform", BindingFlags.Instance | BindingFlags.NonPublic);
            activeIndicatorTransformField.Should().NotBeNull();
            activeIndicatorTransformField!.SetValue(tabs, frozenTransform);
            var resetActiveIndicator = typeof(Tabs).GetMethod("ResetActiveIndicator", BindingFlags.Instance | BindingFlags.NonPublic);
            resetActiveIndicator.Should().NotBeNull();

            resetActiveIndicator!.Invoke(tabs, null);

            var transform = indicator.RenderTransform.Should().BeOfType<TranslateTransform>().Subject;
            transform.IsFrozen.Should().BeFalse();
            transform.X.Should().Be(0);
            indicator.Width.Should().Be(0);
            indicator.Opacity.Should().Be(0);
        });
    }

    private static Window CreateWindow(object content, double width = 420, double height = 280)
    {
        return new Window
        {
            Content = content,
            Width = width,
            Height = height,
        };
    }

    private static void InitializeThemeResources()
    {
        Application.Current!.Resources = new ResourceDictionary();
        Application.Current.Resources.MergedDictionaries.Add(new AntdThemeResources());
    }

    private static void CommitInputNumberText(InputNumber inputNumber)
    {
        var commitTextMethod = typeof(InputNumber).GetMethod("CommitText", BindingFlags.Instance | BindingFlags.NonPublic);
        commitTextMethod.Should().NotBeNull();
        commitTextMethod!.Invoke(inputNumber, null);

        WpfTestHost.Pump();
    }

    private static void InvokeToggleBlockMouseDown(ToggleBlock toggle)
    {
        var onMouseLeftButtonDown = typeof(ToggleBlock).GetMethod("OnMouseLeftButtonDown", BindingFlags.Instance | BindingFlags.NonPublic);
        onMouseLeftButtonDown.Should().NotBeNull();

        var args = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
        {
            RoutedEvent = UIElement.MouseLeftButtonDownEvent,
            Source = toggle,
        };

        onMouseLeftButtonDown!.Invoke(toggle, new object[] { args });
    }

    private static void InvokePopconfirmPreviewMouseDown(Popconfirm popconfirm)
    {
        var onPreviewMouseLeftButtonDown = typeof(Popconfirm).GetMethod("OnPreviewMouseLeftButtonDown", BindingFlags.Instance | BindingFlags.NonPublic);
        onPreviewMouseLeftButtonDown.Should().NotBeNull();

        var args = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
        {
            RoutedEvent = UIElement.PreviewMouseLeftButtonDownEvent,
            Source = popconfirm,
        };

        onPreviewMouseLeftButtonDown!.Invoke(popconfirm, new object[] { args });
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject root)
        where T : DependencyObject
    {
        var count = VisualTreeHelper.GetChildrenCount(root);
        for (var index = 0; index < count; index++)
        {
            var child = VisualTreeHelper.GetChild(root, index);
            if (child is T typedChild)
            {
                yield return typedChild;
            }

            foreach (var nestedChild in FindVisualChildren<T>(child))
            {
                yield return nestedChild;
            }
        }
    }
}
