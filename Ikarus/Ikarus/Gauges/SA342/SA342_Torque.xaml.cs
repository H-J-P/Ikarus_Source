using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für SA342_Torque.xaml
    /// </summary>
    public partial class SA342_Torque : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;

        private double readValue = 0.0;
        private double torque_Bug = 0.0;
        private double lamp = 0.0;
        private double lreadValue = 0.0;
        private double ltorque_Bug = 0.0;
        private double llamp = 0.0;

        RotateTransform rtPin = new RotateTransform();
        RotateTransform rtTorque_Bug = new RotateTransform();

        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        public SA342_Torque()
        {
            InitializeComponent();

            Warning_Light.Visibility = System.Windows.Visibility.Hidden;
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
            helper.SetInput(ref _input, ref valueScale, ref valueScaleIndex, 2);
        }

        public void SetOutput(string _output)
        {
            helper.SetOutput(ref _output, ref degreeDial, 2);
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

                               if (vals.Length > 0) { readValue = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { torque_Bug = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { lamp = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (lreadValue != readValue)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (readValue >= valueScale[n] && readValue <= valueScale[n + 1])
                                       {
                                           rtPin.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (readValue - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   Torque.RenderTransform = rtPin;

                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", readValue.ToString(), Convert.ToInt32(rtPin.Angle).ToString());
                                   }
                               }

                               if (ltorque_Bug != torque_Bug)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (torque_Bug >= valueScale[n] && torque_Bug <= valueScale[n + 1])
                                       {
                                           rtTorque_Bug.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (torque_Bug - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   Torque_Bug.RenderTransform = rtTorque_Bug;

                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "2", torque_Bug.ToString(), Convert.ToInt32(rtTorque_Bug.Angle).ToString());
                                   }
                               }
                               if (llamp != lamp)
                                   Warning_Light.Visibility = (lamp > 0.5) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lreadValue = readValue;
                               ltorque_Bug = torque_Bug;
                               llamp = lamp;
                           }
                           catch { return; }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}
