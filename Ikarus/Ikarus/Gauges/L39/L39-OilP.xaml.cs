using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for L39_OilP.xaml
    /// </summary>
    public partial class L39_OilP : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        double oilTemp = 0.0;
        double oilPress = 0.0;
        double fuelPress = 0.0;

        double loilTemp = 0.0;
        double loilPress = 0.0;
        double lfuelPress = 0.0;

        RotateTransform rtOilTemp = new RotateTransform();
        RotateTransform rtOilPress = new RotateTransform();
        RotateTransform rtFuelPress = new RotateTransform();

        public int GetWindowID() { return windowID; }

        public L39_OilP()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            rtOilTemp.Angle =  - 40;
            OilT.RenderTransform = rtOilTemp;
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

                               if (vals.Length > 0) { oilTemp = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { oilPress = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { fuelPress = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (oilTemp < 0) oilTemp = 0;
                               if (oilPress < 0) oilPress = 0;
                               if (fuelPress < 0) fuelPress = 0;

                               if (loilTemp != oilTemp)
                               {
                                   rtOilTemp.Angle = (145 * oilTemp) - 40;
                                   OilT.RenderTransform = rtOilTemp;
                               }

                               if (loilPress != oilPress)
                               {
                                   rtOilPress.Angle = -145 * oilPress;
                                   OilP.RenderTransform = rtOilPress;
                               }

                               if (lfuelPress != fuelPress)
                               {
                                   rtFuelPress.Angle = 220 * fuelPress;
                                   FuelP.RenderTransform = rtFuelPress;
                               }
                               loilTemp = oilTemp;
                               loilPress = oilPress;
                               lfuelPress = fuelPress;
                           }
                           catch { return; }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}
