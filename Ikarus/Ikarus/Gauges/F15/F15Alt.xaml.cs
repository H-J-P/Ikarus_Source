﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for F15Alt.xaml
    /// </summary>
    public partial class F15Alt : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double altBar = 0.0;
        double alt10000 = 0.0;
        double alt1000 = 0.0;
        double alt100 = 0.0;
        double alt10 = 0.0;

        double laltBar = 0.0;
        double lalt10000 = 0.0;
        double lalt1000 = 0.0;
        double lalt100 = 0.0;
        double lalt10 = 0.0;

        RotateTransform rtaltBar = new RotateTransform();
        TranslateTransform ttalt10000 = new TranslateTransform();
        TranslateTransform ttalt1000 = new TranslateTransform();
        TranslateTransform ttalt100 = new TranslateTransform();
        TranslateTransform ttalt20 = new TranslateTransform();

        public F15Alt()
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

                               if (vals.Length > 0) { altBar = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { alt10000 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { alt1000 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { alt100 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { alt10 = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               const int valueScaleIndex = 11;

                               // Alt {0.0, 1.0}                                 { 0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0 }
                               Double[] valueScale = new double[valueScaleIndex] { 0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 };
                               Double[] degreeDial = new double[valueScaleIndex] { 0.0, 36, 72, 108, 144, 180, 216, 252, 288, 324, 360 };

                               if (laltBar != altBar)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (altBar >= valueScale[n] && altBar <= valueScale[n + 1])
                                       {
                                           rtaltBar.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (altBar - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   AltBar.RenderTransform = rtaltBar;
                               }

                               if (lalt10000 != alt10000)
                               {
                                   ttalt10000.Y = alt10000 * -385;
                                   AltBar_10_000.RenderTransform = ttalt10000;
                               }
                               if (lalt1000 != alt1000)
                               {
                                   ttalt1000.Y = alt1000 * -385;
                                   AltBar_1_000.RenderTransform = ttalt1000;
                               }
                               if (lalt100 != alt100)
                               {
                                   ttalt100.Y = alt100 * -338;
                                   AltBar_100.RenderTransform = ttalt100;
                               }
                               if (lalt10 != alt10)
                               {
                                   ttalt20.Y = alt10 * -170;
                                   AltBar_20.RenderTransform = ttalt20;
                               }

                               laltBar = altBar;
                               lalt10000 = alt10000;
                               lalt1000 = alt1000;
                               lalt100 = alt100;
                               lalt10 = alt10;
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
