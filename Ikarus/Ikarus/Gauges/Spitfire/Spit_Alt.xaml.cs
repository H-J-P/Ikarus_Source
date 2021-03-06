﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for Spit_Alt.xaml
    /// </summary>
    public partial class Spit_Alt : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        double pressure = 0.0;
        double alt100 = 0.0;
        double alt1000 = 0.0;
        double alt10000 = 0.0;
        double lpressure = 0.0;
        double lalt100 = 0.0;
        double lalt1000 = 0.0;
        double lalt10000 = 0.0;

        RotateTransform rtpressure = new RotateTransform();
        RotateTransform rtalt100 = new RotateTransform();
        RotateTransform rtalt1000 = new RotateTransform();
        RotateTransform rtalt10000 = new RotateTransform();

        public int GetWindowID() { return windowID; }

        public Spit_Alt()
		{
			this.InitializeComponent();
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

                               if (vals.Length > 0) { alt100 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { alt1000 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { alt10000 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { pressure = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (pressure < 0.0) { pressure = 0.0; }

                               if (lalt100 != alt100)
                               {
                                   rtalt100.Angle = alt100 * 360;
                                   ALT_100.RenderTransform = rtalt100;
                               }

                               if (lalt1000 != alt1000)
                               {
                                   rtalt1000.Angle = alt1000 * 360;
                                   ALT_1_000.RenderTransform = rtalt1000;
                               }

                               if (lalt10000 != alt10000)
                               {
                                   rtalt10000.Angle = alt10000 * 360;
                                   ALT_10_000.RenderTransform = rtalt10000;
                               }

                               if (lpressure != pressure)
                               {
                                   rtpressure.Angle = pressure * 346;
                                   Pressure.RenderTransform = rtpressure;
                               }
                               lalt100 = alt100;
                               lalt1000 = alt1000;
                               lalt10000 = alt10000;
                               lpressure = pressure;
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