using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for WPVVI200.xaml
    /// </summary>
    public partial class WPVVI200 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double vvi = 0.0;
        double turnindicator = 0.0;
        double slipBallPosition = 0.0;

        double lvvi = 0.0;
        double lturnindicator = 0.0;
        double lslipBallPosition = 0.0;

        RotateTransform rtVVI = new RotateTransform();
        RotateTransform rtTurnindicator = new RotateTransform();
        RotateTransform rtSlipBallPosition = new RotateTransform();

        public WPVVI200()
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
            return 167.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { vvi = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { turnindicator = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { slipBallPosition = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (lvvi != vvi)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (vvi >= valueScale[n] && vvi <= valueScale[n + 1])
                                       {
                                           rtVVI.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (vvi - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", vvi.ToString(), Convert.ToInt32(rtVVI.Angle).ToString());
                                   }
                                   DA200_VerticalVelocity.RenderTransform = rtVVI;
                               }
                               if (lturnindicator != turnindicator)
                               {
                                   rtTurnindicator.Angle = turnindicator * 23;
                                   DA200_TurnNeedle.RenderTransform = rtTurnindicator;
                               }
                               if (lslipBallPosition != slipBallPosition)
                               {
                                   rtSlipBallPosition.Angle = slipBallPosition * 14;
                                   DA200_Slipball.RenderTransform = rtSlipBallPosition;
                               }
                               lvvi = vvi;
                               lturnindicator = turnindicator;
                               lslipBallPosition = slipBallPosition;
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
