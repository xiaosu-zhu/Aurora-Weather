// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.CustomControls;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.AuWeather.SettingOptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using Com.Aurora.Shared.Extensions;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class StartPage : Page, INotifyPropertyChanged, IThemeble
    {
        public StartPage()
        {
            this.InitializeComponent();
            var p = Preferences.Get();
            Theme = p.GetTheme();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                //TitleBlock.Visibility = Visibility.Collapsed;
            }
        }

        private ElementTheme theme;
        private ExpressionAnimation parallaxAnimation0;
        private ExpressionAnimation parallaxAnimation1;
        private ExpressionAnimation parallaxAnimation2;
        private ExpressionAnimation parallaxAnimation3;
        private ExpressionAnimation parallaxAnimation4;
        private CompositionPropertySet scrollViewerProperties;
        private double h = 1080d;
        private double w = 1920d;

        public event PropertyChangedEventHandler PropertyChanged;

        public ElementTheme Theme
        {
            get
            {
                return theme;
            }

            set
            {
                theme = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ChangeThemeColor(Color color)
        {
            var color1 = Color.FromArgb(Convert.ToByte(color.A * 0.9), color.R, color.G, color.B);
            var color2 = Color.FromArgb(Convert.ToByte(color.A * 0.6), color.R, color.G, color.B);
            var color3 = Color.FromArgb(Convert.ToByte(color.A * 0.8), color.R, color.G, color.B);
            (Resources["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            (Resources["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            (Resources["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            (Resources["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            (Resources["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            (Resources["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            (Resources["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
            //if (MainFrame.Content is IThemeble)
            //{
            //    (MainFrame.Content as IThemeble).ChangeThemeColor(color);
            //}
        }

        private void MainFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (App.MainColor.A > 0)
                if (MainFrame.Content is IThemeble)
                {
                    (MainFrame.Content as IThemeble).ChangeThemeColor(App.MainColor);
                }
        }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            Compositor compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            // 创建驱动视差滚动的表达式动画。
            parallaxAnimation0 = compositor.CreateExpressionAnimation("MyForeground.Translation.X / MyParallaxRatio");
            parallaxAnimation1 = compositor.CreateExpressionAnimation("MyForeground.Translation.X / MyParallaxRatio");
            parallaxAnimation2 = compositor.CreateExpressionAnimation("MyForeground.Translation.X / MyParallaxRatio");
            parallaxAnimation3 = compositor.CreateExpressionAnimation("MyForeground.Translation.X / MyParallaxRatio");
            parallaxAnimation4 = compositor.CreateExpressionAnimation("((MyForeground.Translation.X / MyParallaxRatio) + offset)");
            // 设置对前景对象的引用。
            scrollViewerProperties = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(RootScroll);
            parallaxAnimation0.SetReferenceParameter("MyForeground", scrollViewerProperties);
            parallaxAnimation1.SetReferenceParameter("MyForeground", scrollViewerProperties);
            parallaxAnimation2.SetReferenceParameter("MyForeground", scrollViewerProperties);
            parallaxAnimation3.SetReferenceParameter("MyForeground", scrollViewerProperties);
            parallaxAnimation4.SetReferenceParameter("MyForeground", scrollViewerProperties);
            // 设置背景对象视差滚动的速度。
            parallaxAnimation0.SetScalarParameter("MyParallaxRatio", 1f);
            parallaxAnimation1.SetScalarParameter("MyParallaxRatio", 2f);
            parallaxAnimation2.SetScalarParameter("MyParallaxRatio", 4f);
            parallaxAnimation3.SetScalarParameter("MyParallaxRatio", 8f);
            parallaxAnimation4.SetScalarParameter("MyParallaxRatio", 0.5f);
            parallaxAnimation4.SetScalarParameter("offset", 3 * (float)ActualWidth);

            var backgroundVisual0 = ElementCompositionPreview.GetElementVisual(BGLayer0);
            var backgroundVisual1 = ElementCompositionPreview.GetElementVisual(BGLayer1);
            var backgroundVisual2 = ElementCompositionPreview.GetElementVisual(BGLayer2);
            var backgroundVisual3 = ElementCompositionPreview.GetElementVisual(BGLayer3);
            var backgroundVisual4 = ElementCompositionPreview.GetElementVisual(RootFrame);
            // 对背景对象开始视差动画。
            backgroundVisual0.StartAnimation("Offset.X", parallaxAnimation0);
            backgroundVisual1.StartAnimation("Offset.X", parallaxAnimation1);
            backgroundVisual2.StartAnimation("Offset.X", parallaxAnimation2);
            backgroundVisual3.StartAnimation("Offset.X", parallaxAnimation3);
            backgroundVisual4.StartAnimation("Offset.X", parallaxAnimation4);
            MainFrame.Navigate(typeof(CitiesSetting));
            IndAni.Begin();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var width = this.ActualWidth;
            var height = this.ActualHeight;

            if (parallaxAnimation4 != null)
                parallaxAnimation4.SetScalarParameter("offset", 3 * (float)width);
            var verSc = height / h;
            Base.Width = 2 * width;
            BGLayer0.Width = 2 * width;
            BGLayer1.Width = 2 * width;
            BGLayer2.Width = 2 * width;
            BGLayer3.Width = 2 * width;
            BGLayer0.Height = height;
            BGLayer1.Height = height;
            BGLayer2.Height = height;
            BGLayer3.Height = height;
            var horSc = width / w;
            BGLayer0.ReplaceElements(horSc, verSc);
            BGLayer1.ReplaceElements(horSc, verSc);
            BGLayer2.ReplaceElements(horSc, verSc);
            BGLayer3.ReplaceElements(horSc, verSc);
            h = height;
            w = width;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (parallaxAnimation0 != null)
            {
                parallaxAnimation0.Dispose();
                parallaxAnimation0 = null;
            }
            if (parallaxAnimation1 != null)
            {
                parallaxAnimation1.Dispose();
                parallaxAnimation1 = null;
            }
            if (parallaxAnimation2 != null)
            {
                parallaxAnimation2.Dispose();
                parallaxAnimation2 = null;
            }
            if (parallaxAnimation3 != null)
            {
                parallaxAnimation3.Dispose();
                parallaxAnimation3 = null;
            }
            if (parallaxAnimation4 != null)
            {
                parallaxAnimation4.Dispose();
                parallaxAnimation4 = null;
            }
            if (scrollViewerProperties != null)
            {
                scrollViewerProperties.Dispose();
                scrollViewerProperties = null;
            }
            IndAni.Stop();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if ((MainFrame.Content as CitiesSetting).CheckCompleted())

            {

                (MainFrame.Content as CitiesSetting).Complete();

                (Window.Current.Content as Frame).Navigate(typeof(MainPage));

            }
        }
    }
}
