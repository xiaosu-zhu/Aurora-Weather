using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.Shared.Extensions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DonationPage : Page, INotifyPropertyChanged
    {
        private License.License license;

        public event PropertyChangedEventHandler PropertyChanged;

        public DonationPage()
        {
            license = new License.License();
            license.GetPrice();
            var p = Preferences.Get();
            Theme = p.GetTheme();
            license.LicenseChanged += License_LicenseChanged;
            this.InitializeComponent();
            Task.Run(async () =>
            {
                while (license.Price.IsNullorEmpty())
                    await Task.Delay(100);
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
                 {
                     PurchaseSlider_ValueChanged(null, null);
                     TextBlock_Loaded(null, null);
                     Button_Loaded(null, null);
                     PurchaseSlider.ValueChanged += PurchaseSlider_ValueChanged;
                     LoadingPanel.Visibility = Visibility.Collapsed;
                     LoadingRing.IsActive = false;
                 }));
            });
        }

        private ElementTheme theme;

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
            var h = PropertyChanged;
            if (h != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void License_LicenseChanged()
        {
            PurchaseButton.Visibility = Visibility.Visible;
            PurchaseRing.Visibility = Visibility.Collapsed;
            PurchaseRing.IsActive = false;
            PurchaseButton.IsEnabled = !license.IsPurchased;
            PurchaseSlider.IsEnabled = !license.IsPurchased;
            if (license.IsPurchased)
            {
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                ConfirmText.Text = loader.GetString("Thankyou");
                DollarText.Text = loader.GetString("ThankyouTitle");
            }
        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            PurchaseButton.Visibility = Visibility.Visible;
            PurchaseRing.Visibility = Visibility.Collapsed;
            PurchaseRing.IsActive = false;
            PurchaseButton.IsEnabled = !license.IsPurchased;
            PurchaseSlider.IsEnabled = !license.IsPurchased;
            if (license.IsPurchased)
            {
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                ConfirmText.Text = loader.GetString("Thankyou");
                DollarText.Text = loader.GetString("ThankyouTitle");
            }
        }

        private async void PurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!license.IsPurchased)
            {
                try
                {
                    // The customer doesn't own this feature, so 
                    // show the purchase dialog.
                    PurchaseButton.Visibility = Visibility.Collapsed;
                    PurchaseRing.Visibility = Visibility.Visible;
                    PurchaseRing.IsActive = true;
                    PurchaseSlider.IsEnabled = false;
                    await license.TryPurchaseAsync(License.License.DonationPack[(int)PurchaseSlider.Value]);
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                    ConfirmText.Text = loader.GetString("Obtain");
                    if (license.IsPurchased)
                    {
                        Button_Loaded(null, null);
                        ConfirmText.Text = loader.GetString("Thankyou");
                        DollarText.Text = loader.GetString("ThankyouTitle");
                    }
                    else
                    {
                        ConfirmText.Text = loader.GetString("Error");
                        DollarText.Text = loader.GetString("Retry");
                        PurchaseButton.Visibility = Visibility.Visible;
                        PurchaseRing.Visibility = Visibility.Collapsed;
                        PurchaseRing.IsActive = false;
                        PurchaseSlider.IsEnabled = true;
                    }
                    //Check the license state to determine if the in-app purchase was successful.
                }
                catch (Exception)
                {
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                    PurchaseButton.Visibility = Visibility.Visible;
                    PurchaseRing.Visibility = Visibility.Collapsed;
                    PurchaseRing.IsActive = false;
                    PurchaseSlider.IsEnabled = true;
                    var d = new MessageDialog(loader.GetString("Retry"), loader.GetString("Error"));
                    await d.ShowAsync();
                }
            }
            else
            {
                PurchaseButton.Visibility = Visibility.Visible;
                PurchaseRing.Visibility = Visibility.Collapsed;
                PurchaseRing.IsActive = false;
                PurchaseButton.IsEnabled = !license.IsPurchased;
                PurchaseSlider.IsEnabled = !license.IsPurchased;
                if (license.IsPurchased)
                {
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                    ConfirmText.Text = loader.GetString("Thankyou");
                    DollarText.Text = loader.GetString("ThankyouTitle");
                }
            }
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var obtain = loader.GetString("Obtain");
            ConfirmText.Text = obtain;
        }

        private void PurchaseSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            DollarText.Text = license.Price[(int)PurchaseSlider.Value];
        }
    }
}
