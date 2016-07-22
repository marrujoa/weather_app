using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Locations;
using Android.Util;

namespace weather_app.Droid
{
    [Activity(Label = "Current Location Finder", Icon = "@drawable/icon")]
    public class Locations : Activity, ILocationListener
    {
        static readonly string TAG = "X:" + typeof(Locations).Name;
        TextView addressText;
        Location currentLocation;
        LocationManager locationManager;

        string locationProvider;
        TextView locationText;

        public async void OnLocationChanged(Location location)
        {
            currentLocation = location;
            if (currentLocation == null)
            {
                locationText.Text = "Unable to determine your location. Try again in a short while.";
            }
            else
            {
                locationText.Text = string.Format("{0:f6},{1:f6}", currentLocation.Latitude, currentLocation.Longitude);
                Address address = await ReverseGeocodeCurrentLocation();
                DisplayAddress(address);
            }
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Log.Debug(TAG, "{0}, {1}", provider, status);
        }

        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Locations);

            addressText = FindViewById<TextView>(Resource.Id.address_text);
            locationText = FindViewById<TextView>(Resource.Id.location_text);
            FindViewById<TextView>(Resource.Id.get_address_button).Click += AddressButton_OnClick;
            Button returnButton = FindViewById<Button>(Resource.Id.returnToMain);

            // Create your application here
            InitializeLocationManager();
            returnButton.Click += (sender, e) => {
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };
        }

        public void InitializeLocationManager()
        {
            locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            Log.Debug(TAG, "Checking for break 2");

            IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                locationProvider = string.Empty;
                Log.Info(TAG, "No location providers available");
            }
            Log.Debug(TAG, "Using " + locationProvider + ".");
        }

        protected override void OnResume()
        {
            base.OnResume();
            locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
            Log.Debug(TAG, "Listening for location updates using " + locationProvider + ".");
        }

        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);
            Log.Debug(TAG, "No longer listening for location updates.");
        }

        public async void AddressButton_OnClick(object sender, EventArgs eventArgs)
        {
            if (currentLocation == null)
            {
                addressText.Text = "Can't determine location. Please try again soon.";
                Log.Debug(TAG, "Checking for break 3");
                return;
            }

            Address address = await ReverseGeocodeCurrentLocation();
            DisplayAddress(address);
        }

        public async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(currentLocation.Latitude, currentLocation.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        void DisplayAddress(Address address)
        {
            if (address != null)
            {
                StringBuilder deviceAddress = new StringBuilder();
                for (int i = 0; i < address.MaxAddressLineIndex; i++)
                {
                    deviceAddress.AppendLine(address.GetAddressLine(i));
                }
                addressText.Text = deviceAddress.ToString();
            }
            else
            {
                addressText.Text = "Unable to determine the address. Try again in a few minutes.";
            }
        }
    }
}