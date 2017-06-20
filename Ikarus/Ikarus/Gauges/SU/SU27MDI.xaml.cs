using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for SU27MDI.xaml
    /// </summary>
    public partial class SU27MDI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double leftGear = 0.0;
        double rightGear = 0.0;
        double noseGear = 0.0;
        double gearWarningLight = 0.0;
        double flaps = 0.0;
        double slats = 0.0;
        double airbrake = 0.0;

        public SU27MDI()
        {
            InitializeComponent();

            LeftGear.Visibility = System.Windows.Visibility.Hidden;
            RightGear.Visibility = System.Windows.Visibility.Hidden;
            NoseGear.Visibility = System.Windows.Visibility.Hidden;
            GearWarningLight.Visibility = System.Windows.Visibility.Hidden;
            Flaps.Visibility = System.Windows.Visibility.Hidden;
            Slats.Visibility = System.Windows.Visibility.Hidden;
            Airbrake.Visibility = System.Windows.Visibility.Hidden;
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;
            helper = new GaugesHelper(dataImportID, windowID, "Instruments");
            if (MainWindow.editmode) { helper.MakeDraggable(this, this); }
            LoadBmaps();
        }

        public string GetID() { return dataImportID; }

        private void LoadBmaps()
        {
            string frame = "";
            string light = "";

            helper.LoadBmaps(ref frame, ref light);

            try
            {
                if (frame.Length > 4)
                    Frame.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + frame));

                if (light.Length > 4)
                    Light.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + light));

                SwitchLight(false);
            }
            catch { }
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
            return 164.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { leftGear = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { rightGear = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { noseGear = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { gearWarningLight = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { flaps = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { slats = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { airbrake = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }

                               LeftGear.Visibility = (leftGear > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               RightGear.Visibility = (rightGear > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               NoseGear.Visibility = (noseGear > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               GearWarningLight.Visibility = (gearWarningLight > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Flaps.Visibility = (flaps > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Slats.Visibility = (slats > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Airbrake.Visibility = (airbrake > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                           }
                           catch { return; };
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
