using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for F86_RMI.xaml
    /// </summary>
    public partial class F86_RMI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double bearing = 0.0;
        double scale = 0.0;

        double lbearing = 0.0;
        double lscale = 0.0;

        RotateTransform rtBearing = new RotateTransform();
        RotateTransform rtScale = new RotateTransform();

        public F86_RMI()
        {
            InitializeComponent();

            Bearing.Text = "-";
            Scale.Text = "-";

            Bearing.Visibility = System.Windows.Visibility.Visible;
            Scale.Visibility = System.Windows.Visibility.Visible;
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
            return 255.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { bearing = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { scale = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (lbearing != bearing)
                               {
                                   rtBearing.Angle = bearing * 360;
                                   ARN6_Bearing.RenderTransform = rtBearing;
                               }
                               if (lscale != scale)
                               {
                                   rtScale.Angle = scale * 360;
                                   ARN6_Scale.RenderTransform = rtScale;
                               }
                               Bearing.Text = bearing.ToString();
                               Scale.Text = scale.ToString();

                               lbearing = bearing;
                               lscale = scale;
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
