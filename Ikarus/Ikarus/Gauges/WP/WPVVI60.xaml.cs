﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for WPVVI60.xaml
    /// </summary>
    public partial class WPVVI60 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        const int valueScaleIndex = 9;

        public int GetWindowID() { return windowID; }

        double value = 0.0;
        double lvalue = 0.0;

        RotateTransform rtVVI = new RotateTransform();

        public WPVVI60()
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
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               value = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture);

                               if (lvalue != value)
                               {
                                   // Display                                           -60    -40    -20     -10    0     10    20    40   60
                                   double[] valueScale = new double[valueScaleIndex] { -1.0, -0.66, -0.33, -0.165, 0.0, 0.165, 0.33, 0.66, 1.0 };
                                   double[] degreeDial = new double[valueScaleIndex] { -170, -126, -82, -44, 0, 44, 82, 126, 170 };

                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (value >= valueScale[n] && value <= valueScale[n + 1])
                                       {
                                           rtVVI.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (value - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   VVI.RenderTransform = rtVVI;
                               }
                               lvalue = value;
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
