using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG15_ALT.xaml
    /// </summary>
    public partial class MIG15_ALT : UserControl, I_Ikarus
    {
        private string dataImportID = "";

        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double altituteKM = 0.0;
        double altituteMeter = 0.0;
        double baroPressure = 0.0;

        double laltituteKM = 0.0;
        double laltituteMeter = 0.0;
        double lbaroPressure = 0.0;

        RotateTransform rtaltituteKM = new RotateTransform();
        RotateTransform rtAltBar_M = new RotateTransform();
        RotateTransform rtBaroPressure = new RotateTransform();

        public MIG15_ALT()
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

                               if (vals.Length > 0) { altituteKM = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { altituteMeter = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { baroPressure = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (laltituteKM != altituteKM)
                               {
                                   rtaltituteKM.Angle = altituteKM * 360;
                                   Altimeter_km.RenderTransform = rtaltituteKM;
                               }
                               if (laltituteMeter != altituteMeter)
                               {
                                   rtAltBar_M.Angle = altituteMeter * 360;
                                   Altimeter_m.RenderTransform = rtAltBar_M;
                               }
                               if (lbaroPressure != baroPressure)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (baroPressure >= valueScale[n] && baroPressure <= valueScale[n + 1])
                                       {
                                           rtBaroPressure.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (baroPressure - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", baroPressure.ToString(), Convert.ToInt32(rtBaroPressure.Angle).ToString());
                                   }
                                   Pressure.RenderTransform = rtBaroPressure;
                               }
                               laltituteKM = altituteKM;
                               laltituteMeter = altituteMeter;
                               lbaroPressure = baroPressure;
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
