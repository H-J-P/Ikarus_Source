using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for WPClock_late.xaml
    /// </summary>
    public partial class WPClockLate : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double currtimeHours = 0.0;
        double currtimeMinutes = 0.0;
        double currtimeSeconds = 0.0;
        double flightTimeMeterStatus = 0.0;
        double flightHours = 0.0;
        double flightMinutes = 0.0;
        double fightSeconds = 0.0;
        double secondsMeterTimeSeconds = 0.0;

        double lcurrtimeHours = 0.0;
        double lcurrtimeMinutes = 0.0;
        double lcurrtimeSeconds = 0.0;
        double lflightTimeMeterStatus = 0.0;
        double lflightHours = 0.0;
        double lflightMinutes = 0.0;
        double lsecondsMeterTimeMinutes = 0.0;
        double lsecondsMeterTimeSeconds = 0.0;

        RotateTransform rtCurrtimeHours = new RotateTransform();
        RotateTransform rtCurrtimeMinutes = new RotateTransform();
        RotateTransform rtcurrtimeSeconds = new RotateTransform();

        RotateTransform rtFlightTimeMeterStatus = new RotateTransform();
        RotateTransform rtFlightHours = new RotateTransform();
        RotateTransform rtFlightMinutes = new RotateTransform();

        RotateTransform rtSecondsMeterTimeMinutes = new RotateTransform();
        RotateTransform rtSecondsMeterTimeSeconds = new RotateTransform();

        public WPClockLate()
        {
            InitializeComponent();
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;

            helper = new GaugesHelper(dataImportID, windowID, "Instruments");
            helper.LoadBmaps(Frame, Light);

            SwitchLight(false);

            if (MainWindow.editmode) { helper.MakeDraggable(this, this); }
        }

        public string GetID() { return dataImportID; }

        public void SwitchLight(bool _on)
        {
            Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void SetInput(string _input)
        {
        }

        public void SetOutput(string _output)
        {
        }

        public double GetSize()
        {
            return Frame.Width;
        }

        public double GetSizeY()
        {
            return Frame.Height;
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { currtimeHours = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { currtimeMinutes = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { currtimeSeconds = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { flightHours = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { flightMinutes = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { fightSeconds = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }

                               if (lcurrtimeHours != currtimeHours)
                               {
                                   rtCurrtimeHours.Angle = currtimeHours * 360;
                                   Time_hours.RenderTransform = rtCurrtimeHours;
                               }
                               if (lcurrtimeMinutes != currtimeMinutes)
                               {
                                   rtCurrtimeMinutes.Angle = currtimeMinutes * 360;
                                   Time_minutes.RenderTransform = rtCurrtimeMinutes;
                               }
                               if (lcurrtimeSeconds != currtimeSeconds)
                               {
                                   rtcurrtimeSeconds.Angle = currtimeSeconds * 360;
                                   Time_seconds.RenderTransform = rtcurrtimeSeconds;
                               }
                               if (lflightHours != flightHours)
                               {
                                   rtFlightHours.Angle = flightHours * 360;
                                   MissionTime_hours.RenderTransform = rtFlightHours;
                               }
                               if (lflightMinutes != flightMinutes)
                               {
                                   rtFlightMinutes.Angle = flightMinutes * 360;
                                   MissionTime_minutes.RenderTransform = rtFlightMinutes;
                               }
                               if (lsecondsMeterTimeMinutes != fightSeconds)
                               {
                                   rtSecondsMeterTimeMinutes.Angle = fightSeconds * 360;
                                   Chronometer_minutes.RenderTransform = rtSecondsMeterTimeMinutes;
                               }
                               //rtFlightTimeMeterStatus.Angle = flightTimeMeterStatus * 275;
                               //rtSecondsMeterTimeSeconds.Angle = secondsMeterTimeSeconds * 360;
                               //WP_needleCRS_Clock.RenderTransform = rtSecondsMeterTimeSeconds;

                               lcurrtimeHours = currtimeHours;
                               lcurrtimeMinutes = currtimeMinutes;
                               lcurrtimeSeconds = currtimeSeconds;
                               lflightTimeMeterStatus = flightTimeMeterStatus;
                               lflightHours = flightHours;
                               lflightMinutes = flightMinutes;
                               lsecondsMeterTimeMinutes = fightSeconds;
                               lsecondsMeterTimeSeconds = secondsMeterTimeSeconds;
                           }
                           catch (Exception e) { ImportExport.LogMessage(GetType().Name + " got data and failed with exception: " + e.ToString()); }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}
