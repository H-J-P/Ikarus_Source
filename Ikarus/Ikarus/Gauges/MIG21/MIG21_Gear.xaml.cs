using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für MIG21_Gear.xaml
    /// </summary>
    public partial class MIG21_Gear : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double gearNoseUp = 0.0;
        double gearNoseDown = 0.0;
        double gearLeftUp = 0.0;
        double gearLeftDown = 0.0;
        double gearRightUp = 0.0;
        double gearRightDown = 0.0;
        double airBrake = 0.0;
        double flaps = 0.0;
        double checkGear = 0.0;

        public MIG21_Gear()
        {
            InitializeComponent();

            GEAR_NOSE_UP_LIGHT.Visibility = System.Windows.Visibility.Hidden;
            GEAR_NOSE_DOWN_LIGHT.Visibility = System.Windows.Visibility.Hidden;
            GEAR_LEFT_UP_LIGHT.Visibility = System.Windows.Visibility.Hidden;
            GEAR_LEFT_DOWN_LIGHT.Visibility = System.Windows.Visibility.Hidden;
            GEAR_RIGHT_UP_LIGHT.Visibility = System.Windows.Visibility.Hidden;
            GEAR_RIGHT_DOWN_LIGHT.Visibility = System.Windows.Visibility.Hidden;
            AIRBRAKE_LIGHT.Visibility = System.Windows.Visibility.Hidden;
            FLAPS_LIGHT.Visibility = System.Windows.Visibility.Hidden;
            CHECK_GEAR_LIGHT.Visibility = System.Windows.Visibility.Hidden;
        }

        public System.Windows.Size Size
        {
            get { return new System.Windows.Size(176, 88); }
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
            return 176.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { gearNoseUp = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { gearNoseDown = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { gearLeftUp = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { gearLeftDown = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { gearRightUp = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { gearRightDown = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { airBrake = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { flaps = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { checkGear = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }

                               GEAR_NOSE_UP_LIGHT.Visibility = (gearNoseUp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               GEAR_NOSE_DOWN_LIGHT.Visibility = (gearNoseDown > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               GEAR_LEFT_UP_LIGHT.Visibility = (gearLeftUp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               GEAR_LEFT_DOWN_LIGHT.Visibility = (gearLeftDown > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               GEAR_RIGHT_UP_LIGHT.Visibility = (gearRightUp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               GEAR_RIGHT_DOWN_LIGHT.Visibility = (gearRightDown > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               AIRBRAKE_LIGHT.Visibility = (airBrake > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               FLAPS_LIGHT.Visibility = (flaps > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               CHECK_GEAR_LIGHT.Visibility = (checkGear > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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
