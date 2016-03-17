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

        public string Price
        {
            get;
            private set;
        }
        public string Name { get; private set; }

        public event LicenseChangedEventHandler LicenseChanged;

        public License()
        {
            Reload();
        }

        private async void Reload()
        {
            licenseInformation = CurrentAppSimulator.LicenseInformation;
            try
            {
                ListingInformation listing = await CurrentAppSimulator.LoadListingInformationAsync();
                var product = listing.ProductListings[DonationPack];
                Name = product.Name;
                Price = product.FormattedPrice;
                licenseChangeHandler = new LicenseChangedEventHandler(licenseChangedEventHandler);
                CurrentAppSimulator.LicenseInformation.LicenseChanged += licenseChangeHandler;
            }
            catch (Exception)
            {
                Name = "error";
                Price = "...";
            }
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
