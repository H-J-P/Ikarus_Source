using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for AV8BNA_Nozzle.xaml
    /// </summary>
    public partial class M2KC_AP_Altitude : UserControl, I_Ikarus
    {
        private string dataImportID = "";

        GaugesHelper helper = null;
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }

        double alt10000 = 0.0;
        double alt1000 = 0.0;
        double alt100 = 0.0;
        double lalt10000 = 0.0;
        double lalt1000 = 0.0;
        double lalt100 = 0.0;

        TranslateTransform ttAPalt10000 = new TranslateTransform();
        TranslateTransform ttAPalt1000 = new TranslateTransform();
        TranslateTransform ttAPalt100 = new TranslateTransform();

        public M2KC_AP_Altitude()
        {
            InitializeComponent();

            //shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { alt10000 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { alt1000 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { alt100 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               alt10000 = 1 - alt10000;
                               alt1000 = 1 - alt1000;
                               alt100 = 1 - alt100;

                               if (lalt10000 != alt10000)
                               {
                                   ttAPalt10000.Y = alt10000 * -429;
                                   APALT_10_000.RenderTransform = ttAPalt10000;
                               }
                               if (lalt1000 != alt1000)
                               {
                                   ttAPalt1000.Y = alt1000 * -429;
                                   APALT_1_000.RenderTransform = ttAPalt1000;
                               }
                               if (lalt100 != alt100)
                               {
                                   ttAPalt100.Y = alt100 * -429;
                                   APALT_100.RenderTransform = ttAPalt100;
                               }
                               lalt10000 = alt10000;
                               lalt1000 = alt1000;
                               lalt100 = alt100;
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
