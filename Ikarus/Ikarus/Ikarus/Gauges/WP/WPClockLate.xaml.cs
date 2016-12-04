using System;
using System.Windows.Controls;
using System.Data;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

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

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double currtimeHours = 0.0;
        double currtimeMinutes = 0.0;
        double currtimeSeconds = 0.0;
        double flightTimeMeterStatus = 0.0;
        double flightHours = 0.0;
        double flightMinutes = 0.0;
        double secondsMeterTimeMinutes = 0.0;
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

            if (MainWindow.editmode) MakeDraggable(this, this);
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
        }

        public string GetID() { return dataImportID; }

        private void LoadBmaps()
        {
            DataRow[] dataRows = MainWindow.dtInstruments.Select("IDInst=" + dataImportID);

            if (dataRows.Length > 0)
            {
                string frame = dataRows[0]["ImageFrame"].ToString();
                string light = dataRows[0]["ImageLight"].ToString();

                try
                {
                    if (frame.Length > 4)
                    {
                        if (File.Exists(Environment.CurrentDirectory + "\\Images\\Frames\\" + frame))
                            Frame.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + frame));
                    }
                    if (light.Length > 4)
                    {
                        if (File.Exists(Environment.CurrentDirectory + "\\Images\\Frames\\" + light))
                            Light.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + light));
                    }
                    SwitchLight(false);
                }
                catch { }
            }
        }

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
            return 167.0; // Width
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
                               if (vals.Length > 3) { flightTimeMeterStatus = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { flightHours = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { flightMinutes = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { secondsMeterTimeMinutes = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { secondsMeterTimeSeconds = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }

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
                               if (lsecondsMeterTimeMinutes != secondsMeterTimeMinutes)
                               {
                                   rtSecondsMeterTimeMinutes.Angle = secondsMeterTimeMinutes * 360;
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
                               lsecondsMeterTimeMinutes = secondsMeterTimeMinutes;
                               lsecondsMeterTimeSeconds = secondsMeterTimeSeconds;
                           }
                           catch { return; }
                       }));
        }

        private void MakeDraggable(System.Windows.UIElement moveThisElement, System.Windows.UIElement movedByElement)
        {
            System.Windows.Point originalPoint = new System.Windows.Point(0, 0), currentPoint;
            TranslateTransform trUsercontrol = new TranslateTransform(0, 0);
            bool isMousePressed = false;

            movedByElement.MouseLeftButtonDown += (a, b) =>
            {
                isMousePressed = true;
                originalPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
            };

            movedByElement.MouseLeftButtonUp += (a, b) => isMousePressed = false;
            movedByElement.MouseLeave += (a, b) => isMousePressed = false;

            movedByElement.MouseMove += (a, b) =>
            {
                if (!isMousePressed || !MainWindow.editmode) return;

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                trUsercontrol.X += currentPoint.X - originalPoint.X;
                trUsercontrol.Y += currentPoint.Y - originalPoint.Y;
                moveThisElement.RenderTransform = trUsercontrol;

                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID);
            };
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
