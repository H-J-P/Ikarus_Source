using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG15_CabinP.xaml
    /// </summary>
    public partial class MIG15_CabinP : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double cockpitAltitude = 0.0;
        double pressureDifference = 0.0;

        double lcockpitAltitude = 0.0;
        double lpressureDifference = 0.0;

        RotateTransform rtcockpitAltitude = new RotateTransform();
        RotateTransform rtpressureDifference = new RotateTransform();

        public MIG15_CabinP()
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

                               if (vals.Length > 0) { cockpitAltitude = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { pressureDifference = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (cockpitAltitude < 0.0) cockpitAltitude = 0.0;
                               if (pressureDifference < 0.0) pressureDifference = 0.0;

                               if (lcockpitAltitude != cockpitAltitude)
                               {
                                   rtcockpitAltitude.Angle = cockpitAltitude * 140;
                                   CockpitAltitude.RenderTransform = rtcockpitAltitude;
                               }
                               if (lpressureDifference != pressureDifference)
                               {
                                   rtpressureDifference.Angle = pressureDifference * 105;
                                   PressureDifference.RenderTransform = rtpressureDifference;
                               }
                               lcockpitAltitude = cockpitAltitude;
                               lpressureDifference = pressureDifference;
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
