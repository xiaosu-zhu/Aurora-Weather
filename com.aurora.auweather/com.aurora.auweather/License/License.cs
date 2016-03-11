using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace Com.Aurora.AuWeather.License
{
    internal class License
    {
        public const string DonationPack = "2";
        private LicenseInformation licenseInformation;
        private LicenseChangedEventHandler licenseChangeHandler;

        public bool IsPurchased
        {
            get
            {
                return licenseInformation.ProductLicenses[DonationPack].IsActive;
            }
        }

        public event LicenseChangedEventHandler LicenseChanged;

        public License()
        {
            Reload();
            licenseChangeHandler = new LicenseChangedEventHandler(licenseChangedEventHandler);
            CurrentAppSimulator.LicenseInformation.LicenseChanged += licenseChangeHandler;
        }

        private void Reload()
        {
            licenseInformation = CurrentAppSimulator.LicenseInformation;
        }

        private void licenseChangedEventHandler()
        {
            this.OnLicenseChanged();
        }

        private void OnLicenseChanged()
        {
            var h = this.LicenseChanged;
            if (h != null)
            {
                h();
            }
        }

        internal async Task TryPurchaseAsync(string donationPack)
        {
            await CurrentAppSimulator.RequestProductPurchaseAsync(donationPack);
        }
    }
}
