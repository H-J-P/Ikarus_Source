using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG15_Gear.xaml
    /// </summary>
    public partial class MIG15_Gear : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }
        GaugesHelper helper = null;

        double gearLeftDown = 0.0;
        double gearLeftUp = 0.0;
        double gearRightDown = 0.0;
        double gearRightUp = 0.0;
        double gearNoseDown = 0.0;
        double gearNoseUp = 0.0;
        double checkGear = 0.0;

        public MIG15_Gear()
        {
            InitializeComponent();

            left_up.Visibility = System.Windows.Visibility.Hidden;
            left_down.Visibility = System.Windows.Visibility.Hidden;
            right_up.Visibility = System.Windows.Visibility.Hidden;
            right_down.Visibility = System.Windows.Visibility.Hidden;
            nose_up.Visibility = System.Windows.Visibility.Hidden;
            nose_down.Visibility = System.Windows.Visibility.Hidden;
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

        public System.Windows.Size Size
        {
            get { return new System.Windows.Size(176, 88); }
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

                               if (vals.Length > 0) { gearLeftDown = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { gearLeftUp = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { gearRightDown = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { gearRightUp = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { gearNoseDown = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { gearNoseUp = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { checkGear = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }

                               left_up.Visibility = (gearLeftUp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               left_down.Visibility = (gearLeftDown > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               right_up.Visibility = (gearRightUp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               right_down.Visibility = (gearRightDown > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               nose_up.Visibility = (gearNoseUp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               nose_down.Visibility = (gearNoseDown > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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
