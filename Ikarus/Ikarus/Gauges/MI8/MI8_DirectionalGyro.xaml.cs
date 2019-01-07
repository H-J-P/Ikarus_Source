﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MI8_DirectionalGyro.xaml
    /// </summary>
    public partial class MI8_DirectionalGyro : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        RotateTransform rtheading = new RotateTransform();
        RotateTransform rtcommandedCourse = new RotateTransform();
        RotateTransform rtbearingNeedle = new RotateTransform();

        double heading = 0.0;
        double commandedCourse = 0.0;
        double bearingNeedle = 0.0;

        double lheading = 0.0;
        double lcommandedCourse = 0.0;
        double lbearingNeedle = 0.0;

        public MI8_DirectionalGyro()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { heading = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { commandedCourse = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { bearingNeedle = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (lheading != heading)
                                   {
                                   rtheading.Angle = heading * -360;
                                   UGR_4K_heading_L.RenderTransform = rtheading;
                               }
                               if (lcommandedCourse != commandedCourse)
                               {
                                   rtcommandedCourse.Angle = commandedCourse * 360;
                                   UGR_4K_commanded_course_L.RenderTransform = rtcommandedCourse;
                               }
                               if (lbearingNeedle != bearingNeedle)
                               {
                                   rtbearingNeedle.Angle = bearingNeedle * 360;
                                   UGR_4K_bearing_needle_L.RenderTransform = rtbearingNeedle;
                               }
                               lheading = heading;
                               lcommandedCourse = commandedCourse;
                               lbearingNeedle = bearingNeedle;
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
