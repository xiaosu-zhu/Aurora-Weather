using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DonationPage : Page
    {
        private License.License license;

        public DonationPage()
        {
            this.InitializeComponent();
            license = new License.License();
            license.LicenseChanged += License_LicenseChanged;
        }

        private void License_LicenseChanged()
        {
            PurchaseButton.IsEnabled = !license.IsPurchased;
        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            PurchaseButton.IsEnabled = !license.IsPurchased;
        }

        private async void PurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!license.IsPurchased)
            {
                try
                {
                    // The customer doesn't own this feature, so 
                    // show the purchase dialog.

                    await license.TryPurchaseAsync(License.License.DonationPack);
                    if (license.IsPurchased)
                    {
                        Button_Loaded(null, null);
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
            }
        }
    }
}
