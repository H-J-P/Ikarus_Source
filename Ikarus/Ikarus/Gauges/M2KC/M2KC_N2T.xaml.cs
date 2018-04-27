using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for M2KC_N2T.xaml
    /// </summary>
    public partial class M2KC_N2T : UserControl, I_Ikarus
    {
        private string dataImportID = "";

        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        GaugesHelper helper = null;
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }

        double rpm = 0.0;
        double rpm10 = 0.0;
        double rpm1 = 0.0;
        double temp = 0.0;

        double lpower = 0.0;
        double lrpm10 = 0.0;
        double lrpm1 = 0.0;
        double ltemp = 0.0;

        RotateTransform rtRpm = new RotateTransform();
        RotateTransform rtTemp = new RotateTransform();
        TranslateTransform ttRpm10 = new TranslateTransform();
        TranslateTransform ttRpm1 = new TranslateTransform();

        public M2KC_N2T()
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
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { rpm = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { rpm10 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { rpm1 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { temp = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (lpower != rpm)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (rpm >= valueScale[n] && rpm <= valueScale[n + 1])
                                       {
                                           rtRpm.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (rpm - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", rpm.ToString(), Convert.ToInt32(rtRpm.Angle).ToString());
                                   }
                                   N2.RenderTransform = rtRpm;
                               }

                               if (rpm10 < 0) rpm10 = 0.0;
                               if (rpm1 < 0) rpm1 = 0.0;

                               if (lrpm10 != rpm10)
                               {
                                   ttRpm10.Y = rpm10 * 370;
                                   N2_10.RenderTransform = ttRpm10;
                               }

                               if (lrpm1 != rpm1)
                               {
                                   ttRpm1.Y = rpm1 * 337;
                                   N2_1.RenderTransform = ttRpm1;
                               }

                               if (ltemp != temp)
                               {
                                   rtTemp.Angle = temp * 231;
                                   Tt7.RenderTransform = rtTemp;
                               }

                               lpower = rpm;
                               lrpm10 = rpm10;
                               lrpm1 = rpm1;
                               ltemp = temp;
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
