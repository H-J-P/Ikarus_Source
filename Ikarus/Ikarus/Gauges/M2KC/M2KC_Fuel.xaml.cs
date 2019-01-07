using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for M2KC_Fuel.xaml
    /// </summary>
    public partial class M2KC_Fuel : UserControl, I_Ikarus
    {
        private string dataImportID = "";

        GaugesHelper helper = null;
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }

        double jaugFuelX00 = 0.0;
        double jaugFuel0X0 = 0.0;
        double jaugFuel00X = 0.0;

        TranslateTransform ttjaugFuelX00 = new TranslateTransform();
        TranslateTransform ttjaugFuel0X0 = new TranslateTransform();
        TranslateTransform ttjaugFuel00X = new TranslateTransform();

        double detotFuelX00 = 0.0;
        double detotFuel0X0 = 0.0;
        double detotFuel00X = 0.0;

        TranslateTransform ttdetotFuelX00 = new TranslateTransform();
        TranslateTransform ttdetotFuel0X0 = new TranslateTransform();
        TranslateTransform ttdetotFuel00X = new TranslateTransform();

        double needleLeft = 0.0;
        double needleRight = 0.0;

        TranslateTransform ttneedleLeft = new TranslateTransform();
        TranslateTransform ttneedleRight = new TranslateTransform();

        double ljaugFuelX00 = 0.0;
        double ljaugFuel0X0 = 0.0;
        double ljaugFuel00X = 0.0;
        double ldetotFuelX00 = 0.0;
        double ldetotFuel0X0 = 0.0;
        double ldetotFuel00X = 0.0;
        double lneedleLeft = 0.0;
        double lneedleRight = 0.0;

        public M2KC_Fuel()
        {
            InitializeComponent();

            //shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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
            //helper.SetInput(ref _input, ref valueScale, ref valueScaleIndex, 2);
        }

        public void SetOutput(string _output)
        {
            //helper.SetOutput(ref _output, ref degreeDial, 2);
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
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { jaugFuelX00 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { jaugFuel0X0 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { jaugFuel00X = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { detotFuelX00 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { detotFuel0X0 = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { detotFuel00X = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { needleLeft = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { needleRight = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }

                               if (jaugFuelX00 < 0) jaugFuelX00 = 0;
                               if (jaugFuel0X0 < 0) jaugFuel0X0 = 0;
                               if (jaugFuel00X < 0) jaugFuel00X = 0;
                               if (detotFuelX00 < 0) detotFuelX00 = 0;
                               if (detotFuel0X0 < 0) detotFuel0X0 = 0;
                               if (detotFuel00X < 0) detotFuel00X = 0;
                               if (needleLeft < 0) needleLeft = 0;
                               if (needleRight < 0) needleRight = 0;
                               if (needleLeft > 0.7) needleLeft = 0.7;
                               if (needleRight > 0.7) needleRight = 0.7;

                               needleLeft *= 1.429;
                               needleRight *= 1.429;

                               if (ljaugFuelX00 != jaugFuelX00)
                               {
                                   ttjaugFuelX00.Y = jaugFuelX00 * -298;
                                   JAUG_1000.RenderTransform = ttjaugFuelX00;
                               }
                               if (ljaugFuel0X0 != jaugFuel0X0)
                               {
                                   ttjaugFuel0X0.Y = jaugFuel0X0 * -298;
                                   JAUG_100.RenderTransform = ttjaugFuel0X0;
                               }
                               if (ljaugFuel00X != jaugFuel00X)
                               {
                                   ttjaugFuel00X.Y = jaugFuel00X * -298;
                                   JAUG_10.RenderTransform = ttjaugFuel00X;
                               }
                               if (ldetotFuelX00 != detotFuelX00)
                               {
                                   ttdetotFuelX00.Y = detotFuelX00 * -298;
                                   DETOT_1000.RenderTransform = ttdetotFuelX00;
                               }
                               if (ldetotFuel0X0 != detotFuel0X0)
                               {
                                   ttdetotFuel0X0.Y = detotFuel0X0 * -298;
                                   DETOT_100.RenderTransform = ttdetotFuel0X0;
                               }
                               if (ldetotFuel00X != detotFuel00X)
                               {
                                   ttdetotFuel00X.Y = detotFuel00X * -298;
                                   DETOT_10.RenderTransform = ttdetotFuel00X;
                               }
                               if (lneedleLeft != needleLeft)
                               {
                                   ttneedleLeft.Y = needleLeft * -312;
                                   LEFT_FUEL.RenderTransform = ttneedleLeft;
                               }
                               if (lneedleRight != needleRight)
                               {
                                   ttneedleRight.Y = needleRight * -312;
                                   RIGHT_FUEL.RenderTransform = ttneedleRight;
                               }

                               ljaugFuelX00 = jaugFuelX00;
                               ljaugFuel0X0 = jaugFuel0X0;
                               ljaugFuel00X = jaugFuel00X;
                               ldetotFuelX00 = detotFuelX00;
                               ldetotFuel0X0 = detotFuel0X0;
                               ldetotFuel00X = detotFuel00X;
                               lneedleLeft = needleLeft;
                               lneedleRight = needleRight;
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
