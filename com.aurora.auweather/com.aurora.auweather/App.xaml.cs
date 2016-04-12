// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Com.Aurora.AuWeather
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
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
                    .SetDesiredBoundsMode(ApplicationViewBoundsMode.UseVisible);
                }
                /* 桌面设置 */
                var view = ApplicationView.GetForCurrentView();
                view.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                view.TitleBar.InactiveBackgroundColor = Colors.Transparent;
                view.TitleBar.BackgroundColor = Colors.Transparent;
                // button
                //view.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
                //view.TitleBar.ButtonForegroundColor = Colors.White;
                //view.TitleBar.ButtonHoverForegroundColor = Colors.Black;
                //view.TitleBar.ButtonPressedForegroundColor = Colors.Black;
                view.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
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
            // Ensure the current window is active
            Window.Current.Activate();
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
                Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            }
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
    }
}
