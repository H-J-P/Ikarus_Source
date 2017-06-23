using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for WPALT30.xaml
    /// </summary>
    public partial class MIG21_ALT30 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double altituteMeter = 0.0;
        double altituteKM = 0.0;
        double baroPressure = 0.0;

        double laltituteMeter = 0.0;
        double laltituteKM = 0.0;
        double lbaroPressure = 0.0;

        RotateTransform rtAltBar_M = new RotateTransform();
        RotateTransform rtaltituteKM = new RotateTransform();
        RotateTransform rtBaroPressure = new RotateTransform();

        public MIG21_ALT30()
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

                               if (vals.Length > 0) { altituteMeter = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { altituteKM = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { baroPressure = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (laltituteMeter != altituteMeter)
                               {
                                   rtAltBar_M.Angle = altituteMeter * 360;
                                   AltBar_M.RenderTransform = rtAltBar_M;
                               }
                               if (laltituteKM != altituteKM)
                               {
                                   rtaltituteKM.Angle = altituteKM * 360;
                                   AltBar_C.RenderTransform = rtaltituteKM;
                               }
                               if (lbaroPressure != baroPressure)
                               {
                                   rtBaroPressure.Angle = (baroPressure * -300) - 210;
                                   BasicAtmospherePressure.RenderTransform = rtBaroPressure;
                               }
                               laltituteMeter = altituteMeter;
                               laltituteKM = altituteKM;
                               lbaroPressure = baroPressure;
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
