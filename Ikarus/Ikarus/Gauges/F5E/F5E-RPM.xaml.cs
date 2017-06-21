using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for F5E_RPM.xaml
    /// </summary>
    public partial class F5E_RPM : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double engineRPM100 = 0.0;
        double engineRPM_10 = 0.0;

        double lengineRPM100 = 0.0;
        double lengineRPM_10 = 0.0;

        RotateTransform rtEngineRPM100 = new RotateTransform();
        RotateTransform rtEngineRPM10 = new RotateTransform();

        public F5E_RPM()
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

                               if (vals.Length > 0) { engineRPM100 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { engineRPM_10 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (engineRPM100 < 0.0) { engineRPM100 = 0.0; }

                               if (lengineRPM100 != engineRPM100)
                               {
                                   rtEngineRPM100.Angle = engineRPM100 * 270;
                                   RPM.RenderTransform = rtEngineRPM100;
                               }
                               if (lengineRPM_10 != engineRPM_10)
                               {
                                   rtEngineRPM10.Angle = engineRPM_10 * 360;
                                   RPM_10.RenderTransform = rtEngineRPM10;
                               }
                               lengineRPM100 = engineRPM100;
                               lengineRPM_10 = engineRPM_10;
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
