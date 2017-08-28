using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für AJS37_ACC.xaml
    /// </summary>
    public partial class AJS37_ACC : UserControl, I_Ikarus
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
        //double accelerationMax = 0.0;
        //double accelerationMin = 0.0;

        double lacceleration = 0.0;
        //double laccelerationMax = 0.0;
        //double laccelerationMin = 0.0;

        RotateTransform rtAcceleration = new RotateTransform();
        //RotateTransform rtAccelerationMax = new RotateTransform();
        //RotateTransform rtAccelerationMin = new RotateTransform();

        public AJS37_ACC()
        {
            InitializeComponent();

            MAX_G_needle.Visibility = System.Windows.Visibility.Hidden;
            MIN_G_needle.Visibility = System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { acceleration = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               //if (vals.Length > 1) { accelerationMax = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               //if (vals.Length > 2) { accelerationMin = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (acceleration > valueScale[valueScaleIndex - 1])
                                   acceleration = valueScale[valueScaleIndex - 1];

                               if (acceleration < valueScale[0])
                                   acceleration = valueScale[0];

                               if (lacceleration != acceleration)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (acceleration >= valueScale[n] && acceleration <= valueScale[n + 1])
                                       {
                                           rtAcceleration.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (acceleration - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   ACCELEROMETER.RenderTransform = rtAcceleration;

                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", acceleration.ToString(), Convert.ToInt32(rtAcceleration.Angle).ToString());
                                   }
                               }
                               lacceleration = acceleration;
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
