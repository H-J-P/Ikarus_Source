using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MI8_EngPower.xaml
    /// </summary>
    public partial class MI8_EngPower : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double limit = 0.0;
        double leftEngine = 0.0;
        double rightEngine = 0.0;

        double llimit = 0.0;
        double lleftEngine = 0.0;
        double lrightEngine = 0.0;

        TranslateTransform ttLimit = new TranslateTransform();
        TranslateTransform ttLeftEngine = new TranslateTransform();
        TranslateTransform ttRightEngine = new TranslateTransform();

        public MI8_EngPower()
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
            return 170.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { limit = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { leftEngine = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { rightEngine = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (llimit != limit)
                               {
                                   ttLimit.Y = limit * -100;
                                   Limit.RenderTransform = ttLimit;
                               }
                               if (lleftEngine != leftEngine)
                               {
                                   ttLeftEngine.Y = leftEngine * -130;
                                   Left_EnginePower.RenderTransform = ttLeftEngine;
                               }
                               if (lrightEngine != rightEngine)
                               {
                                   ttRightEngine.Y = rightEngine * -130;
                                   Right_EnginePower.RenderTransform = ttRightEngine;
                               }
                               llimit = limit;
                               lleftEngine = leftEngine;
                               lrightEngine = rightEngine;
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
