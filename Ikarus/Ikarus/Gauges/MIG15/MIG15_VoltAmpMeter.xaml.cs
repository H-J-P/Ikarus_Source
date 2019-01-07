using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG15_VoltAmpMeter.xaml
    /// </summary>
    public partial class MIG15_VoltAmpMeter : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double voltAmperMeter = 0.0;
        double lampsLightness = 0.0;

        double lvoltAmperMeter = 0.0;
        double llampsLightness = 0.0;

        RotateTransform rtVoltAmperMeter = new RotateTransform();

        public MIG15_VoltAmpMeter()
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

                               if (vals.Length > 0) { voltAmperMeter = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { lampsLightness = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (lvoltAmperMeter != voltAmperMeter)
                               {
                                   rtVoltAmperMeter.Angle = (voltAmperMeter < 0.0) ? voltAmperMeter * 30 : voltAmperMeter * 90;
                                   VoltAmperMeter.RenderTransform = rtVoltAmperMeter;
                               }
                               lamps_lightness.Visibility = (lampsLightness > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lvoltAmperMeter = voltAmperMeter;
                               llampsLightness = lampsLightness;
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
