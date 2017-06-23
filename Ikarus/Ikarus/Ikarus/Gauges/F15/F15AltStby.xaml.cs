using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for F15AltStby.xaml
    /// </summary>
    public partial class F15AltStby : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        RotateTransform rtalt100 = new RotateTransform();
        RotateTransform rtalt1000 = new RotateTransform();
        RotateTransform rtalt10000 = new RotateTransform();

        double alt10000 = 0.0;
        double alt1000 = 0.0;
        double alt100 = 0.0;

        double lalt10000 = 0.0;
        double lalt1000 = 0.0;
        double lalt100 = 0.0;

        public F15AltStby()
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
            return 156.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { alt10000 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { alt1000 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { alt100 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (lalt100 != alt100)
                               {
                                   rtalt100.Angle = alt100 * 360;
                                   AltBar_100.RenderTransform = rtalt100;
                               }
                               if (lalt1000 != alt1000)
                               {
                                   rtalt1000.Angle = alt1000 * 360;
                                   AltBar_1000.RenderTransform = rtalt1000;
                               }
                               if (lalt10000 != alt10000)
                               {
                                   rtalt10000.Angle = alt10000 * 360;
                                   AltBar_10000.RenderTransform = rtalt10000;
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
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
