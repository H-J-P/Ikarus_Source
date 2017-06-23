using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for KA50EngRPM.xaml
    /// </summary>
    public partial class KA50EngRPM : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double leftEngineRPM = 0.0;
        double rightEngineRPM = 0.0;

        double lleftEngineRPM = 0.0;
        double lrightEngineRPM = 0.0;

        RotateTransform rtLeftEngRPM = new RotateTransform();
        RotateTransform rtRightEngRPM = new RotateTransform();

        public KA50EngRPM()
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
            return 167.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { leftEngineRPM = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { rightEngineRPM = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (leftEngineRPM < 0.0) leftEngineRPM = 0.0;
                               if (rightEngineRPM < 0.0) rightEngineRPM = 0.0;

                               if (lleftEngineRPM != leftEngineRPM)
                               {
                                   rtLeftEngRPM.Angle = leftEngineRPM * 345;
                                   KA50_needle1_RevE.RenderTransform = rtLeftEngRPM;
                               }
                               if (lrightEngineRPM != rightEngineRPM)
                               {
                                   rtRightEngRPM.Angle = rightEngineRPM * 345;
                                   KA50_needle2_RevE.RenderTransform = rtRightEngRPM;
                               }
                               lleftEngineRPM = leftEngineRPM;
                               lrightEngineRPM = rightEngineRPM;
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
