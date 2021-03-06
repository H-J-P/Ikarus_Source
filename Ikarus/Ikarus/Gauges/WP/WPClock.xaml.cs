﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

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
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double time_hours = 0.0;
        double time_minutes = 0.0;
        double time_seconds = 0.0;
        double flag = 0.0;
        double missionTime_hours = 0.0;
        double missionTime_minutes = 0.0;
        double missionTime_Seconds = 0.0;

        double ltime_hours = 0.0;
        double ltime_minutes = 0.0;
        double ltime_seconds = 0.0;
        double lflag = 0.0;
        double lmissionTime_hours = 0.0;
        double lmissionTime_minutes = 0.0;
        double lmissionTime_Seconds = 0.0;

        RotateTransform rtTime_hours = new RotateTransform();
        RotateTransform rtTime_minutes = new RotateTransform();
        RotateTransform rtTime_seconds = new RotateTransform();
        RotateTransform rtFlag = new RotateTransform();
        RotateTransform rtMissionTime_hours = new RotateTransform();
        RotateTransform rtMissionTime_minutes = new RotateTransform();
        RotateTransform rtMissionTime_second = new RotateTransform();

        public WPClock()
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

                               if (vals.Length > 0) { time_hours = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { time_minutes = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { time_seconds = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { flag = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { missionTime_hours = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { missionTime_minutes = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { missionTime_Seconds = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }

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
                               if (lflag != flag)
                               {
                                   rtFlag.Angle = flag * 360;
                                   WP_needleMTS_Clock.RenderTransform = rtFlag;
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
                               if (lmissionTime_Seconds != missionTime_Seconds)
                               {
                                   rtMissionTime_second.Angle = missionTime_Seconds * 360;
                                   WP_needleCRS_Clock.RenderTransform = rtMissionTime_second;
                               }
                               ltime_hours = time_hours;
                               ltime_minutes = time_minutes;
                               ltime_seconds = time_seconds;
                               lflag = flag;
                               lmissionTime_hours = missionTime_hours;
                               lmissionTime_minutes = missionTime_minutes;
                               lmissionTime_Seconds = missionTime_Seconds;
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
