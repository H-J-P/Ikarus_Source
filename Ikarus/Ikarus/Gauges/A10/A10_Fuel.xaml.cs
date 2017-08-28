using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for A10_Fuel.xaml
    /// </summary>
    public partial class A10_Fuel : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double fuel10000 = 0.0;
        double fuel1000 = 0.0;
        double fuel100 = 0.0;
        double fuelQuantityLeft = 0.0;
        double fuelQuantityRight = 0.0;

        double lfuel10000 = 0.0;
        double lfuel1000 = 0.0;
        double lfuel100 = 0.0;
        double lfuelQuantityLeft = 0.0;
        double lfuelQuantityRight = 0.0;

        RotateTransform rtfuelQuantityLeft = new RotateTransform();
        RotateTransform rtfuelQuantityRight = new RotateTransform();
        TranslateTransform trfuel10000 = new TranslateTransform();
        TranslateTransform trfuel1000 = new TranslateTransform();
        TranslateTransform trfuel100 = new TranslateTransform();

        public A10_Fuel()
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

                               if (vals.Length > 0) { fuel10000 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { fuel1000 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { fuel100 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { fuelQuantityLeft = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { fuelQuantityRight = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               if (fuelQuantityLeft < 0.0) fuelQuantityLeft = 0.0;
                               if (fuelQuantityRight < 0.0) fuelQuantityRight = 0.0;

                               if (lfuelQuantityLeft != fuelQuantityLeft)
                               {
                                   rtfuelQuantityLeft.Angle = fuelQuantityLeft * 163;
                                   TotalFuel_left.RenderTransform = rtfuelQuantityLeft;
                               }
                               if (lfuelQuantityRight != fuelQuantityRight)
                               {
                                   rtfuelQuantityRight.Angle = fuelQuantityRight * -163;
                                   TotalFuel_right.RenderTransform = rtfuelQuantityRight;
                               }
                               if (lfuel10000 != fuel10000)
                               {
                                   trfuel10000.Y = fuel10000 * -270;
                                   TotalFuel_10_000.RenderTransform = trfuel10000;
                               }
                               if (lfuel1000 != fuel1000)
                               {
                                   trfuel1000.Y = fuel1000 * -270;
                                   TotalFuel_1_000.RenderTransform = trfuel1000;
                               }
                               if (lfuel100 != fuel100)
                               {
                                   trfuel100.Y = fuel100 * -270;
                                   TotalFuel_100.RenderTransform = trfuel100;
                               }
                               lfuel10000 = fuel10000;
                               lfuel1000 = fuel1000;
                               lfuel100 = fuel100;
                               lfuelQuantityLeft = fuelQuantityLeft;
                               lfuelQuantityRight = fuelQuantityRight;
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
