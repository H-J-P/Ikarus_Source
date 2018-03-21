using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für SA342_ALT.xaml
    /// </summary>
    public partial class SA342_ALT : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        private double alt0100 = 0.0;
        private double alt1000 = 0.0;
        private double baro0001 = 0.0;
        private double baro0010 = 0.0;
        private double baro0100 = 0.0;
        private double baro1000 = 0.0;

        private double lalt0100 = 0.0;
        private double lalt1000 = 0.0;
        private double lbaro0001 = 0.0;
        private double lbaro0010 = 0.0;
        private double lbaro0100 = 0.0;
        private double lbaro1000 = 0.0;

        RotateTransform rtAlt0100 = new RotateTransform();
        RotateTransform rtAlt1000 = new RotateTransform();
        TranslateTransform ttBaro0001 = new TranslateTransform();
        TranslateTransform ttBaro0010 = new TranslateTransform();
        TranslateTransform ttBaro0100 = new TranslateTransform();
        TranslateTransform ttBaro1000 = new TranslateTransform();

        public int GetWindowID() { return windowID; }

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

        public SA342_ALT()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { alt1000 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { alt0100 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { baro0001 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { baro0010 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { baro0100 = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { baro1000 = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }

                               if (lalt1000 != alt1000)
                               {
                                   rtAlt1000.Angle = alt1000 * 360;
                                   ALT_1000.RenderTransform = rtAlt1000;
                               }

                               if (lalt0100 != alt0100)
                               {
                                   rtAlt0100.Angle = alt0100 * 360;
                                   ALT_100.RenderTransform = rtAlt0100;
                               }

                               if (lbaro0001 != baro0001)
                               {
                                   ttBaro0001.Y = baro0001 * -253;
                                   BasicAtmospherePressure_1.RenderTransform = ttBaro0001;
                               }
                               if (lbaro0010 != baro0010)
                               {
                                   ttBaro0010.Y = baro0010 * -253;
                                   BasicAtmospherePressure_10.RenderTransform = ttBaro0010;
                               }
                               if (lbaro0100 != baro0100)
                               {
                                   ttBaro0100.Y = baro0100 * -253;
                                   BasicAtmospherePressure_100.RenderTransform = ttBaro0100;
                               }
                               if (lbaro1000 != baro1000)
                               {
                                   ttBaro1000.Y = baro1000 * -253;
                                   BasicAtmospherePressure_1000.RenderTransform = ttBaro1000;
                               }

                               lalt1000 = alt1000;
                               lalt0100 = alt0100;
                               lbaro0001 = baro0001;
                               lbaro0010 = baro0010;
                               lbaro0100 = baro0100;
                               lbaro1000 = baro1000;
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
