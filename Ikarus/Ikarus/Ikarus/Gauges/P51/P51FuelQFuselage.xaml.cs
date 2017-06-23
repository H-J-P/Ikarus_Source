using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for P51FuelQFuselage.xaml
    /// </summary>
    public partial class P51FuelQFuselage : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }
        double tank = 0.0;
        double ltank = 0.0;

        RotateTransform rtTank = new RotateTransform();

        public P51FuelQFuselage()
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
            helper.SetInput(ref _input, ref valueScale, ref valueScaleIndex, 3);
        }

        public void SetOutput(string _output)
        {
            helper.SetOutput(ref _output, ref degreeDial, 3);
        }

        public double GetSize()
        {
            return 255.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (ltank != tank)
                               {
                                   // Fuel_Tank_Fuselage.input		                = {0.0, 10.0, 20.0, 30.0, 40.0, 50.0, 60.0, 70.0, 80.0, 85.0} -- US GAL
                                   //double[] valueScale = new double[valueScaleIndex] { 0.0, 0.12, 0.28, 0.40, 0.51, 0.62, 0.72, 0.83, 0.96, 1.00 };
                                   //double[] degreeDial = new double[valueScaleIndex] { 0.0, 23.0, 51.0, 75.0, 96.0, 114, 132, 154, 176, 186 };

                                   if (vals.Length > 0) { tank = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (tank >= valueScale[n] && tank <= valueScale[n + 1])
                                       {
                                           rtTank.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (tank - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", tank.ToString(), Convert.ToInt32(rtTank.Angle).ToString());
                                   }
                                   Fuel_Fuselage.RenderTransform = rtTank;
                               }
                               ltank = tank;
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
