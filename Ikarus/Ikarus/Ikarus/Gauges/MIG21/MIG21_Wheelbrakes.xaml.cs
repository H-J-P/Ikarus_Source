using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG21_Wheelbrakes.xaml
    /// </summary>
    public partial class MIG21_Wheelbrakes : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double breakL = 0.0;
        double breakR = 0.0;

        double lbreakL = 0.0;
        double lbreakR = 0.0;

        RotateTransform rtBreakL = new RotateTransform();
        RotateTransform rtBreakR = new RotateTransform();

        public MIG21_Wheelbrakes()
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

                               if (vals.Length > 0) { breakL = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { breakR = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (breakL < 0.0) breakL = 0.0;
                               if (breakR < 0.0) breakR = 0.0;

                               if (lbreakL != breakL)
                               {
                                   rtBreakL.Angle = breakL * 180;
                                   LEFT_Wheelbrake.RenderTransform = rtBreakL;
                               }
                               if (lbreakR != breakR)
                               {
                                   rtBreakR.Angle = breakR * 180;
                                   RIGHT_Wheelbrake.RenderTransform = rtBreakR;
                               }
                               lbreakL = breakL;
                               lbreakR = breakR;
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
