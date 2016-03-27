using Com.Aurora.AuWeather.Models.Settings;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
            var p = Preferences.Get();
            Theme = p.GetTheme();
            license.LicenseChanged += License_LicenseChanged;
            this.InitializeComponent();
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
            PurchaseButton.IsEnabled = !license.IsPurchased;
            if (license.IsPurchased)
            {
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                ConfirmText.Text = loader.GetString("Thankyou");
                DollarText.Text = loader.GetString("ThankyouTitle");
            }
        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            PurchaseButton.IsEnabled = !license.IsPurchased;
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

                    await license.TryPurchaseAsync(License.License.DonationPack[(int)PurchaseSlider.Value]);
                    if (license.IsPurchased)
                    {
                        var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                        Button_Loaded(null, null);
                        ConfirmText.Text = loader.GetString("Thankyou");
                        DollarText.Text = loader.GetString("ThankyouTitle");
                    }
                    //Check the license state to determine if the in-app purchase was successful.
                }
                catch (Exception)
                {

                }
            }
            else
            {
                PurchaseButton.IsEnabled = !license.IsPurchased;
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
        }

        private void PurchaseSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            DollarText.Text = license.Price[(int)PurchaseSlider.Value];
        }
    }
}
