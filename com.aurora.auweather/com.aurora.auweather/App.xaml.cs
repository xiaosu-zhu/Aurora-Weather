﻿// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.CustomControls;
using Com.Aurora.AuWeather.Shared;
using Com.Aurora.Shared;
using Com.Aurora.Shared.Extensions;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Com.Aurora.AuWeather
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static Color MainColor { get; internal set; } = Colors.Transparent;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.UnhandledException += App_UnhandledException;
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        private async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var log = new CrashLog(e.Exception.ToString(), e.Exception.HResult, e.Exception.StackTrace, e.Exception.Source, e.Exception.Message);
            e.Handled = true;
            if (e.Exception.HResult == -2147418113 || e.Exception.HResult.ToHexString() == "0x80070057")
            {
                return;
            }
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
            {
                Window.Current.Content = null;
                var f = new Frame();
                f.Navigate(typeof(CrashReportPage), log);
                Window.Current.Content = f;
            }));
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            {
                /* mobile 设置状态栏 */
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    StatusBar statusBar = StatusBar.GetForCurrentView();
                    statusBar.ForegroundColor = Colors.White;
                    ApplicationView.GetForCurrentView()
                    .SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                }
                /* 桌面设置 */
                var view = ApplicationView.GetForCurrentView();
                view.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                view.TitleBar.InactiveBackgroundColor = Colors.Transparent;
                view.TitleBar.BackgroundColor = Colors.Transparent;
                // button
                view.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            }
            if (args.Kind == ActivationKind.ToastNotification)
            {
                //Get the pre-defined arguments and user inputs from the eventargs;
                var toastArgs = args as ToastNotificationActivatedEventArgs;
                var splash = toastArgs.SplashScreen;
                var arguments = toastArgs.Argument;
                if (Window.Current.Content == null)
                {
                    if (toastArgs.PreviousExecutionState != ApplicationExecutionState.Running)
                    {
                        SplashScreenEx extendedSplash = new SplashScreenEx(splash, arguments);
                        Window.Current.Content = extendedSplash;
                    }
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (Window.Current.Content == null)
            {
                if (e.PreviousExecutionState != ApplicationExecutionState.Running)
                {
                    if (e.Arguments == "Com.Aurora.AuWeather.Calculator")
                    {
                        Calculator.MainPage m = new Calculator.MainPage();
                        Window.Current.Content = m;
                    }
                    else
                    {
                        SplashScreenEx extendedSplash = new SplashScreenEx(e.SplashScreen, e.Arguments);
                        Window.Current.Content = extendedSplash;
                    }
                }
            }

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                //this.DebugSettings.IsTextPerformanceVisualizationEnabled = true;
                //this.DebugSettings.EnableFrameRateCounter = true;
                //this.DebugSettings.IsOverdrawHeatMapEnabled = true;
                //this.DebugSettings.EnableRedrawRegions = true;
            }
#endif
            if (e.Arguments != "Com.Aurora.AuWeather.Calculator")
            {
                /* mobile 设置状态栏 */
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    StatusBar statusBar = StatusBar.GetForCurrentView();
                    statusBar.ForegroundColor = Colors.White;
                    ApplicationView.GetForCurrentView()
                    .SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                }
                /* 桌面设置 */
                var view = ApplicationView.GetForCurrentView();
                view.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                view.TitleBar.InactiveBackgroundColor = Colors.Transparent;
                view.TitleBar.BackgroundColor = Colors.Transparent;
                // button
                view.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
                view.TitleBar.ButtonForegroundColor = Colors.White;
                //view.TitleBar.ButtonHoverForegroundColor = Colors.Black;
                //view.TitleBar.ButtonPressedForegroundColor = Colors.Black;
                view.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            }
            // Ensure the current window is active
            Window.Current.Activate();

        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        public static void ChangeThemeColor(Color color)
        {
            var color1 = Color.FromArgb(Convert.ToByte(color.A * 0.9), color.R, color.G, color.B);
            var color2 = Color.FromArgb(Convert.ToByte(color.A * 0.6), color.R, color.G, color.B);
            var color3 = Color.FromArgb(Convert.ToByte(color.A * 0.8), color.R, color.G, color.B);
            (App.Current.Resources["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            (App.Current.Resources["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            (App.Current.Resources["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            (App.Current.Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            (App.Current.Resources["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            (App.Current.Resources["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            (App.Current.Resources["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            (App.Current.Resources["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            (App.Current.Resources["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            (App.Current.Resources["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            (App.Current.Resources["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            (App.Current.Resources["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            (App.Current.Resources["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            (App.Current.Resources["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            (App.Current.Resources["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
            if ((Window.Current.Content is Frame) && (Window.Current.Content as Frame).Content is IThemeble)
            {
                ((Window.Current.Content as Frame).Content as IThemeble).ChangeThemeColor(color);
            }
        }
    }
}
