using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for UH1_AltAAU31.xaml
    /// </summary>
    public partial class UH1_AltAAU31 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double pressure = 0.0;
        double alt10000 = 0.0;
        double alt1000 = 0.0;
        double alt100 = 0.0;

        double lpressure = 0.0;
        double lalt10000 = 0.0;
        double lalt1000 = 0.0;
        double lalt100 = 0.0;

        RotateTransform rtPressure = new RotateTransform();
        RotateTransform rtAlt10000 = new RotateTransform();
        RotateTransform rtAlt1000 = new RotateTransform();
        RotateTransform rtAlt100 = new RotateTransform();

        public UH1_AltAAU31()
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

                               if (vals.Length > 0) { pressure = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { alt10000 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { alt1000 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { alt100 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (lpressure != pressure)
                               {
                                   rtPressure.Angle = (pressure * -360) - 25;
                                   Pressure.RenderTransform = rtPressure;
                               }
                               if (lalt10000 != alt10000)
                               {
                                   rtAlt10000.Angle = alt10000 * 360;
                                   Alt_10000_AAU_7A.RenderTransform = rtAlt10000;
                               }
                               if (lalt1000 != alt1000)
                               {
                                   rtAlt1000.Angle = alt1000 * 360;
                                   Alt_1000_AAU_7A.RenderTransform = rtAlt1000;
                               }
                               if (lalt100 != alt100)
                               {
                                   rtAlt100.Angle = alt100 * 360;
                                   Alt_100_AAU_7A.RenderTransform = rtAlt100;
                               }
                               lpressure = pressure;
                               lalt10000 = alt10000;
                               lalt1000 = alt1000;
                               lalt100 = alt100;
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
