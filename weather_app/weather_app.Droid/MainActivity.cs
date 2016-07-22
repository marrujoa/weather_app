using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Views;
using Android.Runtime;

namespace weather_app.Droid
{
    [Activity(Label = "Weather App",
        MainLauncher = true,
        Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

            AddTab("Weather!", Resource.Drawable.Icon, new TabFragment());
            AddTab("Location!", Resource.Drawable.Icon, new TabFragmentDos());

            if (savedInstanceState != null)
                ActionBar.SelectTab(ActionBar.GetTabAt(savedInstanceState.GetInt("tab")));

            //Button button = FindViewById<Button>(Resource.Id.weatherBtn);
            //Button currentLoc = FindViewById<Button>(Resource.Id.buttonCurrentLoc);

            //button.Click += Button_Click;
            //currentLoc.Click += (sender, e) => {
            //    Intent intent = new Intent(this, typeof(Locations));
            //    StartActivity(intent);
            //};
        }

        public void OnMenuItemSelected(int menuItemId) { }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt("tab", ActionBar.SelectedNavigationIndex);

            base.OnSaveInstanceState(outState);
        }

        void AddTab(string tabText, int iconResourceId, Fragment view) {
            var tab = ActionBar.NewTab();
            tab.SetText(tabText);
            tab.SetIcon(Resource.Drawable.Icon);
            // must set event handler before adding tab
            tab.TabSelected += delegate (object sender, ActionBar.TabEventArgs e)
            {
                var fragment = FragmentManager.FindFragmentById(Resource.Id.fragmentContainer);
                if (fragment != null)
                    e.FragmentTransaction.Remove(fragment);
                e.FragmentTransaction.Add(Resource.Id.fragmentContainer, view);
            };
            tab.TabUnselected += delegate (object sender, ActionBar.TabEventArgs e) {
                e.FragmentTransaction.Remove(view);
            };

            ActionBar.AddTab(tab);
        }

        class TabFragment : Fragment
        {
            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                base.OnCreateView(inflater, container, savedInstanceState);

                View view = inflater.Inflate(Resource.Layout.fragment_main, container, false);
                var weatherTextView = view.FindViewById<TextView>(Resource.Id.textView_fragment1);
                var mainActivity = (MainActivity) this.Activity;
                
                weatherTextView.Text = "What's your weather like? Let's find out!";
                Button weatherButton = view.FindViewById<Button>(Resource.Id.weatherAccess);

                weatherButton.Click += StartNewActivity;
                return view;
            }

            void StartNewActivity(object sender, EventArgs e)
            {
                Intent intent = new Intent(this.Activity, typeof(WeatherActivity));
                StartActivity(intent);
            }
        }

        class TabFragmentDos : Fragment
        {
            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                base.OnCreateView(inflater, container, savedInstanceState);

                View view = inflater.Inflate(Resource.Layout.fragment_location, container, false);
                var anotherTextView = view.FindViewById<TextView>(Resource.Id.textView_locFragment);
                var mainActivity = (MainActivity) this.Activity;

                anotherTextView.Text = "Check your current location by clicking below:";
                Button locationButton = view.FindViewById<Button>(Resource.Id.locButton);

                locationButton.Click += StartNewActivity;
                return view;
            }

            void StartNewActivity(object sender, EventArgs e)
            {
                Intent intent = new Intent(this.Activity, typeof(Locations));
                StartActivity(intent);
            }
        }
    }
}