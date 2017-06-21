using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for KA50ACC.xaml
    /// </summary>
    public partial class KA50ACC : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double acceleration = 0.0;
        double accelerationMax = 0.0;
        double accelerationMin = 0.0;

        double lacceleration = 0.0;
        double laccelerationMax = 0.0;
        double laccelerationMin = 0.0;

        RotateTransform rtAcceleration = new RotateTransform();
        RotateTransform rtAccelerationMax = new RotateTransform();
        RotateTransform rtAccelerationMin = new RotateTransform();

        public KA50ACC()
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

                               if (vals.Length > 0) { acceleration = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { accelerationMax = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { accelerationMin = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (lacceleration != acceleration)
                               {
                                   rtAcceleration.Angle = (acceleration * 300) - 100;
                                   KA50_needleG_Acc.RenderTransform = rtAcceleration;
                               }
                               if (laccelerationMax != accelerationMax)
                               {
                                   rtAccelerationMax.Angle = (accelerationMax * 300) - 100;
                                   KA50_needleM_Acc.RenderTransform = rtAccelerationMax;
                               }
                               if (laccelerationMin != accelerationMin)
                               {
                                   rtAccelerationMin.Angle = (accelerationMin * 300) - 100;
                                   KA50_needleP_Acc.RenderTransform = rtAccelerationMin;
                               }
                               lacceleration = acceleration;
                               laccelerationMax = accelerationMax;
                               laccelerationMin = accelerationMin;
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
