// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Com.Aurora.Shared.Helpers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Windows.System.Threading;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Com.Aurora.Shared.Controls
{
    public sealed class PulltoRefresh : ContentControl
    {
        public PulltoRefresh()
        {
            DefaultStyleKey = typeof(PulltoRefresh);
        }

        public ScrollViewer Main { get; private set; }
        public Grid Indicator { get; private set; }
        public CompositeTransform IndicatorTransform { get; private set; }
        public Storyboard IndicatorIn { get; private set; }
        public Storyboard IndicatorOut { get; private set; }
        public Storyboard IndicatorOverlayIn { get; private set; }
        public ProgressRing IndicatorRing { get; private set; }
        public Ellipse IndicatorOverlayBackground { get; private set; }
        public Storyboard RefreshStartAni { get; private set; }
        public Storyboard RefreshCompleteAni { get; private set; }
        public RelativePanel Root { get; private set; }

        private bool triggerStart = false;
        private bool triggered = false;
        private bool canSlide = false;
        private bool canTrigger = false;

        public Storyboard IndicatorOverlayOut { get; private set; }

        public event EventHandler<RefreshStartEventArgs> RefreshStart;
        public event EventHandler<ScrollViewerViewChangedEventArgs> ViewChanged;

        private void OnRefreshStart(object sender, RefreshStartEventArgs e)
        {
            RefreshStart?.Invoke(sender, e);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Main = GetTemplateChild("Main") as ScrollViewer;
            Root = GetTemplateChild("Root") as RelativePanel;
            Indicator = GetTemplateChild("Indicator") as Grid;
            IndicatorTransform = GetTemplateChild("IndicatorTransform") as CompositeTransform;
            IndicatorRing = GetTemplateChild("IndicatorRing") as ProgressRing;
            IndicatorOverlayBackground = GetTemplateChild("IndicatorOverlayBackground") as Ellipse;
            IndicatorIn = Root.Resources["IndicatorIn"] as Storyboard;
            IndicatorOut = Root.Resources["IndicatorOut"] as Storyboard;
            IndicatorOverlayIn = Root.Resources["IndicatorOverlayIn"] as Storyboard;
            IndicatorOverlayOut = Root.Resources["IndicatorOverlayOut"] as Storyboard;
            RefreshStartAni = Root.Resources["RefreshStart"] as Storyboard;
            RefreshCompleteAni = Root.Resources["RefreshComplete"] as Storyboard;
            Main.ViewChanged += Main_ViewChanged;

            if (ForceEnabled || InteractionHelper.HaveTouchCapabilities() || Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                Root.ManipulationDelta += Root_ManipulationDelta;
                Root.Loaded += Root_Loaded;
                Root.PointerPressed += Root_PointerPressed;
                Root.PointerReleased += Root_PointerReleased;
                Root.PointerCanceled += Root_PointerReleased;
                Root.PointerCaptureLost += Root_PointerReleased;
                Root.PointerExited += Root_PointerReleased;
                Main.LayoutUpdated += Main_LayoutUpdated;
                Main.VerticalScrollMode = ScrollMode.Disabled;
                Main.PointerWheelChanged += Main_PointerWheelChanged;
            }
            else
            {
                Main.VerticalScrollMode = ScrollMode.Enabled;
                Indicator.Visibility = Visibility.Collapsed;
                Main.ManipulationMode = ManipulationModes.None;
                Main.LayoutUpdated += Main_LayoutUpdated;
            }
        }

        private void Main_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            OnViewChanged(sender, e);
        }

        private void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ViewChanged?.Invoke(sender, e);
        }

        private void Main_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            var ptr = e.GetCurrentPoint(null);
            var ptrpr = ptr.Properties;
            var wheeldelta = ptrpr.MouseWheelDelta;
            var offset = Main.VerticalOffset - wheeldelta;
            Main.ChangeView(0, offset, 1);
        }

        private void Main_LayoutUpdated(object sender, object e)
        {
            VerticalOffset = Main.VerticalOffset;
        }

        private void Root_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            canSlide = true;
            canTrigger = true;
        }
        private void Root_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (Indicator.Height > 0)
            {
                canSlide = false;
                if (triggerStart)
                {
                    TriggerStartComplete();
                }
                else
                {
                    ScrolltoOrigin();
                }
            }
            else
            {
                canTrigger = false;
            }
        }

        public void RefreshComplete()
        {
            ScrolltoOrigin();
            triggered = false;
            IndicatorRestore();
            RefreshCompleteAni.Begin();
        }

        private void TriggerStartComplete()
        {
            Main.ChangeView(0, 0, 1);
            IndicatorOverlayIn.Begin();
            triggerStart = false;
            TriggerRefresh();
        }

        private void TriggerRefresh()
        {
            if (!triggered)
            {
                OnRefreshStart(this, new RefreshStartEventArgs());
                triggered = true;
                ThreadPoolTimer.CreateTimer(async (work) =>
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
                      {
                          RefreshComplete();
                      }));
                }, TimeSpan.FromSeconds(25000));
                RefreshStartAni.Begin();
            }
        }

        private void IndicatorRestore()
        {
            IndicatorOut.Begin();
            IndicatorRing.IsActive = false;
        }

        private void ScrolltoOrigin()
        {
            Main.ChangeView(0, 0, 1);
            IndicatorOverlayOut.Begin();
        }
        private void Root_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (canSlide)
            {
                var delta = e.Delta.Translation.Y;
                if (Indicator.Height > 0 || (Main.VerticalOffset <= 0 && delta > 0 && canTrigger))
                {
                    NormallyPull(delta);
                }
                else
                {
                    NormallySlide(delta);
                }
                e.Handled = true;
            }
        }

        private void NormallyPull(double delta)
        {
            delta *= GetElasticFactor(Indicator.Height / IndicatorHeight);
            var height = Indicator.Height + delta;
            if (height < 0)
            {
                Indicator.Height = 0;
            }
            else
            {
                Indicator.Height = height;
                var percent = Indicator.Height / TriggerOffset;
                TransformIndicator(percent);
                if (Indicator.Height > TriggerOffset && !triggerStart)
                {
                    IndicatorPreparetoTrigger();
                }
                if (Indicator.Height < TriggerOffset && !triggered)
                {
                    IndicatorFallback();
                }
            }
        }

        private void NormallySlide(double delta)
        {
            Main.ScrollToVerticalOffset(Main.VerticalOffset - delta);
        }

        private void IndicatorFallback()
        {
            triggerStart = false;
            IndicatorOut.Begin();
        }

        private void IndicatorPreparetoTrigger()
        {
            triggerStart = true;
            if (!triggered)
                IndicatorIn.Begin();
        }

        private void TransformIndicator(double percent)
        {
            if (!triggered)
                IndicatorTransform.Rotation = 360 * percent;
        }

        private double GetElasticFactor(double percent)
        {
            return ElasticFactor * (1 - EasingHelper.QuinticEase(EasingMode.EaseOut, percent));
        }


        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            //Indicator.Height = IndicatorHeight;
            if (IndicatorDisplayMode == IndicatorDisplayMode.Overlay)
            {
                Indicator.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                IndicatorOverlayBackground.Fill = IndicatorBackground;
                IndicatorOverlayBackground.Visibility = Visibility.Visible;
                Main.SetValue(RelativePanel.AlignTopWithPanelProperty, true);
                Main.SetValue(RelativePanel.BelowProperty, null);
            }
            else
            {
                Indicator.Background = IndicatorBackground;
            }
            //IndicatorFixed.Height = IndicatorHeight;
            (IndicatorOverlayIn.Children[0] as DoubleAnimation).To = TriggerOffset;
            //(IndicatorOverlayOut.Children[0] as DoubleAnimation).To = 0;
        }

        public bool ForceEnabled
        {
            get { return (bool)GetValue(ForceEnabledProperty); }
            set { SetValue(ForceEnabledProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ForceEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForceEnabledProperty =
            DependencyProperty.Register("ForceEnabled", typeof(bool), typeof(PulltoRefresh), new PropertyMetadata(false, new PropertyChangedCallback(OnForceEnabledChanged)));

        private static void OnForceEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = d as PulltoRefresh;
            if ((bool)e.NewValue)
            {
                p.Root.ManipulationDelta += p.Root_ManipulationDelta;
                p.Root_Loaded(null, null);
                p.Root.PointerPressed += p.Root_PointerPressed;
                p.Root.PointerReleased += p.Root_PointerReleased;
                p.Root.PointerCanceled += p.Root_PointerReleased;
                p.Root.PointerCaptureLost += p.Root_PointerReleased;
                p.Root.PointerExited += p.Root_PointerReleased;
                p.Main.LayoutUpdated += p.Main_LayoutUpdated;
                p.Main.PointerPressed += p.Root_PointerPressed;
                p.Main.VerticalScrollMode = ScrollMode.Disabled;
                p.Main.PointerWheelChanged += p.Main_PointerWheelChanged;
                p.Main.VerticalScrollMode = ScrollMode.Disabled;
                p.Indicator.Visibility = Visibility.Visible;
                p.Main.ManipulationMode = ManipulationModes.All;
            }
        }

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }
        // Using a DependencyProperty as the backing store for VerticalScrollBarVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(PulltoRefresh), new PropertyMetadata(0));

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }
        // Using a DependencyProperty as the backing store for VerticalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(PulltoRefresh), new PropertyMetadata(0d));

        public double IndicatorHeight
        {
            get { return (double)GetValue(IndicatorHeightProperty); }
            set { SetValue(IndicatorHeightProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IndicatorHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorHeightProperty =
            DependencyProperty.Register("IndicatorHeight", typeof(double), typeof(PulltoRefresh), new PropertyMetadata(200d));

        public double TriggerOffset
        {
            get { return (double)GetValue(TriggerOffsetProperty); }
            set { SetValue(TriggerOffsetProperty, value); }
        }
        // Using a DependencyProperty as the backing store for TriggerOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TriggerOffsetProperty =
            DependencyProperty.Register("TriggerOffset", typeof(double), typeof(PulltoRefresh), new PropertyMetadata(40d));

        public double ElasticFactor
        {
            get { return (double)GetValue(ElasticFactorProperty); }
            set { SetValue(ElasticFactorProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ElasticFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ElasticFactorProperty =
            DependencyProperty.Register("ElasticFactor", typeof(double), typeof(PulltoRefresh), new PropertyMetadata(0.5));

        public SolidColorBrush IndicatorBackground
        {
            get { return (SolidColorBrush)GetValue(IndicatorBackgroundProperty); }
            set { SetValue(IndicatorBackgroundProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IndicatorBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorBackgroundProperty =
            DependencyProperty.Register("IndicatorBackground", typeof(SolidColorBrush), typeof(PulltoRefresh), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0))));


        public SolidColorBrush IndicatorForeground
        {
            get { return (SolidColorBrush)GetValue(IndicatorForegroundProperty); }
            set { SetValue(IndicatorForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IndicatorForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorForegroundProperty =
            DependencyProperty.Register("IndicatorForeground", typeof(SolidColorBrush), typeof(PulltoRefresh), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 0, 0, 0))));


        public IndicatorDisplayMode IndicatorDisplayMode
        {
            get { return (IndicatorDisplayMode)GetValue(IndicatorDisplayModeProperty); }
            set { SetValue(IndicatorDisplayModeProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IndicatorDisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorDisplayModeProperty =
            DependencyProperty.Register("IndicatorDisplayMode", typeof(IndicatorDisplayMode), typeof(PulltoRefresh), new PropertyMetadata(IndicatorDisplayMode.Header));
    }

    public enum IndicatorDisplayMode { Header, Overlay };
    public class RefreshStartEventArgs
    {

    }
}
