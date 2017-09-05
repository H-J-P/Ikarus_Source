using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for F5E_FUEL_FLOW.xaml
    /// </summary>
    public partial class F5E_FUEL_FLOW : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double fuelL = 0.0;
        double lfuelL = 0.0;
        double fuelR = 0.0;
        double lfuelR = 0.0;

        RotateTransform rtFuelL = new RotateTransform();
        RotateTransform rtFuelR = new RotateTransform();

        public F5E_FUEL_FLOW()
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

                               if (vals.Length > 0) { fuelL = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { fuelR = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (fuelL < 0.0) fuelL = 0.0;
                               if (fuelR < 0.0) fuelR = 0.0;

                               if (lfuelL != fuelL)
                               {
                                   rtFuelL.Angle = fuelL * 308;
                                   FUEL_FLOW_L.RenderTransform = rtFuelL;
                               }
                               if (lfuelR != fuelR)
                               {
                                   rtFuelR.Angle = fuelR * 308;
                                   FUEL_FLOW_R.RenderTransform = rtFuelR;
                               }
                               lfuelL = fuelL;
                               lfuelR = fuelR;
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
