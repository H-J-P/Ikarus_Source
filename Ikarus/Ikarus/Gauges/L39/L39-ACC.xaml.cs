﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for L39_ACC.xaml
    /// </summary>
    public partial class L39_ACC : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        private static double range = (225 + 113);
        private double acc = 0.0;
        private double minG = 0.0;
        private double maxG = 0.0;

        private double lacc = 0.0;
        private double lminG = 0.0;
        private double lmaxG = 0.0;

        private RotateTransform rtAcc = new RotateTransform();
        private RotateTransform rtMinG = new RotateTransform();
        private RotateTransform rtMaxG = new RotateTransform();

        public L39_ACC()
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
                               // AccelerationMin.input = { -5.0,	1.0}
                               // AccelerationMin.output = { 0.31,	0.695}

                               vals = strData.Split(';');

                               if (vals.Length > 0) { acc = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { minG = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { maxG = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (acc < 0.0) acc = 0.0;
                               if (maxG < 0.0) maxG = 0.0;
                               if (minG < 0.31) minG = 0.31;
                               if (minG > 0.695) minG = 0.695;

                               minG = (minG - 0.31) / (0.695 - 0.31);

                               if (lacc != acc)
                               {
                                   rtAcc.Angle = acc * range - 113;
                                   ACCELEROMETER.RenderTransform = rtAcc;
                               }
                               if (lminG != minG)
                               {
                                   rtMinG.Angle = ((minG * (112 + 23)) - 112);
                                   MIN_G_needle.RenderTransform = rtMinG;
                               }
                               if (lmaxG != maxG)
                               {
                                   rtMaxG.Angle = maxG * range - 113;
                                   MAX_G_needle.RenderTransform = rtMaxG;
                               }
                               lacc = acc;
                               lminG = minG;
                               lmaxG = maxG;
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
