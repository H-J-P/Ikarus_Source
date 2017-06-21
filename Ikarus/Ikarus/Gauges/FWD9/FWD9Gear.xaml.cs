using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for FWD9Gear.xaml
    /// </summary>
    public partial class FWD9Gear : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double leftGearUp = 0.0;
        double leftGearDown = 0.0;
        double rightGearUp = 0.0;
        double rightGearDown = 0.0;

        double flapsUp = 0.0;
        double flapsStart = 0.0;
        double flapsDown = 0.0;

        public FWD9Gear()
        {
            InitializeComponent();

            L_GEAR_UP.Visibility = System.Windows.Visibility.Hidden;
            L_GEAR_DOWN.Visibility = System.Windows.Visibility.Hidden;
            R_GEAR_UP.Visibility = System.Windows.Visibility.Hidden;
            R_GEAR_DOWN.Visibility = System.Windows.Visibility.Hidden;

            FLAPS_UP.Visibility = System.Windows.Visibility.Hidden;
            FLAPS_START.Visibility = System.Windows.Visibility.Hidden;
            FLAPS_DOWN.Visibility = System.Windows.Visibility.Hidden;
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
            return 130.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { leftGearUp = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { leftGearDown = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { rightGearUp = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { rightGearDown = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { flapsUp = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { flapsStart = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { flapsDown = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }

                               L_GEAR_UP.Visibility = (leftGearUp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               L_GEAR_DOWN.Visibility = (leftGearDown > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               R_GEAR_UP.Visibility = (rightGearUp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               R_GEAR_DOWN.Visibility = (rightGearDown > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               FLAPS_UP.Visibility = (flapsUp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               FLAPS_START.Visibility = (flapsStart > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               FLAPS_DOWN.Visibility = (flapsDown > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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
