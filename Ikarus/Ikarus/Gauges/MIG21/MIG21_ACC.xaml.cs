using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG21_ACC.xaml
    /// </summary>
    public partial class MIG21_ACC : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double acceleration = 0.0;
        double accelerationMax = 0.0;
        double accelerationMin = 0.0;

        double lacceleration = 0.0;
        double laccelerationMax = 0.0;
        double laccelerationMin = 0.0;

        RotateTransform rtAcceleration = new RotateTransform();
        RotateTransform rtAccelerationMax = new RotateTransform();
        RotateTransform rtAccelerationMin = new RotateTransform();

        public MIG21_ACC()
        {
            InitializeComponent();

            rtAcceleration.Angle = -111;
            rtAccelerationMax.Angle = +22;
            rtAccelerationMin.Angle = -111;
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
            helper.SetInput(ref _input, ref valueScale, ref valueScaleIndex, 3);
        }

        public void SetOutput(string _output)
        {
            helper.SetOutput(ref _output, ref degreeDial, 3);
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

                               const int valueScaleIndex = 5;

                               if (vals.Length > 0) { acceleration = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { accelerationMax = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { accelerationMin = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (lacceleration != acceleration)
                               {
                                   // 110   ACCELEROMETER.input =                    {  -5.0,     1,   5,    8, 10.0 } 
                                   //double[] valueScale = new double[valueScaleIndex] { -0.41, 0.096, 0.5, 0.81, 1 };
                                   //double[] degreeDial = new double[valueScaleIndex] { -110, 23, 114, 180, 225 };

                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (acceleration >= valueScale[n] && acceleration <= valueScale[n + 1])
                                       {
                                           rtAcceleration.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (acceleration - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", acceleration.ToString(), Convert.ToInt32(rtAcceleration.Angle).ToString());
                                   }
                                   ACCELEROMETER.RenderTransform = rtAcceleration;
                               }
                               if (laccelerationMax != accelerationMax)
                               {
                                   rtAccelerationMax.Angle = (accelerationMax * 203) + 22;
                                   MAX_G_needle.RenderTransform = rtAccelerationMax;
                               }
                               if (laccelerationMin != accelerationMin)
                               {
                                   rtAccelerationMin.Angle = (accelerationMin * 133) - 111;
                                   MIN_G_needle.RenderTransform = rtAccelerationMin;
                               }
                               lacceleration = acceleration;
                               laccelerationMax = accelerationMax;
                               laccelerationMin = accelerationMin;
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
