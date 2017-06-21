using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG15_Pressure80.xaml
    /// MIG15_PressureFlaps
    /// MIG15_PressureGear
    /// </summary>
    public partial class MIG15_PressureAirFlaps : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double pressure = 0.0;
        double lpressure = 0.0;

        RotateTransform rtPressure = new RotateTransform();

        public MIG15_PressureAirFlaps()
        {
            InitializeComponent();

            HydraulicPressureAirGears.Visibility = System.Windows.Visibility.Hidden;
            HydraulicPressureAirFlaps.Visibility = System.Windows.Visibility.Visible;
        }

        public System.Windows.Size Size
        {
            get { return new System.Windows.Size(170, 170); }
        }

        public string GetID() { return dataImportID; }

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
            return 170.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { pressure = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               if (pressure < 0.0) pressure = 0.0;

                               if (lpressure != pressure)
                               {
                                   rtPressure.Angle = pressure * 288;
                                   Pressure_80at.RenderTransform = rtPressure;
                               }
                               lpressure = pressure;
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
