using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NextCirc.Resources;
using System.Device.Location;
using System.Diagnostics;
using System.Windows.Threading;
using System.IO.IsolatedStorage;

namespace NextCirc
{
    public partial class MainPage : PhoneApplicationPage
    {
        public List<CircStop> stopList;
        public int currentStop;
        IsolatedStorageSettings settings;
        string curStopKey = "current_stop";
        public bool ready;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            settings = IsolatedStorageSettings.ApplicationSettings;
            ready = false;

            DateTime now = DateTime.Now;

            Debugger.Log(0, "Debug", "NextCirc Version " + NextCirc.Images.About.appVersion.ToString("N1") + "\nCurrent Time: " + now.ToShortDateString() + " " + now.ToShortTimeString() + "\n\n");

            // Load saved stop
            if (settings.Contains(curStopKey))
            {
                currentStop = Convert.ToInt32(settings[curStopKey]);
                Debugger.Log(0, "Debug", "found setting = " + currentStop + "\n");
            }
            else
            {
                settings.Add(curStopKey, currentStop);
                Debugger.Log(0, "Debug", "no setting\n");
            }

            // Build list of stops
            stopList = new List<CircStop>();
            // TODO: stopList.Add(new CircStop("NextCirc", new GeoCoordinate(0, 0), 0));
            stopList.Add(new CircStop("south forty", new GeoCoordinate(38.645327, -90.312952), 0));
            stopList.Add(new CircStop("mallinckrodt (to skinker)", new GeoCoordinate(38.647021, -90.309522), 3));
            stopList.Add(new CircStop("skinker", new GeoCoordinate(38.647654, -90.30133), 6));
            stopList.Add(new CircStop("millbrook", new GeoCoordinate(38.650172, -90.311331), 10));
            stopList.Add(new CircStop("brookings", new GeoCoordinate(38.647923, -90.304025), 14));
            stopList.Add(new CircStop("mallinckrodt (to south forty)", new GeoCoordinate(38.647021, -90.309522), 17));
            this.stopPicker.ItemsSource = stopList;

            this.stopPicker.SelectedIndex = currentStop;
            ready = true;
            Debugger.Log(0, "Debug", "initialized.\n");

            chooseStop(currentStop);

            // Register timer to keep View up to date
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            if (now.Second == 0)
            {
                Debugger.Log(0, "Debug", "updating cause :00\n");
                chooseStop(currentStop);
            }
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void stopPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debugger.Log(0, "Debug", "selection changed\n");
            if (ready)
            {
                chooseStop(stopPicker.SelectedIndex);
            }
        }

        public void chooseStop(int stopIndex)
        {
            Debugger.Log(0, "Debug", "chooseStop(" + stopIndex + ")\n");
            // Update model
            currentStop = stopIndex;

            settings[curStopKey] = currentStop;
            settings.Save();
            Debugger.Log(0, "debug", "saved current stop as " + currentStop + "\n");

            CircSchedule sched = new CircSchedule(stopList[stopIndex]);
            DateTime now = DateTime.Now;
            List<DateTime> upcomingStops = new List<DateTime>();
            Debugger.Log(0, "Debug", "getting next 3 stops from CircSchedule object\n");
            for (int i = 0; i < 3; i++)
            {
                upcomingStops.Add(sched.getNextStop());

                // Find and update time display
                TextBlock timeTextBlock = this.FindName("time" + ( i + 1 )) as TextBlock;
                timeTextBlock.Text = upcomingStops[i].ToShortTimeString();

                // Build relative time text
                string relativeText = "IN ";
                TimeSpan duration = upcomingStops[i] - now;
                if (duration.Hours > 0)
                {
                    relativeText += duration.Hours.ToString() + " HOURS, ";
                }
                if (duration.Minutes == 1)
                {
                    relativeText += "1 MINUTE";
                }
                else
                {
                    relativeText += duration.Minutes.ToString() + " MINUTES";
                }

                // Find and update relative time display
                TextBlock relativeTextBlock = this.FindName("time" + ( i + 1 ) + "_note") as TextBlock;
                relativeTextBlock.Text = relativeText;
            }
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }


        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}