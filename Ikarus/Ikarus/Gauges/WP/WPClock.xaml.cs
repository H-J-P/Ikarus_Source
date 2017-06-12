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
    /// Interaction logic for WPClock.xaml
    /// </summary>
    public partial class WPClock : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double time_hours = 0.0;
        double time_minutes = 0.0;
        double time_seconds = 0.0;
        double chronometer_minutes = 0.0;
        double missionTime_hours = 0.0;
        double missionTime_minutes = 0.0;
        double missionTimeInSeconds = 0.0;

        double ltime_hours = 0.0;
        double ltime_minutes = 0.0;
        double ltime_seconds = 0.0;
        double lchronometer_minutes = 0.0;
        double lmissionTime_hours = 0.0;
        double lmissionTime_minutes = 0.0;
        double lmissionTimeInSeconds = 0.0;

        RotateTransform rtTime_hours = new RotateTransform();
        RotateTransform rtChronometer_minutes = new RotateTransform();
        RotateTransform rtMissionTime_minutes = new RotateTransform();
        RotateTransform rtMissionTime_hours = new RotateTransform();
        RotateTransform rtTime_seconds = new RotateTransform();
        RotateTransform rtTime_minutes = new RotateTransform();

        public WPClock()
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
            return 181.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { time_hours = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { time_minutes = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { time_seconds = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { chronometer_minutes = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { missionTime_hours = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { missionTime_minutes = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { missionTimeInSeconds = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }

                               if (ltime_hours != time_hours)
                               {
                                   rtTime_hours.Angle = time_hours * 360;
                                   WP_needleH_Clock.RenderTransform = rtTime_hours;
                               }
                               if (ltime_minutes != time_minutes)
                               {
                                   rtTime_minutes.Angle = time_minutes * 360;
                                   WP_needleM_Clock.RenderTransform = rtTime_minutes;
                               }
                               if (ltime_seconds != time_seconds)
                               {
                                   rtTime_seconds.Angle = time_seconds * 360;
                                   WP_needleS_Clock.RenderTransform = rtTime_seconds;
                               }
                               if (lmissionTime_hours != missionTime_hours)
                               {
                                   rtMissionTime_hours.Angle = missionTime_hours * 360;
                                   WP_needleMTH_Clock.RenderTransform = rtMissionTime_hours;
                               }
                               if (lmissionTime_minutes != missionTime_minutes)
                               {
                                   rtMissionTime_minutes.Angle = missionTime_minutes * 360;
                                   WP_needleMTM_Clock.RenderTransform = rtMissionTime_minutes;
                               }
                               if (lchronometer_minutes != chronometer_minutes)
                               {
                                   rtChronometer_minutes.Angle = chronometer_minutes * 360;
                                   WP_needleMTS_Clock.RenderTransform = rtChronometer_minutes;
                               }
                               ltime_hours = time_hours;
                               ltime_minutes = time_minutes;
                               ltime_seconds = time_seconds;
                               lchronometer_minutes = chronometer_minutes;
                               lmissionTime_hours = missionTime_hours;
                               lmissionTime_minutes = missionTime_minutes;
                               lmissionTimeInSeconds = missionTimeInSeconds;
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

            movedByElement.MouseLeftButtonUp += (a, b) =>
            {
                isMousePressed = false;
                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID);
            };
            movedByElement.MouseLeave += (a, b) => isMousePressed = false;

            movedByElement.MouseMove += (a, b) =>
            {
                if (!isMousePressed || !MainWindow.editmode) return;

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                trUsercontrol.X += currentPoint.X - originalPoint.X;
                trUsercontrol.Y += currentPoint.Y - originalPoint.Y;
                moveThisElement.RenderTransform = trUsercontrol;
            };
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
