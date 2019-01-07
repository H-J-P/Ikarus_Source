using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for L39_VoltAampere.xaml
    /// </summary>
    public partial class L39_VoltAmpere : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }
        GaugesHelper helper = null;

        double volt = 0.0;
        double amp = 0.0;

        double lvolt = 0.0;
        double lamp = 0.0;

        RotateTransform rtVolt = new RotateTransform();
        RotateTransform rtAmp = new RotateTransform();

        public L39_VoltAmpere()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            rtAmp.Angle = + 45;
            Ampere.RenderTransform = rtAmp;
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
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { volt = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { amp = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (volt < 0) volt = 0;
                               if (amp < 0) amp = 0;

                               if (lvolt != volt)
                               {
                                   rtVolt.Angle = 180 * volt;
                                   Volt.RenderTransform = rtVolt;
                               }

                               if (lamp != amp)
                               {
                                   rtAmp.Angle = (- 180 * amp) + 45;
                                   Ampere.RenderTransform = rtAmp;
                               }
                               lvolt = volt;
                               lamp = amp;
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
