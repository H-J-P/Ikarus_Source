﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG21_AOA.xaml
    /// </summary>
    public partial class MIG21_AOA : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double uuaIndicator = 0.0;
        double luuaIndicator = 0.0;

        public MIG21_AOA()
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
            string[] vals = _input.Split(',');

            if (vals.Length < 3) return;

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

            if (vals.Length < 3) return;

            degreeDial = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                degreeDial[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
        }

        public double GetSize()
        {
            return 170.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           const int valueScaleIndex = 3;

                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { uuaIndicator = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               if (luuaIndicator != uuaIndicator)
                               {
                                   // 105 UUA_indicator.input  =                     { -0.1745, 0, 0.6108 } 
                                   //double[] valueScale = new double[valueScaleIndex] { -0.2857, 0, 1.0 };
                                   //double[] degreeDial = new double[valueScaleIndex] { -55, 0, 194 };

                                   RotateTransform rtuuaIndicator = new RotateTransform();

                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (uuaIndicator >= valueScale[n] && uuaIndicator <= valueScale[n + 1])
                                       {
                                           rtuuaIndicator.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (uuaIndicator - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", uuaIndicator.ToString(), Convert.ToInt32(uuaIndicator).ToString());
                                   }
                                   UUA_indicator.RenderTransform = rtuuaIndicator;
                               }
                               luuaIndicator = uuaIndicator;
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
