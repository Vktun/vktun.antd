namespace Vktun.Antd.Wpf.Sample.Snippets;

public static class LayoutSnippets
{
    public static string Divider => """
<StackPanel>
    <TextBlock Text="搜索条件" />
    <antd:Divider Margin="0,12,0,12" />
    <TextBlock Text="结果列表" />
</StackPanel>
""";

    public static string Grid => """
<antd:Row Gutter="16" Justify="Center" Align="Center">
    <antd:Col Span="8"><Border Height="64" /></antd:Col>
    <antd:Col Span="8"><Border Height="64" /></antd:Col>
    <antd:Col Span="8"><Border Height="64" /></antd:Col>
</antd:Row>
""";

    public static string Space => """
<antd:Space Orientation="Horizontal" Gap="12">
    <antd:Button Content="取消" />
    <antd:Button Type="Primary" Content="提交" />
    <antd:Tag Color="Blue">Draft</antd:Tag>
</antd:Space>
""";

    public static string Flex => """
<antd:Flex Gap="12"
           Wrap="True"
           Justify="Center"
           Align="Center">
    <Border Width="96" Height="48" />
    <Border Width="120" Height="48" />
    <Border Width="88" Height="48" />
</antd:Flex>
""";

    public static string Layout => """
<antd:Layout>
    <antd:LayoutHeader Content="Header" />
    <antd:LayoutSider Width="180" Content="Sider" />
    <antd:LayoutContent Content="Content" />
    <antd:LayoutFooter Content="Footer" />
</antd:Layout>
""";

    public static string Splitter => """
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" />
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="2*" />
    </Grid.ColumnDefinitions>

    <Border Grid.Column="0" />
    <antd:Splitter Grid.Column="1"
                   ResizeDirection="Columns"
                   ResizeBehavior="PreviousAndNext" />
    <Border Grid.Column="2" />
</Grid>
""";
}
