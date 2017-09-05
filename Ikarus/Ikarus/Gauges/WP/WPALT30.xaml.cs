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
    public partial class WPALT30 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double baroPressure = 0.0;
        double altitute_M = 0.0;
        double altitute_C = 0.0;

        double lbaroPressure = 0.0;
        double laltitute_M = 0.0;
        double laltitute_C = 0.0;

        RotateTransform rtAltBar_M = new RotateTransform();
        RotateTransform rtBaroPressure = new RotateTransform();
        RotateTransform rtAltBar_C = new RotateTransform();

        public WPALT30()
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
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { baroPressure = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { altitute_M = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { altitute_C = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (baroPressure < 0.0) baroPressure = 0.0;

                               if (laltitute_M != altitute_M)
                               {
                                   rtAltBar_M.Angle = altitute_M * 360;
                                   AltBar_M.RenderTransform = rtAltBar_M;
                               }
                               if (laltitute_C != altitute_C)
                               {
                                   rtAltBar_C.Angle = altitute_C * 360;
                                   AltBar_C.RenderTransform = rtAltBar_C;
                               }
                               if (lbaroPressure != baroPressure)
                               {
                                   rtBaroPressure.Angle = (baroPressure * -300);
                                   BasicAtmospherePressure.RenderTransform = rtBaroPressure;
                               }
                               lbaroPressure = baroPressure;
                               laltitute_M = altitute_M;
                               laltitute_C = altitute_C;
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
