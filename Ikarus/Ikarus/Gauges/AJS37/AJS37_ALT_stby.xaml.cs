using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für AJS37_ALT_stby.xaml
    /// </summary>
    public partial class AJS37_ALT_stby : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        RotateTransform rtalt100FP = new RotateTransform();
        RotateTransform rtalt1000FP = new RotateTransform();
        TranslateTransform ttpressure_0 = new TranslateTransform();
        TranslateTransform ttpressure_1 = new TranslateTransform();
        TranslateTransform ttpressure_2 = new TranslateTransform();
        TranslateTransform ttpressure_3 = new TranslateTransform();

        double alt100FP = 0.0;
        double alt1000FP = 0.0;
        double pressure_0 = 0.0;
        double pressure_1 = 0.0;
        double pressure_2 = 0.0;
        double pressure_3 = 0.0;

        double lalt100FP = 0.0;
        double lalt1000FP = 0.0;
        double lpressure_0 = 0.0;
        double lpressure_1 = 0.0;
        double lpressure_2 = 0.0;
        double lpressure_3 = 0.0;

        public AJS37_ALT_stby()
        {
            InitializeComponent();

            rtalt100FP.Angle =  180;
            Altimeter_100.RenderTransform = rtalt100FP;
            rtalt1000FP.Angle =  180;
            Altimeter_1000.RenderTransform = rtalt1000FP;
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
                           vals = strData.Split(';');

                           try
                           {
                               if (vals.Length > 0) { alt100FP = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { alt1000FP = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { pressure_0 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { pressure_1 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { pressure_2 = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { pressure_3 = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                           }
                           catch { return; }

                           if (alt100FP != lalt100FP)
                           {
                               rtalt100FP.Angle = (alt100FP * 360) + 180;
                               Altimeter_100.RenderTransform = rtalt100FP;
                           }

                           if (alt1000FP != lalt1000FP)
                           {
                               rtalt1000FP.Angle = (alt1000FP * 360) + 180;
                               Altimeter_1000.RenderTransform = rtalt1000FP;
                           }

                           if (pressure_0 != lpressure_0)
                           {
                               ttpressure_0.Y = pressure_0 * -203;
                               ALT_P_1.RenderTransform = ttpressure_0;
                           }

                           if (pressure_1 != lpressure_1)
                           {
                               ttpressure_1.Y = pressure_1 * -203;
                               ALT_P_10.RenderTransform = ttpressure_1;
                           }

                           if (pressure_2 != lpressure_2)
                           {
                               ttpressure_2.Y = pressure_2 * -203;
                               ALT_P_100.RenderTransform = ttpressure_2;
                           }

                           if (pressure_3 != lpressure_3)
                           {
                               ttpressure_3.Y = pressure_3 * -203;
                               ALT_P_1000.RenderTransform = ttpressure_3;
                           }

                           lalt100FP = alt100FP;
                           lalt1000FP = alt1000FP;
                           lpressure_0 = pressure_0;
                           lpressure_1 = pressure_1;
                           lpressure_2 = pressure_2;
                           lpressure_3 = pressure_3;
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
