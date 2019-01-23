using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MI8_OilEng.xaml
    /// </summary>
    public partial class MI8_OilEng : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double pressure = 0.0;
        double temperature = 0.0;

        double lpressure = 0.0;
        double ltemperature = 0.0;

        RotateTransform rtpressure = new RotateTransform();
        RotateTransform rttemperature = new RotateTransform();

        public MI8_OilEng()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;

            helper = new GaugesHelper(dataImportID, windowID, "Instruments");
            helper.LoadBmaps(Frame, Light);

            SwitchLight(false);

            if (MainWindow.editmode) { helper.MakeDraggable(this, this); }
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
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

                               if (vals.Length > 0) { pressure = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { temperature = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (pressure < 0.0) pressure = 0.0;
                               if (temperature < -0.25) temperature = -0.25; // -0.25 to 0.75

                               if (lpressure != pressure)
                               {
                                   rtpressure.Angle = pressure * -120;
                                   oils_p_X_engine.RenderTransform = rtpressure;
                               }
                               if (ltemperature != temperature)
                               {
                                   if (temperature >= 0)
                                   {
                                       rttemperature.Angle = temperature * 90 * (1 / 0.75);
                                   }
                                   if (temperature < 0)
                                   {
                                       rttemperature.Angle = temperature * 30 * (1 / 0.25);
                                   }
                                   oils_t_X_engine.RenderTransform = rttemperature;
                               }
                               lpressure = pressure;
                               ltemperature = temperature;
                           }
                           catch (Exception e) { ImportExport.LogMessage(GetType().Name + " got data and failed with exception: " + e.ToString()); }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}
