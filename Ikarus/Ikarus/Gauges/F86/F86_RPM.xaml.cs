using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for F86_RPM.xaml
    /// </summary>
    public partial class F86_RPM : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double rpm = 0.0;
        double rpm_50 = 0.0;

        double lrpm = 0.0;
        double lrpm_50 = 0.0;

        RotateTransform rtRPM = new RotateTransform();
        RotateTransform rtRPM50 = new RotateTransform();

        public F86_RPM()
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

        public void SetOutput(string _output)
        {
        }

        public void SetInput(string _input)
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

                               if (vals.Length > 0) { rpm = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               if (rpm < 0.5)
                               {
                                   rpm_50 = rpm;
                                   rpm = 0;
                               }
                               else
                               {
                                   rpm_50 = 0.5;
                                   rpm = rpm - 0.5;
                               }
                               if (lrpm != rpm)
                               {
                                   rtRPM.Angle = rpm * (330 * 2);
                                   Tachometer_1.RenderTransform = rtRPM;
                               }
                               if (lrpm_50 != rpm_50)
                               {
                                   rtRPM50.Angle = rpm_50 * (276 * 2);
                                   Tachometer_50.RenderTransform = rtRPM50;
                               }
                               lrpm = rpm;
                               lrpm_50 = rpm_50;
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
