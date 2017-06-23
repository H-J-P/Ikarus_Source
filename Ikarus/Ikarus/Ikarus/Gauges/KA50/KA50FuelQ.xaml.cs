using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for KA50FuelQ.xaml
    /// </summary>
    public partial class KA50FuelQ : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double rearTankAmount = 0.0;
        double forwardTankAmount = 0.0;
        double lampForwardTank = 0.0;
        double lampRearTank = 0.0;

        double lrearTankAmount = 0.0;
        double lforwardTankAmount = 0.0;
        double llampForwardTank = 0.0;
        double llampRearTank = 0.0;

        RotateTransform rtRearTankAmount = new RotateTransform();
        RotateTransform rtForwardTankAmount = new RotateTransform();

        public KA50FuelQ()
        {
            InitializeComponent();

            KA50_needleFR_FuelQ.Visibility = System.Windows.Visibility.Hidden;
            KA50_needleRR_FuelQ.Visibility = System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { rearTankAmount = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { forwardTankAmount = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { lampForwardTank = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { lampRearTank = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (lforwardTankAmount != forwardTankAmount)
                               {
                                   rtForwardTankAmount.Angle = forwardTankAmount * 300;
                                   KA50_needleR_FuelQ.RenderTransform = rtForwardTankAmount;
                               }
                               if (lrearTankAmount != rearTankAmount)
                               {
                                   rtRearTankAmount.Angle = rearTankAmount * 300;
                                   KA50_needleF_FuelQ.RenderTransform = rtRearTankAmount;
                               }

                               KA50_needleFR_FuelQ.Visibility = (lampForwardTank > 0.5) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               KA50_needleRR_FuelQ.Visibility = (lampRearTank > 0.5) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lrearTankAmount = rearTankAmount;
                               lforwardTankAmount = forwardTankAmount;
                               llampForwardTank = lampForwardTank;
                               llampRearTank = lampRearTank;
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
