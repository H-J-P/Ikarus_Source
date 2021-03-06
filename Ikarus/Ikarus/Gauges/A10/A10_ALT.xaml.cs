﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for A10_ALT.xaml
    /// </summary>
    public partial class A10_ALT : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        RotateTransform rtAlt100FP = new RotateTransform();
        TranslateTransform ttAlt10000 = new TranslateTransform();
        TranslateTransform ttAlt1000 = new TranslateTransform();
        TranslateTransform ttAlt100 = new TranslateTransform();
        TranslateTransform ttPressure_0 = new TranslateTransform();
        TranslateTransform ttPressure_1 = new TranslateTransform();
        TranslateTransform ttPressure_2 = new TranslateTransform();
        TranslateTransform ttPressure_3 = new TranslateTransform();

        double alt100FP = 0.0;
        double alt10000 = 0.0;
        double alt1000 = 0.0;
        double alt100 = 0.0;
        double pressure_0 = 0.0;
        double pressure_1 = 0.0;
        double pressure_2 = 0.0;
        double pressure_3 = 0.0;
        double flag_pneu = 0.0;

        double lalt100FP = 0.0;
        double lalt10000 = 0.0;
        double lalt1000 = 0.0;
        double lalt100 = 0.0;
        double lpressure_0 = 0.0;
        double lpressure_1 = 0.0;
        double lpressure_2 = 0.0;
        double lpressure_3 = 0.0;

        public A10_ALT()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            //Flagg_Elect.Visibility = System.Windows.Visibility.Visible;
            Flagg_Elect.Visibility = System.Windows.Visibility.Hidden;
            Flagg_Pneu.Visibility = System.Windows.Visibility.Hidden;
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
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           vals = strData.Split(';');

                           try
                           {
                               if (vals.Length > 0) { alt100FP = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { alt10000 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { alt1000 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { alt100 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { pressure_0 = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { pressure_1 = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { pressure_2 = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { pressure_3 = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { flag_pneu = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }

                               if (alt100FP != lalt100FP)
                               {
                                   rtAlt100FP.Angle = alt100FP * 360;
                                   AltBar.RenderTransform = rtAlt100FP;
                               }
                               if (alt10000 != lalt10000)
                               {
                                   ttAlt10000.Y = alt10000 * -390;
                                   AltBar_10_000.RenderTransform = ttAlt10000;
                               }
                               if (alt1000 != lalt1000)
                               {
                                   ttAlt1000.Y = alt1000 * -390;
                                   AltBar_1_000.RenderTransform = ttAlt1000;
                               }
                               if (alt100 != lalt100)
                               {
                                   ttAlt100.Y = alt100 * -390;
                                   AltBar_100.RenderTransform = ttAlt100;
                               }

                               if (pressure_0 != lpressure_0)
                               {
                                   ttPressure_0.Y = pressure_0 * -180;
                                   BasicAtmospherePressure_1.RenderTransform = ttPressure_0;
                               }
                               if (pressure_1 != lpressure_1)
                               {
                                   ttPressure_1.Y = pressure_1 * -180;
                                   BasicAtmospherePressure_10.RenderTransform = ttPressure_1;
                               }
                               if (pressure_2 != lpressure_2)
                               {
                                   ttPressure_2.Y = pressure_2 * -180;
                                   BasicAtmospherePressure_100.RenderTransform = ttPressure_2;
                               }
                               if (pressure_3 != lpressure_3)
                               {
                                   ttPressure_3.Y = pressure_3 * -180;
                                   BasicAtmospherePressure_1000.RenderTransform = ttPressure_3;
                               }

                               lalt100FP = alt100FP;
                               lalt10000 = alt10000;
                               lalt1000 = alt1000;
                               lalt100 = alt100;
                               lpressure_0 = pressure_0;
                               lpressure_1 = pressure_1;
                               lpressure_2 = pressure_2;
                               lpressure_3 = pressure_3;
                           }
                           //catch { return; }
                           catch (Exception e) { ImportExport.LogMessage(GetType().Name + " got data and failed with exception: " + e.ToString()); }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}
