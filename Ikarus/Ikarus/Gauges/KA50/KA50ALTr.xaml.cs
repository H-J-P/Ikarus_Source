﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for KA50ALTr.xaml
    /// </summary>
    public partial class KA50ALTr : UserControl, I_Ikarus
    {
        private string dataImportID = "";

        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double rAltimeter = 0.0;
        double dangerAltimeter = 0.0;
        double warningFlag = 0.0;
        double dangerAltimeterLamp = 0.0;

        double lrAltimeter = 0.0;
        double ldangerAltimeter = 0.0;
        double lwarningFlag = 0.0;
        double ldangerAltimeterLamp = 0.0;

        RotateTransform rtrAltimeter = new RotateTransform();
        RotateTransform rtDangerAltimeter = new RotateTransform();
        RotateTransform rtWarningFlag = new RotateTransform();

        public KA50ALTr()
        {
            InitializeComponent();

            KA50_knobAl_ALTr_Alarm.Visibility = System.Windows.Visibility.Hidden;

            rtDangerAltimeter.Angle = -203;
            KA50_needleAl_ALTr.RenderTransform = rtDangerAltimeter;

            Input.Visibility = System.Windows.Visibility.Hidden;
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;
            helper = new GaugesHelper(dataImportID, windowID, "Instruments");
            if (MainWindow.editmode) { helper.MakeDraggable(this, this); }
            LoadBmaps();
        }

        public string GetID() { return dataImportID; }

        private void LoadBmaps()
        {
            string frame = "";
            string light = "";

            helper.LoadBmaps(ref frame, ref light);

            try
            {
                if (frame.Length > 4)
                    Frame.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + frame));

                if (light.Length > 4)
                    Light.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + light));

                SwitchLight(false);
            }
            catch { }
        }

        public void SwitchLight(bool _on)
        {
            Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void SetInput(string _input)
        {
            string[] vals = _input.Split(',');

            if (vals.Length < 3) { return; }

            valueScale = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                valueScale[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
            valueScaleIndex = vals.Length;
        }

        public void SetOutput(string _output)
        {
            string[] vals = _output.Split(',');

            if (vals.Length < 3) { return; }

            degreeDial = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                degreeDial[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
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

                               if (vals.Length > 0) { rAltimeter = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { dangerAltimeter = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { warningFlag = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { dangerAltimeterLamp = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (lrAltimeter != rAltimeter)
                               {
                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (rAltimeter > valueScale[n] && rAltimeter <= valueScale[n + 1])
                                       {
                                           rtrAltimeter.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (rAltimeter - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", rAltimeter.ToString(), Convert.ToInt32(rtrAltimeter.Angle).ToString());
                                   }
                                   KA50_needle_ALTr.RenderTransform = rtrAltimeter;
                               }
                               if (ldangerAltimeter != dangerAltimeter)
                               {
                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (dangerAltimeter > valueScale[n] && dangerAltimeter <= valueScale[n + 1])
                                       {
                                           rtDangerAltimeter.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (dangerAltimeter - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   rtDangerAltimeter.Angle -= 203;
                                   KA50_needleAl_ALTr.RenderTransform = rtDangerAltimeter;
                               }
                               if (lwarningFlag != warningFlag)
                               {
                                   rtWarningFlag.Angle = warningFlag * 12;
                                   KA50_needleOFF_ALTr.RenderTransform = rtWarningFlag;
                               }
                               KA50_knobAl_ALTr_Alarm.Visibility = dangerAltimeterLamp > 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               Input.Text = rAltimeter.ToString();

                               lrAltimeter = rAltimeter;
                               ldangerAltimeter = dangerAltimeter;
                               lwarningFlag = warningFlag;
                               ldangerAltimeterLamp = dangerAltimeterLamp;
                           }
                           catch { return; }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
