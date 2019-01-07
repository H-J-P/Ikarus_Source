using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG15_Brake.xaml
    /// </summary>
    public partial class MIG15_Brake : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double leftBrake = 0.0;
        double rightBrake = 0.0;

        double lleftBrake = 0.0;
        double lrightBrake = 0.0;

        RotateTransform rtRightBrake = new RotateTransform();
        RotateTransform rtLeftBrake = new RotateTransform();

        public MIG15_Brake()
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

                               if (vals.Length > 0) { leftBrake = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { rightBrake = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (leftBrake < 0.0) leftBrake = 0.0;
                               if (rightBrake < 0.0) rightBrake = 0.0;

                               if (lleftBrake != leftBrake)
                               {
                                   rtLeftBrake.Angle = leftBrake * 180;
                                   LeftBrakePressure.RenderTransform = rtLeftBrake;
                               }
                               if (lrightBrake != rightBrake)
                               {
                                   rtRightBrake.Angle = rightBrake * 180;
                                   RightBrakePressure.RenderTransform = rtRightBrake;
                               }
                               lleftBrake = leftBrake;
                               lrightBrake = rightBrake;
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
