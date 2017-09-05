using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG15_FuelFlow.xaml
    /// </summary>
    public partial class MIG15_FuelFlow : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }
        GaugesHelper helper = null;

        double oilTemperature = 0.0;
        double oilPressure = 0.0;
        double engineFuelPressure = 0.0;

        double loilTemperature = 0.0;
        double loilPressure = 0.0;
        double lengineFuelPressure = 0.0;

        RotateTransform rtoilTemperature = new RotateTransform();
        RotateTransform rtoilPressure = new RotateTransform();
        RotateTransform rtengineFuelPressure = new RotateTransform();

        public MIG15_FuelFlow()
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

                               if (vals.Length > 0) { oilTemperature = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { oilPressure = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { engineFuelPressure = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (oilTemperature < 0.0) oilTemperature = 0.0;
                               if (oilPressure < 0.0) oilPressure = 0.0;
                               if (engineFuelPressure < 0.0) engineFuelPressure = 0.0;

                               if (loilTemperature != oilTemperature)
                               {
                                   rtoilTemperature.Angle = (oilTemperature * 120) - 30;
                                   OilTemperature.RenderTransform = rtoilTemperature;
                               }
                               if (loilPressure != oilPressure)
                               {
                                   rtoilPressure.Angle = oilPressure * -120;
                                   OilPressure.RenderTransform = rtoilPressure;
                               }
                               if (lengineFuelPressure != engineFuelPressure)
                               {
                                   rtengineFuelPressure.Angle = engineFuelPressure * 280;
                                   EngineFuelPressure.RenderTransform = rtengineFuelPressure;
                               }
                               loilTemperature = oilTemperature;
                               loilPressure = oilPressure;
                               lengineFuelPressure = engineFuelPressure;
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
