using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MI8_RPM_Eng.xaml
    /// </summary>
    public partial class MI8_RPM_Eng : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double leftRPM = 0.0;
        double rightRPM = 0.0;

        double lleftRPM = 0.0;
        double lrightRPM = 0.0;

        RotateTransform rtleftRPM = new RotateTransform();
        RotateTransform rtrightRPM = new RotateTransform();

        public MI8_RPM_Eng()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { leftRPM = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { rightRPM = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (leftRPM < 0.0) leftRPM = 0.0;
                               if (rightRPM < 0.0) rightRPM = 0.0;

                               if (lleftRPM != leftRPM)
                               {
                                   rtleftRPM.Angle = leftRPM * 345;
                                   LeftEngineRPM.RenderTransform = rtleftRPM;
                               }
                               if (lrightRPM != rightRPM)
                               {
                                   rtrightRPM.Angle = rightRPM * 345;
                                   RightEngineRPM.RenderTransform = rtrightRPM;
                               }
                               lleftRPM = leftRPM;
                               lrightRPM = rightRPM;
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
