﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for USACC.xaml
    /// </summary>
    public partial class US_ACC : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }
        GaugesHelper helper = null;

        RotateTransform rtGLmain = new RotateTransform();
        RotateTransform rtGLmax = new RotateTransform();
        RotateTransform rtGLmin = new RotateTransform();

        double accelerometerMain = 0.0;
        double accelerometerMin = 0.0;
        double accelerometerMax = 0.0;

        double laccelerometerMain = 0.0;
        double laccelerometerMin = 0.0;
        double laccelerometerMax = 0.0;

        public US_ACC()
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

                               if (vals.Length > 0) { accelerometerMain = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { accelerometerMin = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { accelerometerMax = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (laccelerometerMain != accelerometerMain)
                               {
                                   rtGLmain.Angle = (accelerometerMain * 337) - 112;
                                   GLoad.RenderTransform = rtGLmain;
                               }
                               if (laccelerometerMin != accelerometerMin)
                               {
                                   rtGLmax.Angle = (accelerometerMax * 337) - 112;
                                   GLmax.RenderTransform = rtGLmax;
                               }
                               if (laccelerometerMax != accelerometerMax)
                               {
                                   rtGLmin.Angle = (accelerometerMin * 337) - 112;
                                   GLmin.RenderTransform = rtGLmin;
                               }
                               laccelerometerMain = accelerometerMain;
                               laccelerometerMin = accelerometerMin;
                               laccelerometerMax = accelerometerMax;
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
