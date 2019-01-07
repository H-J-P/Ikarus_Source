using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG21_RPM.xaml
    /// </summary>
    public partial class MIG21_RPM : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double engineRPM = 0.0;
        double engineRPM2 = 0.0;

        double lengineRPM = 0.0;
        double lengineRPM2 = 0.0;

        RotateTransform rtEngineRPM = new RotateTransform();
        RotateTransform rtEngineRPM2 = new RotateTransform();

        public MIG21_RPM()
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

                               if (vals.Length > 0) { engineRPM = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { engineRPM2 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (engineRPM < 0.0) engineRPM = 0.0;
                               if (engineRPM2 < 0.0) engineRPM2 = 0.0;

                               if (lengineRPM != engineRPM)
                               {
                                   rtEngineRPM.Angle = engineRPM * 345;
                                   ENGINE_RPM.RenderTransform = rtEngineRPM;
                               }
                               if (lengineRPM2 != engineRPM2)
                               {
                                   rtEngineRPM2.Angle = engineRPM2 * 345;
                                   ENGINE_RPM2.RenderTransform = rtEngineRPM2;
                               }
                               lengineRPM = engineRPM;
                               lengineRPM2 = engineRPM2;
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
