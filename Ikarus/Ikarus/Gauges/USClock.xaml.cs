using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Globalization;

namespace HawgTouch.GaugePack.Gauges
{
    /// <summary>
    /// Interaction logic for USClock.xaml
    /// </summary>
    public partial class USClock : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;
        public string DataImportID
        {
            get { return _DataImportID; }
            set { _DataImportID = value; }
        }

        public USClock()
        {
            InitializeComponent();
            this.six.Visibility = System.Windows.Visibility.Hidden;
        }

        public System.Windows.Size Size
        {
            get { return new System.Windows.Size(255, 255); }
        }
        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           double currtimeHours = 0.0;
                           double currtimeMinutes = 0.0;
                           double currtimeSeconds = 0.0;
                           double flightTimeMeterStatus = 0.0;
                           double flightHours = 0.0;
                           double flightMinutes = 0.0;
                           double cronoMinutes = 0.0;
                           double cronoSeconds = 0.0;

                           try
                           {
                               if (vals.Length > 0) { currtimeHours = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { currtimeMinutes = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { currtimeSeconds = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { flightTimeMeterStatus = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { flightHours = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { flightMinutes = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { cronoMinutes = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { cronoSeconds = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                           }
                           catch { return; }

                           RotateTransform rtCurrtimeHours = new RotateTransform();
                           RotateTransform rtCurrtimeMinutes = new RotateTransform();
                           RotateTransform rtcurrtimeSeconds = new RotateTransform();

                           RotateTransform rtFlightTimeMeterStatus = new RotateTransform();
                           RotateTransform rtFlightHours = new RotateTransform();
                           RotateTransform rtFlightMinutes = new RotateTransform();

                           RotateTransform rtCronoMinutes = new RotateTransform();
                           RotateTransform rtCronoSeconds = new RotateTransform();

                           rtCurrtimeHours.Angle = currtimeHours * 360;
                           rtCurrtimeMinutes.Angle = currtimeMinutes * 360;
                           rtcurrtimeSeconds.Angle = currtimeSeconds * 360;

                           rtFlightTimeMeterStatus.Angle = flightTimeMeterStatus * 275;
                           rtFlightHours.Angle = flightHours * 360;
                           rtFlightMinutes.Angle = flightMinutes * 360;

                           rtCronoMinutes.Angle = cronoMinutes * 360;
                           rtCronoSeconds.Angle = cronoSeconds * 360;

                           this.Time_Hour.RenderTransform = rtCurrtimeHours;
                           this.Time_Minute.RenderTransform = rtCurrtimeMinutes;
                           this.Time_Second.RenderTransform = rtcurrtimeSeconds;

                           this.Crono_Minutes.RenderTransform = rtCronoMinutes;
                           this.Crono_Second.RenderTransform = rtCronoSeconds;

                       }));
        }
    }
}
