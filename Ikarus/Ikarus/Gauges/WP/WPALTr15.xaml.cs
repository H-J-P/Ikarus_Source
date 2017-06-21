using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for WPALTr15.xaml
    /// </summary>
    public partial class WPALTr15 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        GaugesHelper helper = null;

        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }

        double rAltimeter = 0.0;
        double dangerAltimeter = 0.0;
        double offFlag = 0.0;
        double dangerAltimeterLamp = 0.0;

        double lrAltimeter = 0.0;
        double ldangerAltimeter = 0.0;
        double loffFlag = 0.0;
        double ldangerAltimeterLamp = 0.0;

        RotateTransform rtrAltimeter = new RotateTransform();
        RotateTransform rtDangerAltimeter = new RotateTransform();

        public WPALTr15()
        {
            InitializeComponent();

            Off_Flagg.Visibility = System.Windows.Visibility.Visible;
            knob_Marker_Alarm.Visibility = System.Windows.Visibility.Hidden;
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
                               if (vals.Length > 2) { offFlag = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { dangerAltimeterLamp = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               // WPALTr15                      		     =   { 0,   100,   150,   200,   300,   400,   600,   800,  1000, 1500 }
                               //double[] valueScale = new double[valueScaleIndex] { 0, 0.067, 0.100, 0.133, 0.200, 0.267, 0.400, 0.533, 0.667, 1.0 };
                               //double[] degreeDial = new double[valueScaleIndex] { 0, 104, 154, 180, 217, 241, 270, 286, 298, 317 };

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
                                   AltRad.RenderTransform = rtrAltimeter;
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
                                   Alarm_Marker.RenderTransform = rtDangerAltimeter;
                               }
                               Off_Flagg.Visibility = offFlag > 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               knob_Marker_Alarm.Visibility = dangerAltimeterLamp > 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lrAltimeter = rAltimeter;
                               ldangerAltimeter = dangerAltimeter;
                               loffFlag = offFlag;
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
