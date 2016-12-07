using Com.Aurora.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
namespace Com.Aurora.AuWeather.CustomControls
{
    public sealed partial class ColorPicker : UserControl
    {
        public ColorPicker()
        {
            this.InitializeComponent();
        }
        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            Header.Background = Picked;
            PickColor.Text = Title;
            AccentColor.Text = AccentColorTitle;
            var lightness = Palette.RGBtoL(Picked.Color);
            PickColor.Foreground = new SolidColorBrush(lightness <= 127 ? Windows.UI.Colors.White : Windows.UI.Colors.Black);
        }
        public string Colors
        {
            get { return (string)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorsProperty =
            DependencyProperty.Register("Colors", typeof(string), typeof(ColorPicker), new PropertyMetadata(string.Empty, OnColorsChanged));
        public double ColorGridHeight
        {
            get { return (double)GetValue(ColorGridHeightProperty); }
            set { SetValue(ColorGridHeightProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorGridHeightProperty =
            DependencyProperty.Register("ColorGridHeight", typeof(double), typeof(ColorPicker), new PropertyMetadata(24d));
        public double ColorGridWidth
        {
            get { return (double)GetValue(ColorGridWidthProperty); }
            set { SetValue(ColorGridWidthProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorGridWidthProperty =
            DependencyProperty.Register("ColorGridWidth", typeof(double), typeof(ColorPicker), new PropertyMetadata(24d));
        public SolidColorBrush Picked
        {
            get { return (SolidColorBrush)GetValue(PickedProperty); }
            set { SetValue(PickedProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PickedProperty =
    DependencyProperty.Register("Picked", typeof(SolidColorBrush), typeof(ColorPicker), new PropertyMetadata(new SolidColorBrush((Color)App.Current.Resources["SystemAccentColor"]), OnPickedChanged));
        private static void OnPickedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var picker = d as ColorPicker;
            if (picker.Root != null)
            {
                if (picker.Picked == null)
                    return;
                picker.Header.Background = picker.Picked;
                var lightness = Palette.RGBtoL(picker.Picked.Color);
                picker.PickColor.Foreground = new SolidColorBrush(lightness <= 127 ? Windows.UI.Colors.White : Windows.UI.Colors.Black);
            }
        }
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ColorPicker), new PropertyMetadata("Pick a color"));
        public string AccentColorTitle
        {
            get { return (string)GetValue(AccentColorTitleProperty); }
            set { SetValue(AccentColorTitleProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AccentColorTitleProperty =
            DependencyProperty.Register("AccentColorTitle", typeof(string), typeof(ColorPicker), new PropertyMetadata("Accent color"));
        public event EventHandler<ColorPickedEventArgs> ColorPicked;
        private static List<ColorGrid> list;
        private static void OnColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var picker = d as ColorPicker;
            if (picker.Root == null)
                return;
            if (picker.Colors != string.Empty && picker.Colors.Length > 0)
            {
                list = new List<ColorGrid>();
                var colors = picker.Colors.Split(',');
                CultureInfo provider = CultureInfo.InvariantCulture;
                ulong value;
                foreach (string item in colors)
                {
                    if (ulong.TryParse(item.Trim().Remove(0, 2), NumberStyles.HexNumber, provider, out value))
                        list.Add(new ColorGrid(new SolidColorBrush(Color.FromArgb((byte)((value >> 24) & 0xFF), (byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF), (byte)((value) & 0xFF))), picker.ColorGridWidth, picker.ColorGridHeight));
                }
                if (list.Count > 0)
                    picker.Root.ItemsSource = list;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (ChangeColor.Children[0] as ColorAnimation).To = (Color)App.Current.Resources["SystemAccentColor"];
            var lightness = Palette.RGBtoL((Color)App.Current.Resources["SystemAccentColor"]);
            (ChangeColor.Children[1] as ColorAnimation).To = lightness <= 127 ? Windows.UI.Colors.White : Windows.UI.Colors.Black;
            ChangeColor.Completed += (s, v) =>
            {
                this.Picked = new SolidColorBrush((Color)App.Current.Resources["SystemAccentColor"]);
            };
            ChangeColor.Begin();
            this.OnColorPicked((Color)App.Current.Resources["SystemAccentColor"], true);
        }
        private void OnColorPicked(Color color, bool isSystem)
        {
            ColorPicked?.Invoke(this, new ColorPickedEventArgs(color, isSystem));
        }
        private void Root_ItemClick(object sender, ItemClickEventArgs e)
        {
            (ChangeColor.Children[0] as ColorAnimation).To = (e.ClickedItem as ColorGrid).Color.Color;
            var lightness = Palette.RGBtoL((e.ClickedItem as ColorGrid).Color.Color);
            (ChangeColor.Children[1] as ColorAnimation).To = lightness <= 127 ? Windows.UI.Colors.White : Windows.UI.Colors.Black;
            ChangeColor.Completed += (s, v) =>
            {
                this.Picked = (e.ClickedItem as ColorGrid).Color;
            };
            ChangeColor.Begin();
            this.OnColorPicked((e.ClickedItem as ColorGrid).Color.Color, false);
        }
    }
    public class ColorPickedEventArgs
    {
        public ColorPickedEventArgs(Color color, bool isSystemAccent)
        {
            Color = color;
            IsSystemAccent = isSystemAccent;
        }
        public Color Color { get; set; }
        public bool IsSystemAccent { get; set; }
    }
    class ColorGrid
    {
        public ColorGrid(SolidColorBrush color)
        {
            Color = color;
        }
        public ColorGrid(SolidColorBrush color, double width, double height) : this(color)
        {
            this.Width = width;
            this.Height = height;
        }
        public SolidColorBrush Color { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }
    }
}
